using Verdure.Braincase.Settings.Models;

namespace Verdure.Braincase.Settings.Services;
public class SettingDataInitService : IDataInitService
{
    private readonly ILocalSettingsService _localSettingsService;
    public SettingDataInitService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }
    public async Task InitializeDataAsync()
    {
        var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders);
        if (modelList == null)
        {
            modelList =
            [
                new CustomLlmProviderSetting
                {
                    Provider = "azure-openai",
                    Models = [ new()
                    {
                        Provider = "azure-openai",
                        Name = "gpt-4o-mini"
                    } ]
                },
            ];
            await _localSettingsService.SaveSettingAsync(Constants.LlmProviders, modelList);
        }

    }
}
