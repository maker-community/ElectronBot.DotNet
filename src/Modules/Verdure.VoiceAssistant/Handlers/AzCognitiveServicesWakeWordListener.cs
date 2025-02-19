using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.VoiceAssistant.Configuration;
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

    public AzCognitiveServicesWakeWordListener(
        IOptions<AzureCognitiveServicesOptions> options,
        ILogger<AzCognitiveServicesWakeWordListener> logger)
    {
        _logger = logger;
        _options = options.Value;
        var keywordModelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Keyword\\{_options.WakePhraseModel}";
        _keywordModel = KeywordRecognitionModel.FromFile(keywordModelPath);
        _audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        _keywordRecognizer = new KeywordRecognizer(_audioConfig);
    }

    /// <summary>
    /// Wait for the wake word or phrase to be detected before returning.
    /// </summary>
    public async Task<bool> WaitForWakeWordAsync(CancellationToken cancellationToken)
    {
        KeywordRecognitionResult result;
        do
        {
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