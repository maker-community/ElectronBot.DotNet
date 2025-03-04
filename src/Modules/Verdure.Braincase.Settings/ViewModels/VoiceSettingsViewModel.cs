using Verdure.Braincase.Core.Configuration;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class VoiceSettingsViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;
    public VoiceSettingsViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
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


    [RelayCommand]
    public async Task OnLoadedAsync()
    {
        var model = await _localSettingsService.ReadSettingAsync<AzureCognitiveServicesOptions>(Constants.AzureLlmVoiceConfigKey);

        if (model != null)
        {
            AzureLlmVoiceSetting = model;
        }
    }

    [RelayCommand]
    private async Task OnSaveAzureLlmModelSettingAsync()
    {
        await _localSettingsService.SaveSettingAsync(Constants.AzureLlmVoiceConfigKey, AzureLlmVoiceSetting);
    }
}
