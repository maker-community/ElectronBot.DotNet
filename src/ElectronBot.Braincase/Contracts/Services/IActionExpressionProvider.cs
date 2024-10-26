using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ElectronBot.Braincase.Models;
using Verdure.Braincase.Core.Models;
using Verdure.WinUI.Common.Models;

namespace ElectronBot.Braincase.Contracts.Services;
public interface IActionExpressionProvider
{
    public string Name
    {
        get;
    }
    Task PlayActionExpressionAsync(string actionName);

    Task PlayActionExpressionAsync(EmoticonAction emoticonAction);

    Task PlayActionExpressionAsync(string actionName, List<ElectronBotAction> actions);

    Task PlayActionExpressionAsync(EmoticonAction emoticonAction, List<ElectronBotAction> actions);
}
