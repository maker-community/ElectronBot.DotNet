namespace Verdure.Braincase.Core.Configuration;

/// <summary>
/// Configuration options class for interacting with Azure Cognitive Services.
/// </summary>
public class AzureCognitiveServicesOptions
{
    /// <summary>
    /// Location/region (e.g. EastUS)
    /// </summary>
    public string Region
    {
        get; set;
    }

    /// <summary>
    /// Access Key
    /// </summary>
    public string Key
    {
        get; set;
    }

    /// <summary>
    /// Default language for speech recognition (speech-to-text).
    /// </summary>
    public string SpeechRecognitionLanguage
    {
        get; set;
    }

    /// <summary>
    /// Name of the voice to use for speaking (text-to-speech).
    /// </summary>
    /// <remarks>
    /// https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support?tabs=stt-tts#text-to-speech
    /// </remarks>
    public string SpeechSynthesisVoiceName
    {
        get; set;
    }

    /// <summary>
    /// True to enable style cues when speaking.
    /// </summary>
    public bool EnableSpeechStyle
    {
        get; set;
    }

    /// <summary>
    /// Filename of the wake phrase model to use.
    /// </summary>
    public string WakePhraseModel
    {
        get; set;
    }

    /// <summary>
    /// Indicates the speaking rate of the text.
    /// https://learn.microsoft.com/en-us/azure/cognitive-services/speech-service/speech-synthesis-markup-voice#adjust-prosody
    /// </summary>
    public string Rate
    {
        get; set;
    }

    /// <summary>
    /// Validate options, throw an exception is any are invalid.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Region))
            throw new ArgumentException("Argument is invalid.", nameof(Region));

        if (string.IsNullOrWhiteSpace(Key))
            throw new ArgumentException("Argument is invalid.", nameof(Key));

        if (string.IsNullOrWhiteSpace(SpeechRecognitionLanguage))
            throw new ArgumentException("Argument is invalid.", nameof(SpeechRecognitionLanguage));

        if (string.IsNullOrWhiteSpace(SpeechSynthesisVoiceName))
            throw new ArgumentException("Argument is invalid.", nameof(SpeechSynthesisVoiceName));
    }
}