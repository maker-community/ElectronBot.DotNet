using BotSharp.Abstraction.Conversations.Models;
using System.Text.Encodings.Web;
using System.Text.Json;
using Verdure.Braincase.Copilot.Plugin.Models;

namespace Verdure.Braincase.Copilot.Plugin.Functions;

public class ControlTheLightsFn : IFunctionCallback
{
    public string Name => "control_the_lights";
    private readonly JsonSerializerOptions _options;
    public ControlTheLightsFn()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public async Task<bool> Execute(RoleDialogModel message)
    {
        var args = JsonSerializer.Deserialize<ControlTheLightsFunctionArgs>(message.FunctionArgs ?? "", _options) ?? new ControlTheLightsFunctionArgs();

        message.Data = new
        {
            pepperoni_unit_price = 3.2,
            cheese_unit_price = 3.5,
            margherita_unit_price = 3.8,
        };
        message.Content = JsonSerializer.Serialize(message.Data);
        return true;
    }
}
