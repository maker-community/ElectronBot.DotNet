namespace Verdure.Braincase.HelloWordKeyboard.Services.Hw75View;
public class Hw75DynamicTodoViewProvider : IHw75DynamicViewProvider
{
    private readonly string _name = "TodoView";
    public string Name => _name;

    public UIElement CreateHw75DynamickView(string viewName)
    {
        return Ioc.Default.GetRequiredService<TodoView>();
    }
}
