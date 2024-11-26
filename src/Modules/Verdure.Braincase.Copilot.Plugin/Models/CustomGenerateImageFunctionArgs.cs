using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;

public class CustomGenerateImageFunctionArgs
{
    [JsonPropertyName("image_name")]
    public string ImageName { get; set; } = string.Empty;

    [JsonPropertyName("image_description")]
    public string ImageDescription { get; set; } = string.Empty;
}
