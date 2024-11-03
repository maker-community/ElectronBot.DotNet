namespace Verdure.Braincase.WinUI.Common.Services.Picker;

public interface IObjectPicker<T>
{
    event EventHandler<ObjectPickedEventArgs<T>> ObjectPicked;
    event EventHandler Canceled;
}
