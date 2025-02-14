using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class DialogueSettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    

    public DialogueSettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        UpdateOpenAiLogo();
    }
    [ObservableProperty]
    private string _openAiLogo;

    private void UpdateOpenAiLogo()
    {
        switch (_themeSelectorService.Theme)
        {
            case ElementTheme.Dark:
                OpenAiLogo = "ms-appx:///Assets/Providers/OpenAI-black-monoblossom.svg";
                break;
            case ElementTheme.Light:
                OpenAiLogo = "ms-appx:///Assets/Providers/OpenAI-white-monoblossom.svg";
                break;
            default:
                OpenAiLogo = "ms-appx:///Assets/Providers/OpenAI-black-monoblossom.svg";
                break;
        }
    }
}
