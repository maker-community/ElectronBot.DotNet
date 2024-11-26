using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;

public class ControlTheLightsFunctionArgs
{
    [JsonPropertyName("light_status")]
    public bool LightStatus
    {
        get; set;
    }
}
