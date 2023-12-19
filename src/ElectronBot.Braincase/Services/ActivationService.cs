using ElectronBot.Braincase.Activation;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ElectronBot.Braincase.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private UIElement? _shell = null;

    private readonly UserDataService _userDataService;

    private readonly IdentityService _identityService;

    private readonly ILocalSettingsService _localSettingsService;


    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
        IEnumerable<IActivationHandler> activationHandlers,
        IThemeSelectorService themeSelectorService,
        UserDataService userDataService,
        IdentityService identityService,
        ILocalSettingsService localSettingsService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
        _userDataService = userDataService;
        _identityService = identityService;
        _localSettingsService = localSettingsService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        _userDataService.Initialize();
        _identityService.InitializeWithAadAndPersonalMsAccounts();
        await _identityService.AttachTokenCacheAsync();
        await _identityService.AcquireTokenSilentAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            var config = await _localSettingsService
              .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

            var customConfig = config ?? new CustomClockTitleConfig();

            if (customConfig.Hw75IsOpen)
            {
                _shell = App.GetService<Hw75ShellPage>();
                App.MainWindow.Content = _shell ?? new Frame();
            }
            else
            {
                _shell = App.GetService<ShellPage>();
                App.MainWindow.Content = _shell ?? new Frame();
            }
        }

        // Activate the MainWindow.
        App.MainWindow.Activate();

        // Handle activation via ActivationHandlers.
        await HandleActivationAsync(activationArgs);


        // Execute tasks after activation.
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await _themeSelectorService.InitializeAsync().ConfigureAwait(false);
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await _themeSelectorService.SetRequestedThemeAsync();
        await Task.CompletedTask;
    }
}
