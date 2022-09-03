using ElectronBot.BraincasePreview.ClockViews;
using ElectronBot.BraincasePreview.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Core;

namespace ElectronBot.BraincasePreview.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();

        InitializeComponent();
    }

    private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        HandleInitializationError();
    }
    private void HandleInitializationError()
    {
        var instance = AppInstance.GetCurrent();

        if(instance != null)
        {
            // Restart in safe mode to avoid whatever made initialization fail
            AppRestartFailureReason reason = AppInstance.Restart("/safemode");
            switch (reason)
            {
                case AppRestartFailureReason.RestartPending:
                    ////Telemetry.WriteLine("Another restart is currently pending.");
                    break;
                case AppRestartFailureReason.InvalidUser:
                    ///Telemetry.WriteLine("Current user is not signed in or not a valid user.");
                    break;
                case AppRestartFailureReason.Other:
                    //Telemetry.WriteLine("Failure restarting.");
                    break;
            }
        }   
    }
}
