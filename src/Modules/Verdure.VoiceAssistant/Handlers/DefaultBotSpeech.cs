
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Microsoft.Extensions.Logging;
using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.VoiceAssistant.Handlers;
public class DefaultBotSpeech : IBotSpeech
{
    public string Provider => "DefaultVoice";
    private readonly SpeechRecognitionEngine _speechRecognizer;
    private readonly SpeechSynthesizer _speechSynthesizer;
    private readonly ILogger _logger;
    public DefaultBotSpeech(ILogger<DefaultBotSpeech> logger)
    {
        _logger = logger;
        _speechSynthesizer = new SpeechSynthesizer();
        _speechRecognizer = new SpeechRecognitionEngine();
        var dictationGrammar = new DictationGrammar();
        _speechRecognizer.LoadGrammar(dictationGrammar);
    }
    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        await Task.Run(() => _speechRecognizer.SetInputToDefaultAudioDevice(), cancellationToken);
    }
    public async Task<string> ListenAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Listening...");
            var result = await Task.Run(() => _speechRecognizer.Recognize(), cancellationToken);
            if (result != null)
            {
                _logger.LogInformation($"Recognized: {result.Text}");
                return result.Text;
            }
            else
            {
                _logger.LogWarning("Speech recognizer session canceled.");
            }
        }
        return string.Empty;
    }
    public async Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(text))
        {
            await Task.Run(() => _speechSynthesizer.Speak(text), cancellationToken);
        }
    }

    public void Dispose()
    {
        _speechSynthesizer.Dispose();
        _speechRecognizer.Dispose();
    }
}
