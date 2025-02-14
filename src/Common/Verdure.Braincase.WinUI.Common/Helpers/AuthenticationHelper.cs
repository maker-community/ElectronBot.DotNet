using Verdure.Braincase.Core.Helpers;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.WinUI.Common.Helpers;
using Verdure.Braincase.WinUI.Common.Contracts.Services;

namespace Verdure.Braincase.Helpers;

public static class AuthenticationHelper
{
    public static async Task ShowLoginErrorAsync(LoginResultType loginResult)
    {
        var window = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow();
        switch (loginResult)
        {
            case LoginResultType.NoNetworkAvailable:
                await new ContentDialog()
                {
                    XamlRoot = window.Content.XamlRoot,
                    Content = "DialogNoNetworkAvailableContent".GetLocalized(),
                    Title = "DialogAuthenticationTitle".GetLocalized()
                }.ShowAsync();
                break;
            case LoginResultType.UnknownError:
                await new ContentDialog()
                {
                    XamlRoot = window.Content.XamlRoot,
                    Content = "DialogStatusUnknownErrorContent".GetLocalized(),
                    Title = "DialogAuthenticationTitle".GetLocalized()
                }.ShowAsync();
                break;
        }
    }
}
