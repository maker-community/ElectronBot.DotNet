using System.Text.Encodings.Web;
using System.Text.Json;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.Messaging;
using Verdure.Braincase.Copilot.Plugin.Models;
using Verdure.Braincase.Core.Models;

namespace Verdure.Braincase.Copilot.Plugin.Functions;

public class ChangeAppModeFn : IFunctionCallback
{
    public string Name => "change_app_mode";
    private readonly JsonSerializerOptions _options;
    public ChangeAppModeFn()
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

    public Task<bool> Execute(RoleDialogModel message)
    {
        var args = JsonSerializer.Deserialize<ChangeAppModeFunctionArgs>(message.FunctionArgs ?? "", _options) ?? new ChangeAppModeFunctionArgs();

        var clockView = new ChangeAppMode
        {
            ModeName = args.ModeName
        };
        WeakReferenceMessenger.Default.Send(clockView);
        return Task.FromResult(true);
    }
}
