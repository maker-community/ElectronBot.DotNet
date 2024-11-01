using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;

public class LearnWordsFunctionArgs
{
    [JsonPropertyName("word")]
    public string Word { get; set; } = string.Empty;

    [JsonPropertyName("word_description")]
    public string WordDescription { get; set; } = string.Empty;
}
