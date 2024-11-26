using Microsoft.UI.Xaml;

namespace Verdure.Braincase.WinUI.Common.Contracts.Services;
public interface IClockViewProvider
{
    public string Name
    {
        get;
    }
    UIElement CreateClockView(string viewName);
}
