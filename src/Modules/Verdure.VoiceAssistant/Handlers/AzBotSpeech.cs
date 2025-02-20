using System.Text.RegularExpressions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.VoiceAssistant.Configuration;

namespace Verdure.VoiceAssistant.Handlers;
public class AzBotSpeech : IBotSpeech
{
    private readonly ILogger _logger;
    private readonly AzureCognitiveServicesOptions _options;
    private readonly AudioConfig _audioConfig;
    private readonly SpeechRecognizer _speechRecognizer;
    private readonly SpeechSynthesizer _speechSynthesizer;

    /// <summary>
    /// Regex for extracting style cues from OpenAI responses.
    /// (not currently supported after the migrations to ChatGPT models)
    /// </summary>
    private static readonly Regex _styleRegex = new Regex(@"(~~(.+)~~)");

    public AzBotSpeech(IOptions<AzureCognitiveServicesOptions> options,
            ILogger<AzBotSpeech> logger)
    {
        _logger = logger;
        _options = options.Value;
        _options.Validate();

        _audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        SpeechConfig speechConfig = SpeechConfig.FromSubscription(_options.Key, _options.Region);
        speechConfig.SpeechRecognitionLanguage = _options.SpeechRecognitionLanguage;
        speechConfig.SetProperty(PropertyId.SpeechServiceResponse_PostProcessingOption, "TrueText");
        speechConfig.SpeechSynthesisVoiceName = _options.SpeechSynthesisVoiceName;

        _speechRecognizer = new SpeechRecognizer(speechConfig, _audioConfig);
        _speechSynthesizer = new SpeechSynthesizer(speechConfig);
    }
    public async Task<string> ListenAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Listening...");
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

            var ssml = GenerateSsml(
                text,
                _options.EnableSpeechStyle ? style : string.Empty,
                _options.SpeechSynthesisVoiceName);

            _logger.LogDebug(ssml);
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
    private string GenerateSsml(string message, string style, string voiceName)
        => "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"https://www.w3.org/2001/mstts\" xml:lang=\"en-US\">" +
            $"<voice name=\"{voiceName}\">" +
                $"<prosody rate=\"{_options.Rate}\">" +
                    $"<mstts:express-as style=\"{style}\">" +
                        $"{message}" +
                    "</mstts:express-as>" +
                "</prosody>" +
                "</voice>" +
            "</speak>";

    public void Dispose()
    {
        _speechRecognizer.Dispose();
        _audioConfig.Dispose();
    }
}
