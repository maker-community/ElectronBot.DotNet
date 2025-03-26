using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verdure.Braincase.Core.Configuration;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.WinUI.Common;
using Windows.ApplicationModel;

namespace Verdure.VoiceAssistant.Handlers;

/// <summary>
/// A wake word listener using Azure Cognitive Services keyword recognition.
/// </summary>
public class AzCognitiveServicesWakeWordListener : IWakeWordListener
{
    private readonly ILogger _logger;
    private readonly AzureCognitiveServicesOptions _options;
    private readonly AudioConfig _audioConfig;
    private readonly KeywordRecognizer _keywordRecognizer;
    private readonly KeywordRecognitionModel _keywordModel;
    private readonly IElectronBotPlayer _electronBotPlayer;
    private readonly IMemoryCache _memoryCache;
    private readonly ILocalSettingsService _localSettingsService;
    public AzCognitiveServicesWakeWordListener(
        IOptions<AzureCognitiveServicesOptions> options,
        ILogger<AzCognitiveServicesWakeWordListener> logger,
        IElectronBotPlayer electronBotPlayer,
        IMemoryCache memoryCache,
        ILocalSettingsService localSettingsService)
    {
        _logger = logger;
        _options = options.Value;
        var keywordModelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Keyword\\{_options.WakePhraseModel}";
        _keywordModel = KeywordRecognitionModel.FromFile(keywordModelPath);
        _audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        _keywordRecognizer = new KeywordRecognizer(_audioConfig);
        _electronBotPlayer = electronBotPlayer;
        _memoryCache = memoryCache;
        _localSettingsService = localSettingsService;
    }

    /// <summary>
    /// Wait for the wake word or phrase to be detected before returning.
    /// </summary>
    public async Task<bool> WaitForWakeWordAsync(CancellationToken cancellationToken)
    {
        KeywordRecognitionResult result;
        do
        {
            try
            {
                // 启动动画但不阻塞当前执行流程
                var animationTask = _electronBotPlayer.PlayLottieByNameIdAsync("look", -1);

                // 可以选择添加异常处理
                animationTask?.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        _logger.LogError($"Animation playback failed: {t.Exception}");
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                await _electronBotPlayer.StopLottiePlaybackAsync();
                _logger.LogError($"Failed to start animation: {ex.Message}");
                // 根据需要处理异常
            }
            _logger.LogInformation($"Waiting for wake phrase...");
            result = await _keywordRecognizer.RecognizeOnceAsync(_keywordModel);
            _logger.LogInformation("Wake phrase detected.");
            _logger.LogDebug($"{result.Reason}");

            // 停止动画
            await _electronBotPlayer.StopLottiePlaybackAsync();

        } while (result.Reason != ResultReason.RecognizedKeyword);
        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _audioConfig.Dispose();
    }
}