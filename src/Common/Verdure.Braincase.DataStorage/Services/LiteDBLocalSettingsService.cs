using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Helpers;
using Verdure.Braincase.DataStorage.Collections;

namespace Verdure.Braincase.DataStorage.Services;
public class LiteDBLocalSettingsService : ILocalSettingsService
{
    private readonly BraincaseLiteDBContext _db;
    public LiteDBLocalSettingsService(BraincaseLiteDBContext db)
    {
        _db = db;
    }
    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        var setting = _db.LocalSettings.FindOne(x => x.Key == key);
        if (setting == null)
        {
            return default;
        }
        return await Json.ToObjectAsync<T>(setting.Value);
    }
    public async Task SaveSettingAsync<T>(string key, T value)
    {
        var setting = _db.LocalSettings.FindOne(x => x.Key == key);

        if (setting == null)
        {
            _db.LocalSettings.Insert(new LocalSettingDocument { Key = key, Value = await Json.StringifyAsync(value) });
        }
        else
        {
            setting.Value = await Json.StringifyAsync(value);
            _db.LocalSettings.Update(setting);
        }
    }
}
