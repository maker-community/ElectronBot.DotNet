using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.MLTasks.Settings;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Utilities;
using Verdure.Braincase.Settings.Models;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class DialogueSettingsViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;
    public DialogueSettingsViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        _chatBotComboxModels = GetChatBotClientComboxList();
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

    /// <summary>
    /// 聊天机器人选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel chatBotSelect;

    /// <summary>
    /// 聊天机器人列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> _chatBotComboxModels;

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

        var chatBotModel = await _localSettingsService
             .ReadSettingAsync<ComboxItemModel>(Constants.DefaultChatBotNameKey);

        if (chatBotModel != null)
        {
            ChatBotSelect = ChatBotComboxModels.FirstOrDefault(c => c.DataValue == chatBotModel.DataValue);
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

    [RelayCommand]
    public async Task ChatBotChangedAsync()
    {
        var chatBotName = ChatBotSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(chatBotName))
        {
            var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders);

            if (modelList != null)
            {
                CustomLlmModelSetting model = null;
                var models = modelList.FirstOrDefault(m => m.Provider == chatBotName);
                if (models != null)
                {
                    model = models.Models.FirstOrDefault();
                }
                else
                {
                    models = modelList.Where(m => m.Provider == "openai").FirstOrDefault();
                    if (models != null)
                    {
                        model = models.Models.FirstOrDefault(m => m.Provider == chatBotName);
                    }
                }
                if (model != null && !string.IsNullOrEmpty(model.ApiKey))
                {
                    var agentService = Ioc.Default.GetRequiredService<IAgentService>();

                    var agents = (await agentService.GetAgents(new AgentFilter
                    {
                        Pager = new Pagination
                        {
                            Page = 1,
                            Size = 100
                        }
                    })).Items.ToList();

                    foreach (var agent in agents)
                    {
                        agent.LlmConfig.Provider = model.Provider
                            .Replace("tongyi", "openai");

                        agent.LlmConfig.Model = model.Name;

                        await agentService.UpdateAgent(agent, AgentField.LlmConfig);
                    }
                    await _localSettingsService.SaveSettingAsync(Constants.DefaultChatBotNameKey, ChatBotSelect);
                    ToastHelper.SendToast("Save Ok", TimeSpan.FromSeconds(3));
                }
                else
                {
                    ChatBotSelect = null;
                    ToastHelper.SendToast("ApiKey is null", TimeSpan.FromSeconds(3));
                }
            }
        }
    }

    private ObservableCollection<ComboxItemModel> GetChatBotClientComboxList()
    {
        return new ObservableCollection<ComboxItemModel>
            {

                new() { DataKey = "azure-openai", DataValue = "AzureOpenai" },
                new() { DataKey = "openai", DataValue = "Openai" },
                new() { DataKey = "deepseek-ai", DataValue ="DeepseekAi" },
                new() { DataKey = "tongyi", DataValue ="Tongyi" }
            };
    }
}
