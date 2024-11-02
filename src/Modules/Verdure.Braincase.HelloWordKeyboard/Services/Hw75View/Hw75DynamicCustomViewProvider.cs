namespace Verdure.Braincase.HelloWordKeyboard.Services.Hw75View;
public class Hw75DynamicCustomViewProvider : IHw75DynamicViewProvider
{
    private readonly string _name = "Hw75CustomView";
    public string Name => _name;

    public UIElement CreateHw75DynamickView(string viewName)
    {
        return Ioc.Default.GetRequiredService<Hw75CustomView>();
    }
}
