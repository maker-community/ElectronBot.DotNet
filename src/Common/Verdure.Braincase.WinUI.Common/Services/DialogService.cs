using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.WinUI.Common.Contracts.Services;

namespace Verdure.Braincase.WinUI.Common.Services;
public class DialogService : IDialogService
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ICompositorProvider _compositorProvider;
    public DialogService(IThemeSelectorService themeSelectorService, ICompositorProvider compositorProvider)
    {
        _themeSelectorService = themeSelectorService;
        _compositorProvider = compositorProvider;
    }
    public async Task<ContentDialogResult> ShowDialogAsync(string title, string primaryButtonText, string closeButtonText, object content)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            PrimaryButtonText = primaryButtonText,
            CloseButtonText = closeButtonText,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = _compositorProvider.GetWindow().Content.XamlRoot,
            Content = content,
            RequestedTheme = _themeSelectorService.Theme
        };

        return await dialog.ShowAsync();
    }
}