using ClockViews;
using Verdure.Braincase.ClockViews;
using Verdure.Braincase.Contracts.Services;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Services;
public class GradientsWithBlendClockViewProvider : IClockViewProvider
{
    private readonly string _name = "GradientsWithBlend";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new GradientsWithBlend();
    }
}
