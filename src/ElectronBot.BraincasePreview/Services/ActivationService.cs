using ElectronBot.BraincasePreview.Activation;
using ElectronBot.BraincasePreview.Contracts.Services;
using Verdure.ElectronBot.Core.Services;
using ElectronBot.BraincasePreview.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ElectronBot.BraincasePreview.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IThemeSelectorService _themeSelectorService;
    private UIElement? _shell = null;

    private readonly UserDataService _userDataService;

    private readonly IdentityService _identityService;


    public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, 
        IEnumerable<IActivationHandler> activationHandlers, 
        IThemeSelectorService themeSelectorService, 
        UserDataService userDataService, 
        IdentityService identityService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _themeSelectorService = themeSelectorService;
        _userDataService = userDataService;
        _identityService = identityService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        // Execute tasks before activation.
        await InitializeAsync();

        _userDataService.Initialize();
        _identityService.InitializeWithAadAndPersonalMsAccounts();
        await _identityService.AcquireTokenSilentAsync();

        // Set the MainWindow Content.
        if (App.MainWindow.Content == null)
        {
            _shell = App.GetService<ShellPage>();
            App.MainWindow.Content = _shell ?? new Frame();
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
