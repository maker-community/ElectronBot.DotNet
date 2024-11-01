using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Verdure.Braincase.Models;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.WinUI.Common.Models;

namespace Verdure.Braincase.Contracts.Services;
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
