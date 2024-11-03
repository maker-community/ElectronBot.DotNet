using Verdure.Braincase.WinUI.Common.Services.Picker;

namespace Verdure.Braincase.WinUI.Common.Services;
public class ObjectPickerService
{
    private readonly Dictionary<string, Dictionary<string, Type>> _pages =
        new();
    public async Task<PickResult<T>> PickSingleObjectAsync<T>(string pageKey, object parameter = null,
        PickerOpenOption startOption = null)
    {
        var page = Type.GetType(pageKey);

        var picker = Ioc.Default.GetRequiredService<ObjectPicker<T>>();

        //var picker = new ObjectPicker<T>(service);

        if (startOption != null) picker.PickerOpenOption = startOption;

        var result = await picker.PickSingleObjectAsync(page!, parameter);
        return result;
    }
}
