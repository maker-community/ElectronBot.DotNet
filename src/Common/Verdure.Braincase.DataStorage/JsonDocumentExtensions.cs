using System.Text.Json;

namespace Verdure.Braincase.DataStorage;
public static class JsonDocumentExtensions
{
    public static string ToJsonString(this JsonDocument? jsonDocument)
    {
        return jsonDocument?.RootElement.GetRawText() ?? string.Empty;
    }
}
