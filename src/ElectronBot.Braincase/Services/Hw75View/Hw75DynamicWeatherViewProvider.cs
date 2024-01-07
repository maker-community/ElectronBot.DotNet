using ElectronBot.Braincase.ClockViews;
using ElectronBot.Braincase.Contracts.Services;
using Hw75Views;
using Microsoft.UI.Xaml;

namespace ElectronBot.Braincase.Services;
public class Hw75DynamicWeatherViewProvider : IHw75DynamicViewProvider
{
    private readonly string _name = "Hw75WeatherView";
    public string Name => _name;

    public UIElement CreateHw75DynamickView(string viewName)
    {
        return App.GetService<Hw75WeatherView>();
    }
}
