using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ElectronBot.BraincasePreview.Models;

namespace ElectronBot.BraincasePreview.Contracts.Services;
public interface IActionExpressionProvider
{
    public string Name
    {
        get;
    }
    Task PlayActionExpressionAsync(string actionName);

    Task PlayActionExpressionAsync(string actionName, List<ElectronBotAction> actions);
}
