using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;

public class ChangeAppModeFunctionArgs
{
    [JsonPropertyName("mode_name")]
    public string ModeName
    {
        get; set;
    } = string.Empty;
}
