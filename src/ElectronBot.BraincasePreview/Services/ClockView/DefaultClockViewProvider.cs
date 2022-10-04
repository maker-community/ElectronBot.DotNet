using ElectronBot.BraincasePreview.ClockViews;
using ElectronBot.BraincasePreview.Contracts.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.BraincasePreview.Services;
public class DefaultClockViewProvider : IClockViewProvider
{
    private readonly string _name = "DefautView";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new HiddenTextView();
    }
}
