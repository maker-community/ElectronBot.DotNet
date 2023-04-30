using ElectronBot.Braincase.ClockViews;
using ElectronBot.Braincase.Contracts.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.Braincase.Services;
public class GooeyFooterClockViewProvider : IClockViewProvider
{
    private readonly string _name = "GooeyFooter";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new GooeyFooter();
    }
}
