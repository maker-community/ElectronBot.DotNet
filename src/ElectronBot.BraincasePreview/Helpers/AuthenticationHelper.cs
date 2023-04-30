using Verdure.ElectronBot.Core.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace ElectronBot.Braincase.Helpers;

internal static class AuthenticationHelper
{
    internal static async Task ShowLoginErrorAsync(LoginResultType loginResult)
    {
        switch (loginResult)
        {
            case LoginResultType.NoNetworkAvailable:
                await new ContentDialog()
                {
                    XamlRoot = App.MainWindow.Content.XamlRoot,
                    Content = "DialogNoNetworkAvailableContent".GetLocalized(),
                    Title = "DialogAuthenticationTitle".GetLocalized()
                }.ShowAsync();
                break;
            case LoginResultType.UnknownError:
                await new ContentDialog()
                {
                    XamlRoot = App.MainWindow.Content.XamlRoot,
                    Content = "DialogStatusUnknownErrorContent".GetLocalized(),
                    Title = "DialogAuthenticationTitle".GetLocalized()
                }.ShowAsync();
                break;
        }
    }
}
