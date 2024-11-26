using Verdure.Braincase.ClockViews;
using Verdure.Braincase.Contracts.Services;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Services;

public class GrooveClockViewProvider : IClockViewProvider
{
    private readonly string _name = "GrooveView";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new GrooveClockView();
    }
}

