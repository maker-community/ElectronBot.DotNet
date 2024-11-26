using System.Text.Encodings.Web;
using System.Text.Json;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.Messaging;
using Verdure.Braincase.Copilot.Plugin.Models;
using Verdure.Braincase.Core.Models;

namespace Verdure.Braincase.Copilot.Plugin.Functions;

public class ChangeClockViewFn : IFunctionCallback
{
    public string Name => "change_clock_view";
    private readonly JsonSerializerOptions _options;
    public ChangeClockViewFn()
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
        var args = JsonSerializer.Deserialize<ChangeClockViewFunctionArgs>(message.FunctionArgs ?? "", _options) ?? new ChangeClockViewFunctionArgs();

        var clockView = new ChangeClockView
        {
            ClockViewName = args.ClockView
        };
        WeakReferenceMessenger.Default.Send(clockView);
        return Task.FromResult(true);
    }
}
