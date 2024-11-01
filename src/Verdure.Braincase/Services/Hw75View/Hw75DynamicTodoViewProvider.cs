using Verdure.Braincase.ClockViews;
using Verdure.Braincase.Contracts.Services;
using Hw75Views;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Services;
public class Hw75DynamicTodoViewProvider : IHw75DynamicViewProvider
{
    private readonly string _name = "TodoView";
    public string Name => _name;

    public UIElement CreateHw75DynamickView(string viewName)
    {
        return Ioc.Default.GetRequiredService<TodoView>();
    }
}
