using System.Text.Json.Serialization;

namespace Verdure.Braincase.Copilot.Plugin.Models;
public class ImageTaskResponse
{
    [JsonPropertyName("request_id")]
    public string RequestId
    {
        get; set;
    }
    public ImageTaskOutput Output
    {
        get; set;
    }
    public ImageTaskUsage Usage
    {
        get; set;
    }
}

public class ImageTaskOutput
{
    [JsonPropertyName("task_id")]
    public string TaskId
    {
        get; set;
    }
    [JsonPropertyName("task_status")]
    public string TaskStatus
    {
        get; set;
    }
    [JsonPropertyName("submit_time")]
    public string SubmitTime
    {
        get; set;
    }
    [JsonPropertyName("scheduled_time")]
    public string ScheduledTime
    {
        get; set;
    }
    [JsonPropertyName("end_time")]
    public string EndTime
    {
        get; set;
    }
    public List<ImageTaskResult> Results
    {
        get; set;
    }
    [JsonPropertyName("task_metrics")]
    public TaskMetrics TaskMetrics
    {
        get; set;
    }
}

public class TaskMetrics
{
    public int TOTAL
    {
        get; set;
    }
    public int SUCCEEDED
    {
        get; set;
    }
    public int FAILED
    {
        get; set;
    }
}

public class ImageTaskResult
{
    public string Url
    {
        get; set;
    }
}

public class ImageTaskUsage
{
    [JsonPropertyName("image_count")]
    public int ImageCount
    {
        get; set;
    }
}
