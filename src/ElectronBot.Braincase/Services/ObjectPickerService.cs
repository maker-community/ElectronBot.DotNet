using ElectronBot.Braincase.Picker;

namespace ElectronBot.Braincase.Services;
public class ObjectPickerService
{
    private readonly Dictionary<string, Dictionary<string, Type>> _pages =
        new();
    public async Task<PickResult<T>> PickSingleObjectAsync<T>(string pageKey, object parameter = null,
        PickerOpenOption startOption = null)
    {
        Type? page = Type.GetType(pageKey);

        var picker = App.GetService<ObjectPicker<T>>();

        //var picker = new ObjectPicker<T>(service);

        if (startOption != null) picker.PickerOpenOption = startOption;

        var result = await picker.PickSingleObjectAsync(page!, parameter);
        return result;
    }
}
