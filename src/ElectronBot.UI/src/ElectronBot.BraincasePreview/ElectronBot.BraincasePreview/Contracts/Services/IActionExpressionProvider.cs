using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.BraincasePreview.Contracts.Services;
public interface IActionExpressionProvider
{
    public string Name
    {
        get;
    }
    Task PlayActionExpressionAsync(string actionName);
}
