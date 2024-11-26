using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;
public class GenerateImageResponse
{
    public GenerateImageOutput Output
    {
        get; set;
    } = new GenerateImageOutput();

    [JsonPropertyName("request_id")]
    public string RequestId
    {
        get; set;
    } = string.Empty;
}

public class GenerateImageOutput
{
    [JsonPropertyName("task_status")]
    public string TaskStatus
    {
        get; set;
    } = string.Empty;

    [JsonPropertyName("task_id")]
    public string TaskId
    {
        get; set;
    } = string.Empty;
}
