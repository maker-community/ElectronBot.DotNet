using System.Text.Encodings.Web;
using System.Text.Json;

namespace Verdure.Braincase.Core.Helpers;

public static class Json
{
    public static async Task<T> ToObjectAsync<T>(string value, JsonSerializerOptions? options = null)
    {
        return await Task.Run<T>(() =>
        {
            options ??= new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                AllowTrailingCommas = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return JsonSerializer.Deserialize<T>(value, options);
        });
    }

    public static async Task<string> StringifyAsync(object value, JsonSerializerOptions? options = null)
    {
        return await Task.Run<string>(() =>
        {
            options ??= new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                AllowTrailingCommas = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            return JsonSerializer.Serialize(value, options);
        });
    }
}
