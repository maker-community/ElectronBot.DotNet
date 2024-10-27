using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronBot.Braincase.Contracts.Services;

namespace Verdure.Braincase.DataStorage.Services;
public class LiteDBLocalSettingsService : ILocalSettingsService
{
    public Task<T?> ReadSettingAsync<T>(string key) => throw new NotImplementedException();
    public Task SaveSettingAsync<T>(string key, T value) => throw new NotImplementedException();
}
