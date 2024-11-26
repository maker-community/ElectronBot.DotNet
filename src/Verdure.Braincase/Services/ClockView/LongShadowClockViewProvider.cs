using Verdure.Braincase.ClockViews;
using Verdure.Braincase.Contracts.Services;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Services;
public class LongShadowClockViewProvider : IClockViewProvider
{
    private readonly string _name = "LongShadowView";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new LongShadow();
    }
}
