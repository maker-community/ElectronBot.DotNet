using Verdure.Braincase.ClockViews;
using Verdure.Braincase.Contracts.Services;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Services;

public class CustomClockViewProvider : IClockViewProvider
{
    private readonly string _name = "CustomView";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new CustomClockView();
    }
}

