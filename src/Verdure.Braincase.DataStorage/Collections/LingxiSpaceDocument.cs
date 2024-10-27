using System.Text.Json;

namespace Verdure.Braincase.DataStorage.Collections;
public class LingxiSpaceDocument : LiteDBBase
{
    public string Name
    {
        get; set;
    } = string.Empty;

    public string Desc
    {
        get; set;
    } = string.Empty;

    public string Type
    {
        get; set;
    } = string.Empty;

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
    } = string.Empty;
}
