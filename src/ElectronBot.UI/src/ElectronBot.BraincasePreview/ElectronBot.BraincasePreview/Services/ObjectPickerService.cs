using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Picker;

namespace ElectronBot.BraincasePreview.Services;
public class ObjectPickerService
{
    private readonly Dictionary<string, Dictionary<string, Type>> _pages =
        new Dictionary<string, Dictionary<string, Type>>();

    public void Configure(string key, string pageKey, Type pageType)
    {
        lock (_pages)
        {
            if (_pages.ContainsKey(key))
            {
                if (_pages[key].ContainsKey(pageKey))
                    throw new ArgumentException(
                        string.Format("The key {0} is already configured in ObjectPickerService", pageKey));
            }
            else
            {
                _pages.Add(key, new Dictionary<string, Type>());
            }

            if (_pages.Any(p => p.Value.ContainsKey(pageKey)))
                throw new ArgumentException(string.Format("This page is already configured with key {0}",
                    _pages.First(p => p.Value.ContainsKey(pageKey)).Key));

            _pages[key].Add(pageKey, pageType);
        }
    }

    public string GetNameOfRegisteredPage(Type page)
    {
        lock (_pages)
        {
            if (_pages.Any(p => p.Value.ContainsValue(page)))
            {
                var dic = _pages.FirstOrDefault(p => p.Value.ContainsValue(page)).Value;
                return dic.FirstOrDefault(p => p.Value == page).Key;
            }

            throw new ArgumentException(string.Format("The page '{0}' is unknown by the ObjectPickerService",
                page.Name));
        }
    }

    public async Task<PickResult<T>> PickSingleObjectAsync<T>(string pageKey, object parameter = null,
        PickerOpenOption startOption = null)
    {
        Type page;
        lock (_pages)
        {
            var typeKey = typeof(T).FullName;
            if (!_pages.TryGetValue(typeKey, out var pages))
                throw new ArgumentException(
                    string.Format("Type not found: {0}. Did you forget to call ObjectPickerService.Configure?",
                        typeKey),
                    nameof(typeKey));

            if (!pages.TryGetValue(pageKey, out page))
                throw new ArgumentException(
                    string.Format("Page not found: {0}. Did you forget to call ObjectPickerService.Configure?",
                        pageKey),
                    nameof(pageKey));
        }

        var picker = App.GetService<ObjectPicker<T>>();

        //var picker = new ObjectPicker<T>(service);

        if (startOption != null) picker.PickerOpenOption = startOption;

        var result = await picker.PickSingleObjectAsync(page, parameter);
        return result;
    }
}
