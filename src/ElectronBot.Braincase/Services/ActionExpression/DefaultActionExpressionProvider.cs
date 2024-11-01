using Verdure.Braincase.Contracts.Services;
using Verdure.Braincase.Models;
using Verdure.Braincase.WinUI.Common.Models;

namespace Verdure.Braincase.Services;
public class DefaultActionExpressionProvider : IActionExpressionProvider
{
    public string Name => "Default";

    public async Task PlayActionExpressionAsync(string actionName)
    {

    }

    public async Task PlayActionExpressionAsync(string actionName, List<ElectronBotAction> actions)
    {

    }

    public async Task PlayActionExpressionAsync(EmoticonAction emoticonAction, List<ElectronBotAction> actions)
    {

    }

    public async Task PlayActionExpressionAsync(EmoticonAction emoticonAction)
    {

    }
}
