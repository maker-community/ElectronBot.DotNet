using System.Text.RegularExpressions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Verdure.Braincase.Core.Configuration;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.WinUI.Common;

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
    /// <summary>
    /// Regex for extracting style cues from OpenAI responses.
    /// (not currently supported after the migrations to ChatGPT models)
    /// </summary>
    private static readonly Regex _styleRegex = new Regex(@"(~~(.+)~~)");

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
        var options = await _localSettingsService.ReadSettingAsync<AzureCognitiveServicesOptions>(Constants.AzureLlmVoiceConfigKey);
        if (options != null)
        {
            _options = options;
            options.Validate();
            _audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            SpeechConfig speechConfig = SpeechConfig.FromSubscription(options.Key, options.Region);
            speechConfig.SpeechRecognitionLanguage = options.SpeechRecognitionLanguage;
            speechConfig.SetProperty(PropertyId.SpeechServiceResponse_PostProcessingOption, "TrueText");
            speechConfig.SpeechSynthesisVoiceName = options.SpeechSynthesisVoiceName;

            _speechRecognizer = new SpeechRecognizer(speechConfig, _audioConfig);
            _speechSynthesizer = new SpeechSynthesizer(speechConfig);
        }
    }
    public async Task<string> ListenAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Listening...");

            var name = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);
            if (name != "ClockMode")
            {
                _ = _electronBotPlayer.PlayLottieByNameIdAsync("think");
            }

            SpeechRecognitionResult result = await _speechRecognizer.RecognizeOnceAsync();
            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    _logger.LogInformation($"Recognized: {result.Text}");
                    return result.Text;
                case ResultReason.Canceled:
                    _logger.LogWarning($"Speech recognizer session canceled.");

                    CancellationDetails cancelDetails = CancellationDetails.FromResult(result);
                    _logger.LogWarning($"{cancelDetails.Reason}: {cancelDetails.ErrorCode}");
                    _logger.LogDebug(cancelDetails.ToString());
                    break;
            }
        }
        return string.Empty;
    }
    public async Task SpeakAsync(string text, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            // Parse speaking style, if any
            text = ExtractStyle(text, out var style);
            if (string.IsNullOrWhiteSpace(style))
            {
                _logger.LogInformation($"Speaking (none): {text}");
            }
            else
            {
                _logger.LogInformation($"Speaking ({style}): {text}");
            }

            var ssml = GenerateCoquettishSsml(
                text,
                _options.SpeechSynthesisVoiceName);

            _logger.LogDebug(ssml);
            var name = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);
            if (name != "ClockMode")
            {
                _ = _electronBotPlayer.PlayLottieByNameIdAsync("speak", 2);
            }

            await _speechSynthesizer.SpeakSsmlAsync(ssml);
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

    public void Dispose()
    {
        _speechRecognizer?.Dispose();
        _audioConfig?.Dispose();
    }
}
