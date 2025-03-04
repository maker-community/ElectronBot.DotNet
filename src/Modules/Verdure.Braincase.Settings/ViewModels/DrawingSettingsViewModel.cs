using BotSharp.Abstraction.MLTasks.Settings;
using Verdure.Braincase.Settings.Models;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class DrawingSettingsViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;
    public DrawingSettingsViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    [ObservableProperty]
    private CustomLlmModelSetting _tongyiLlmModelSetting = new()
    {
        Type = LlmModelType.Image,
        Provider = "tongyi",
        Endpoint = "https://dashscope.aliyuncs.com/api/v1",
        Name = "wanx-v1"
    };

    [RelayCommand]
    public async Task OnLoadedAsync()
    {
        var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders);

        if (modelList != null)
        {
            var openAiProvider = modelList.FirstOrDefault(m => m.Provider == "openai");
            if (openAiProvider != null)
            {
                var imageModel = openAiProvider.Models.FirstOrDefault(m => m.Type == LlmModelType.Image);
                if (imageModel != null)
                {
                    TongyiLlmModelSetting = imageModel;
                }
            }
        }
    }

    [RelayCommand]
    private async Task OnSaveLlmModelSettingAsync()
    {
        TongyiLlmModelSetting.Provider = "tongyi";
        var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders);
        if (modelList != null)
        {
            var openAiProvider = modelList.FirstOrDefault(m => m.Provider == "openai");
            if (openAiProvider != null)
            {
                var existingModel = openAiProvider.Models.FirstOrDefault(m => m.Type == LlmModelType.Image);
                if (existingModel != null)
                {
                    existingModel.Endpoint = TongyiLlmModelSetting.Endpoint;
                    existingModel.Name = TongyiLlmModelSetting.Name;
                    existingModel.ApiKey = TongyiLlmModelSetting.ApiKey;
                }
                else
                {
                    openAiProvider.Models.Add(TongyiLlmModelSetting);
                }
            }
            else
            {
                modelList.Add(new CustomLlmProviderSetting
                {
                    Provider = "openai",
                    Models = new List<CustomLlmModelSetting> { TongyiLlmModelSetting }
                });
            }
        }
        else
        {
            modelList = new List<CustomLlmProviderSetting>
            {
                new()
                {
                    Provider = "openai",
                    Models = new List<CustomLlmModelSetting> { TongyiLlmModelSetting }
                }
            };
        }
        await _localSettingsService.SaveSettingAsync(Constants.LlmProviders, modelList);
    }
}
