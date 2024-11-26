using Microsoft.UI.Xaml;

namespace Verdure.Braincase.HelloWordKeyboard.Contracts.Services;
public interface IHw75DynamicViewProvider
{
    public string Name
    {
        get;
    }
    UIElement CreateHw75DynamickView(string viewName);
}
