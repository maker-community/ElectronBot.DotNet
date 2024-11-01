using Verdure.Braincase.Picker;

namespace Verdure.Braincase.Services;
public class ObjectPickerService
{
    private readonly Dictionary<string, Dictionary<string, Type>> _pages =
        new();
    public async Task<PickResult<T>> PickSingleObjectAsync<T>(string pageKey, object parameter = null,
        PickerOpenOption startOption = null)
    {
        Type? page = Type.GetType(pageKey);

        var picker = Ioc.Default.GetRequiredService<ObjectPicker<T>>();

        //var picker = new ObjectPicker<T>(service);

        if (startOption != null) picker.PickerOpenOption = startOption;

        var result = await picker.PickSingleObjectAsync(page!, parameter);
        return result;
    }
}
