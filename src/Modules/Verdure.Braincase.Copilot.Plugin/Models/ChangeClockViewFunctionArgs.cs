using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;

public class ChangeClockViewFunctionArgs
{
    [JsonPropertyName("clock_view")]
    public string ClockView
    {
        get; set;
    } = string.Empty;
}
