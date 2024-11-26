using Microsoft.UI.Xaml;
using Verdure.Braincase.ViewModels;

namespace Verdure.Braincase.Activation;

public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly ILocalSettingsService _localSettingsService;
    private readonly INavigationService _navigationService;

    public DefaultActivationHandler(INavigationService navigationService, ILocalSettingsService localSettingsService)
    {
        _navigationService = navigationService;
        _localSettingsService = localSettingsService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        // None of the ActivationHandlers has handled the activation.
        return _navigationService.Frame?.Content == null;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        var config = await _localSettingsService
            .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        var customConfig = config ?? new CustomClockTitleConfig();

        if (customConfig.Hw75IsOpen)
        {
            _navigationService.NavigateTo(typeof(Hw75ViewModel).FullName!, args.Arguments);

        }
        else
        {
            _navigationService.NavigateTo(typeof(HomeViewModel).FullName!, args.Arguments);

        }

        await Task.CompletedTask;
    }
}
