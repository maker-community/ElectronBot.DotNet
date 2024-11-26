namespace Verdure.Braincase.WinUI.Common.Services.Picker;

public class PickResult<T>
{
    public bool Canceled
    {
        get; set;
    }
    public T Result
    {
        get; set;
    }
}
