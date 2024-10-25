using BotSharp.Abstraction.Conversations.Models;
using Verdure.ElectronBot.Core.Contracts.Services;

namespace Verdure.Plugin.Copilot.Functions;

public class GetWeatherFn : IFunctionCallback
{
    private readonly IBotToolService _botToolService;
    public GetWeatherFn(IBotToolService botToolService)
    {
        _botToolService = botToolService;
    }
    public string Name => "get_weather";

    public async Task<bool> Execute(RoleDialogModel message)
    {
        var result = await _botToolService.SendWeatherToBotAsync();
        message.Content = result;
        return true;
    }
}
