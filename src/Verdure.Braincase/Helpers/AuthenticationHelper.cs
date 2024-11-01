using Verdure.Braincase.Core.Helpers;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.WinUI.Common.Helpers;

namespace Verdure.Braincase.Helpers;

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
