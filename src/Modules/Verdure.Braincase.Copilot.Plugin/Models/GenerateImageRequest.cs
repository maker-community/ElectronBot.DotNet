using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;
public class GenerateImageRequest
{
    public string Model
    {
        get; set;
    } = "wanx-v1";
    public GenerateImageInput Input
    {
        get; set;
    } = new GenerateImageInput();
    public GenerateImageParameters Parameters
    {
        get; set;
    } = new GenerateImageParameters();
}

public class GenerateImageInput
{
    public string Prompt
    {
        get; set;
    } = string.Empty;

    [JsonPropertyName("negative_prompt")]
    public string NegativePrompt
    {
        get; set;
    } = string.Empty;
}

public class GenerateImageParameters
{
    public string Style
    {
        get; set;
    } = "<auto>";
    public string Size
    {
        get; set;
    } = "1024*1024";
    public int N
    {
        get; set;
    } = 1;
}

