using BotSharp.Abstraction.MLTasks.Settings;
using Verdure.Braincase.Settings.Models;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class DialogueSettingsViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;
    public DialogueSettingsViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    [ObservableProperty]
    private CustomLlmModelSetting _openAILlmModelSetting = new()
    {
        Provider = "openai",
        Endpoint = "https://api.openai.com/v1",
        Name = "gpt-4o-mini"
    };

    [ObservableProperty]
    private CustomLlmModelSetting _azureOpenAILlmModelSetting = new()
    {
        Provider = "azure-openai",
        Name = "gpt-4o-mini"
    };

    [ObservableProperty]
    private CustomLlmModelSetting _deepSeekILlmModelSetting = new()
    {
        Provider = "deepseek-ai",
        Endpoint = "https://api.deepseek.com/v1",
        Name = "deepseek-chat"
    };

    [ObservableProperty]
    private CustomLlmModelSetting _tongyiILlmModelSetting = new()
    {
        Provider = "tongyi",
        Endpoint = "https://dashscope.aliyuncs.com/compatible-mode/v1",
        Name = "qwen2.5-72b-instruct"
    };

    [RelayCommand]
    public async Task OnLoadedAsync()
    {
        var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders);

        if (modelList != null)
        {
            var azureOpenAILlmModelSetting =
                modelList.FirstOrDefault(x => x.Provider == "azure-openai")?.Models.FirstOrDefault();
            if (azureOpenAILlmModelSetting != null)
            {
                AzureOpenAILlmModelSetting = azureOpenAILlmModelSetting;
            }

            var openAILlmModelSetting =
                modelList.FirstOrDefault(x => x.Provider == "openai")?.Models.FirstOrDefault(x => x.Name.StartsWith("gpt"));
            if (openAILlmModelSetting != null)
            {
                OpenAILlmModelSetting = openAILlmModelSetting;
            }

            var deepSeekILlmModelSetting =
                modelList.FirstOrDefault(x => x.Provider == "deepseek-ai")?.Models.FirstOrDefault();
            if (deepSeekILlmModelSetting != null)
            {
                DeepSeekILlmModelSetting = deepSeekILlmModelSetting;
            }

            var tongyiILlmModelSetting =
                modelList.FirstOrDefault(x => x.Provider == "openai")?.Models.FirstOrDefault(x => x.Name.StartsWith("qwen"));
            if (tongyiILlmModelSetting != null)
            {
                TongyiILlmModelSetting = tongyiILlmModelSetting;
            }
        }
    }

    [RelayCommand]
    private async Task OnSaveLlmModelSettingAsync()
    {
        AzureOpenAILlmModelSetting.Provider = "azure-openai";
        OpenAILlmModelSetting.Provider = "openai";
        TongyiILlmModelSetting.Provider = "tongyi";
        DeepSeekILlmModelSetting.Provider = "deepseek-ai";

        var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders) ?? new List<CustomLlmProviderSetting>();

        var azureProvider = modelList.FirstOrDefault(x => x.Provider == "azure-openai");
        if (azureProvider != null)
        {
            azureProvider.Models = new List<CustomLlmModelSetting> { AzureOpenAILlmModelSetting };
        }
        else
        {
            modelList.Add(new CustomLlmProviderSetting
            {
                Provider = "azure-openai",
                Models = new List<CustomLlmModelSetting> { AzureOpenAILlmModelSetting }
            });
        }
        var openaiProvider = modelList.FirstOrDefault(x => x.Provider == "openai");
        if (openaiProvider != null)
        {
            var nonImageModels = openaiProvider.Models.Where(m => m.Type != LlmModelType.Image).ToList();
            nonImageModels.Clear();
            nonImageModels.Add(OpenAILlmModelSetting);
            nonImageModels.Add(TongyiILlmModelSetting);
            var imageModels = openaiProvider.Models.Where(m => m.Type == LlmModelType.Image).ToList();
            openaiProvider.Models = nonImageModels.Concat(imageModels).ToList();
        }
        else
        {
            modelList.Add(new CustomLlmProviderSetting
            {
                Provider = "openai",
                Models = new List<CustomLlmModelSetting> { OpenAILlmModelSetting, TongyiILlmModelSetting }
            });
        }

        var deepseekProvider = modelList.FirstOrDefault(x => x.Provider == "deepseek-ai");
        if (deepseekProvider != null)
        {
            deepseekProvider.Models = new List<CustomLlmModelSetting> { DeepSeekILlmModelSetting };
        }
        else
        {
            modelList.Add(new CustomLlmProviderSetting
            {
                Provider = "deepseek-ai",
                Models = new List<CustomLlmModelSetting> { DeepSeekILlmModelSetting }
            });
        }

        await _localSettingsService.SaveSettingAsync(Constants.LlmProviders, modelList);
    }
}
