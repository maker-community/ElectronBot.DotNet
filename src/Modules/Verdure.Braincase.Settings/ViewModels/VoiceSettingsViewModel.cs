using Verdure.Braincase.Core.Configuration;
using Verdure.Braincase.Services;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class VoiceSettingsViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;
    public VoiceSettingsViewModel(ILocalSettingsService localSettingsService, ComboxDataService comboxDataService)
    {
        _localSettingsService = localSettingsService;
        _llmVoiceComboxModels = comboxDataService.GetLlmVoiceComboxList();
    }

    [ObservableProperty]
    private AzureCognitiveServicesOptions _azureLlmVoiceSetting = new()
    {
        Region = "eastus",
        SpeechRecognitionLanguage = "zh-CN",
        SpeechSynthesisVoiceName = "zh-CN-XiaoyiNeural",
        EnableSpeechStyle = false,
        Rate = "+15%",
        WakePhraseModel = "keyword_cortana.table"
    };

    /// <summary>
    /// 语音服务列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> _llmVoiceComboxModels;

    /// <summary>
    /// 聊天机器人选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel _llmVoiceSelect;


    [RelayCommand]
    public async Task OnLoadedAsync()
    {
        var model = await _localSettingsService.ReadSettingAsync<AzureCognitiveServicesOptions>(Constants.AzureLlmVoiceConfigKey);

        if (model != null)
        {
            AzureLlmVoiceSetting = model;
        }


        var llmVoiceModel = await _localSettingsService
            .ReadSettingAsync<ComboxItemModel>(Constants.DefaultLlmVoiceNameKey);

        if (llmVoiceModel != null)
        {
            LlmVoiceSelect = LlmVoiceComboxModels.FirstOrDefault(c => c.DataValue == llmVoiceModel.DataValue);
        }
    }

    [RelayCommand]
    private async Task OnSaveAzureLlmModelSettingAsync()
    {
        await _localSettingsService.SaveSettingAsync(Constants.AzureLlmVoiceConfigKey, AzureLlmVoiceSetting);
    }

    [RelayCommand]
    public async Task LlmVoiceChangedAsync()
    {
        var llmVoiceName = LlmVoiceSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(llmVoiceName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultLlmVoiceNameKey, LlmVoiceSelect);
        }
    }
}
