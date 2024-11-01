using Verdure.Braincase.ClockViews;
using Verdure.Braincase.Contracts.Services;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Services;
public class DefaultClockViewProvider : IClockViewProvider
{
    private readonly string _name = "DefautView";
    public string Name => _name;

    public UIElement CreateClockView(string viewName)
    {
        return new HiddenTextView();
    }
}
