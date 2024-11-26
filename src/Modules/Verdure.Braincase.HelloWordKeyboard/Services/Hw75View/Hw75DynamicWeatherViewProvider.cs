namespace Verdure.Braincase.HelloWordKeyboard.Services.Hw75View;
public class Hw75DynamicWeatherViewProvider : IHw75DynamicViewProvider
{
    private readonly string _name = "Hw75WeatherView";
    public string Name => _name;

    public UIElement CreateHw75DynamickView(string viewName)
    {
        return Ioc.Default.GetRequiredService<Hw75WeatherView>();
    }
}
