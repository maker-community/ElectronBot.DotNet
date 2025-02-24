
using Microsoft.Extensions.Logging;
using Verdure.Braincase.Core.Contracts.Services;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;

namespace Verdure.VoiceAssistant.Handlers;
public class DefaultBotSpeech : IBotSpeech
{
    public string Provider => "DefaultVoice";
    private readonly SpeechRecognizer _speechRecognizer;
    private readonly SpeechSynthesizer _speechSynthesizer;
    private readonly MediaPlayer _mediaPlayer = new();
    private readonly ILogger _logger;
    public DefaultBotSpeech(ILogger<DefaultBotSpeech> logger)
    {
        _logger = logger;
        _speechSynthesizer = new SpeechSynthesizer();
        _speechRecognizer = new SpeechRecognizer(SpeechRecognizer.SystemSpeechLanguage);
        var webSearchGrammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.WebSearch, "webSearch");
        _speechRecognizer.Constraints.Add(webSearchGrammar);
    }
    public Task InitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
    public async Task<string> ListenAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Listening...");
            SpeechRecognitionResult result = await _speechRecognizer.RecognizeAsync();
            switch (result.Status)
            {
                case SpeechRecognitionResultStatus.Success:
                    _logger.LogInformation($"Recognized: {result.Text}");
                    return result.Text;
                case SpeechRecognitionResultStatus.UserCanceled:
                    _logger.LogWarning($"Speech recognizer session canceled.");
                    break;
            }
        }
        return string.Empty;
    }
    public async Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(text))
        {
            // Create a stream from the text. This will be played using a media element.
            var synthesisStream = await _speechSynthesizer.SynthesizeTextToStreamAsync(text);
            _mediaPlayer.SetStreamSource(synthesisStream);
            _mediaPlayer.Play();
        }
    }

    public void Dispose()
    {
        _speechSynthesizer.Dispose();
        _speechRecognizer.Dispose();
        _mediaPlayer.Dispose();
    }
}
