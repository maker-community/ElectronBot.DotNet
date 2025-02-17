using BotSharp.Abstraction.MLTasks.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class VoiceSettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    public VoiceSettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
    }

    [ObservableProperty]
    private LlmModelSetting _llmModelSetting;

    [RelayCommand]
    private Task OnSaveLlmModelSettingAsync()
    {
        return Task.CompletedTask;
    }
}
