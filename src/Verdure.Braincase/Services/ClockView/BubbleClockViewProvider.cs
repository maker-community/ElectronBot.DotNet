using Microsoft.UI.Xaml;
using Verdure.Braincase.ClockViews;

namespace Verdure.Braincase.Services;

public class BubbleClockViewProvider : IClockViewProvider
{
    private readonly string _name = "BubbleView";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new Bubble();
    }
}

