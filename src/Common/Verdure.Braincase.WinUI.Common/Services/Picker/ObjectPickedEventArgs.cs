namespace Verdure.Braincase.WinUI.Common.Services.Picker;

public class ObjectPickedEventArgs<T> : EventArgs
{
    public T Result
    {
        get; set;
    }

    public ObjectPickedEventArgs(T result)
    {
        Result = result;
    }
}
