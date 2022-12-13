using ClockViews;
using ElectronBot.BraincasePreview.ClockViews;
using ElectronBot.BraincasePreview.Contracts.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.BraincasePreview.Services;
public class GradientsWithBlendClockViewProvider : IClockViewProvider
{
    private readonly string _name = "GradientsWithBlend";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new GradientsWithBlend();
    }
}
