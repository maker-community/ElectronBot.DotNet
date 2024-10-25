using System.Text.Json.Serialization;

namespace Verdure.Plugin.Copilot.Models;

public class ControlTheLightsFunctionArgs
{
    [JsonPropertyName("light_status")]
    public bool LightStatus
    {
        get; set;
    }
}
