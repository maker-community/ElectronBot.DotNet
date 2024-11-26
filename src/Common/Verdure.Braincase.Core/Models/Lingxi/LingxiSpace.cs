using System.Text.Json;

namespace Verdure.Braincase.Core.Models.Lingxi;
public class LingxiSpace
{
    public string Id
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
    public string Desc
    {
        get; set;
    }
    public string Type
    {
        get; set;
    }
    public JsonDocument? Content
    {
        get; set;
    }

    public DateTime CreatedTime
    {
        get; set;
    }

    public string ConversationId
    {
        get; set;
    }
}
