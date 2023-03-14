using ElectronBot.BraincasePreview.Contracts.Services;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace ElectronBot.BraincasePreview.Activation;

public class StartupTaskActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
{
    private readonly INavigationService _navigationService;
    private readonly IAppNotificationService _notificationService;

    public StartupTaskActivationHandler(INavigationService navigationService, IAppNotificationService notificationService)
    {
        _navigationService = navigationService;
        _notificationService = notificationService;
    }

    protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
    {
        return AppInstance.GetCurrent().GetActivatedEventArgs()?.Kind == ExtendedActivationKind.StartupTask;
    }

    protected async override Task HandleInternalAsync(LaunchActivatedEventArgs args)
    {
        //App.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        //{
        //    App.MainWindow.ShowMessageDialogAsync("TODO: Handle StartupTask activations.", "StartupTask Activation");
        //});
        App.MainWindow.Hide();
        await Task.CompletedTask;
    }
}
