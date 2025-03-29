using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verdure.Braincase.Core.Configuration;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.WinUI.Common;
using Verdure.Braincase.WinUI.Common.Models;

namespace Verdure.VoiceAssistant.Handlers;
public class AzBotSpeech : IBotSpeech
{
    private readonly ILogger _logger;
    private AzureCognitiveServicesOptions _options;
    private AudioConfig _audioConfig;
    private SpeechRecognizer _speechRecognizer;
    private SpeechSynthesizer _speechSynthesizer;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IElectronBotPlayer _electronBotPlayer;
    private readonly IMemoryCache _memoryCache;
    private bool _isInitialized = false;
    /// <summary>
    /// Regex for extracting style cues from OpenAI responses.
    /// (not currently supported after the migrations to ChatGPT models)
    /// </summary>
    private static readonly Regex _styleRegex = new(@"(~~(.+)~~)");

    public string Provider => "AzureVoice";

    public AzBotSpeech(ILogger<AzBotSpeech> logger, ILocalSettingsService localSettingsService, IElectronBotPlayer electronBotPlayer, IMemoryCache memoryCache)
    {
        _logger = logger;
        _localSettingsService = localSettingsService;
        _electronBotPlayer = electronBotPlayer;
        _memoryCache = memoryCache;
    }

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var options = await _localSettingsService.ReadSettingAsync<AzureCognitiveServicesOptions>(Constants.AzureLlmVoiceConfigKey);

            options ??= Ioc.Default.GetRequiredService<IOptions<AzureCognitiveServicesOptions>>().Value;

            _options = options;

            try
            {
                options.Validate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Azure Cognitive Services options validation failed");
                return;
            }

            try
            {
                var subscriptionKey = string.IsNullOrEmpty(options.Key) ? Ioc.Default.GetRequiredService<IOptions<LocalSettingsOptions>>().Value.AzureCognitiveServicesKey : options.Key;

                _audioConfig = AudioConfig.FromDefaultMicrophoneInput();

                SpeechConfig speechConfig = SpeechConfig.FromSubscription(subscriptionKey, options.Region);
                speechConfig.SpeechRecognitionLanguage = options.SpeechRecognitionLanguage;
                speechConfig.SetProperty(PropertyId.SpeechServiceResponse_PostProcessingOption, "TrueText");
                speechConfig.SpeechSynthesisVoiceName = options.SpeechSynthesisVoiceName;

                _speechRecognizer = new SpeechRecognizer(speechConfig, _audioConfig);
                _speechSynthesizer = new SpeechSynthesizer(speechConfig);

                _isInitialized = true;
                _logger.LogInformation("Azure Cognitive Services successfully initialized");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure Cognitive Services");
                CleanupResources();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Azure Cognitive Services initialization");
        }
    }
    public async Task<string> ListenAsync(CancellationToken cancellationToken)
    {
        if (!EnsureInitialized())
        {
            return string.Empty;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _electronBotPlayer.StopLottiePlaybackAsync();
                var animationTask = _electronBotPlayer.PlayLottieByNameIdAsync("look", -1);

                animationTask?.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        _logger.LogError($"Animation playback failed: {t.Exception}");
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);

                _logger.LogInformation("Listening...");

                var result = await _speechRecognizer.RecognizeOnceAsync();

                await _electronBotPlayer.StopLottiePlaybackAsync();

                switch (result.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                        _logger.LogInformation($"Recognized: {result.Text}");
                        return result.Text;
                    case ResultReason.Canceled:
                        var cancelDetails = CancellationDetails.FromResult(result);
                        _logger.LogWarning($"Speech recognition canceled: {cancelDetails.Reason}, Error code: {cancelDetails.ErrorCode}, Error details: {cancelDetails.ErrorDetails}");

                        // 如果是服务错误，可能是订阅问题
                        if (cancelDetails.Reason == CancellationReason.Error)
                        {
                            _logger.LogError($"Speech service error. This could be due to subscription issues or network problems.");
                            // 添加重试逻辑或返回错误信息
                            return string.Empty;
                        }
                        break;
                    case ResultReason.NoMatch:
                        _logger.LogInformation("No speech could be recognized.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during speech recognition");
                // 考虑添加短暂延迟避免在错误条件下快速循环
                await Task.Delay(1000, cancellationToken);
            }
        }
        return string.Empty;
    }
    public async Task SpeakAsync(string text, CancellationToken cancellationToken)
    {
        if (!EnsureInitialized() || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        try
        {
            var animationTask = _electronBotPlayer.PlayLottieByNameIdAsync("speak", -1);
            animationTask?.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogError($"Animation playback failed: {t.Exception}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            text = ExtractStyle(text, out var style);
            _logger.LogInformation($"Speaking ({(string.IsNullOrEmpty(style) ? "none" : style)}): {text}");

            var ssml = GenerateCoquettishSsml(text, _options.SpeechSynthesisVoiceName);
            _logger.LogDebug(ssml);

            var result = await _speechSynthesizer.SpeakSsmlAsync(ssml);

            // 检查语音合成结果
            if (result.Reason == ResultReason.Canceled)
            {
                var cancelDetails = SpeechSynthesisCancellationDetails.FromResult(result);
                _logger.LogError($"Speech synthesis canceled: {cancelDetails.Reason}, Error code: {cancelDetails.ErrorCode}, Error details: {cancelDetails.ErrorDetails}");

                if (cancelDetails.Reason == CancellationReason.Error)
                {
                    _logger.LogError("Speech synthesis failed. This could be due to subscription issues.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during speech synthesis");
        }
        finally
        {
            try
            {
                await _electronBotPlayer.StopLottiePlaybackAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping animation");
            }
        }
    }
    /// <summary>
    /// Extract style cues from a message.
    /// </summary>
    private string ExtractStyle(string message, out string style)
    {
        style = string.Empty;
        Match match = _styleRegex.Match(message);
        if (match.Success)
        {
            style = match.Groups[2].Value.Trim();
            message = message.Replace(match.Groups[1].Value, string.Empty).Trim();
        }
        return message;
    }

    /// <summary>
    /// Generate speech synthesis markup language (SSML) from a message.
    /// </summary>
    private string GenerateCoquettishSsml(string message, string voiceName)
        => "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"https://www.w3.org/2001/mstts\" xml:lang=\"zh-CN\">" +
            $"<voice name=\"{voiceName}\">" +
                "<prosody rate=\"1.1\" pitch=\"high\">" +
                    "<mstts:express-as style=\"cheerful\">" +
                        $"{message}" +
                    "</mstts:express-as>" +
                "</prosody>" +
            "</voice>" +
        "</speak>";

    private bool EnsureInitialized()
    {
        if (!_isInitialized || _options == null || _speechRecognizer == null || _speechSynthesizer == null)
        {
            _logger.LogWarning("Azure Cognitive Services not properly initialized. Call InitAsync first.");
            return false;
        }
        return true;
    }

    private void CleanupResources()
    {
        _speechRecognizer?.Dispose();
        _speechRecognizer = null;

        _speechSynthesizer?.Dispose();
        _speechSynthesizer = null;

        _audioConfig?.Dispose();
        _audioConfig = null;

        _isInitialized = false;
    }

    public void Dispose()
    {
        CleanupResources();
    }
}
