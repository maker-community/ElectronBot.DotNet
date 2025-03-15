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
            var name = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);
            if (name != "ClockMode")
            {
                _ = _electronBotPlayer.PlayLottieByNameIdAsync("look", -1);
            }
            _logger.LogInformation($"Waiting for wake phrase...");
            result = await _keywordRecognizer.RecognizeOnceAsync(_keywordModel);
            _logger.LogInformation("Wake phrase detected.");
            _logger.LogDebug($"{result.Reason}");
        } while (result.Reason != ResultReason.RecognizedKeyword);
        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _audioConfig.Dispose();
    }
}