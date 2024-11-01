using Controls;
using Verdure.Braincase.Contracts.Services;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.Services;
using HidApi;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.WinUI.Common.Helpers;

namespace Verdure.Braincase;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static Frame RootFrame
    {
        get; set;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar
    {
        get; set;
    }

    public App()
    {
        InitializeComponent();



        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
           services.AddServices(context.Configuration);
        }).
        Build();

        Ioc.Default.GetRequiredService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {

        e.Handled = true;

        ElectronBotHelper.Instance.InvokeClockCanvasStop();

        var service = Ioc.Default.GetRequiredService<EmoticonActionFrameService>();

        service.ClearQueue();

        Thread.Sleep(1000);

        ElectronBotHelper.Instance?.ElectronBot?.Disconnect();

        Hw75Helper.Instance.Hw75DynamicDevice?.Close();
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        MainWindow.AppWindow.Closing += AppWindow_Closing;
        //Ioc.Default.GetRequiredService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await Ioc.Default.GetRequiredService<IActivationService>().ActivateAsync(args);
    }

    private async void AppWindow_Closing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        args.Cancel = true;

        var theme = Ioc.Default.GetRequiredService<IThemeSelectorService>();

        var dialog = new ContentDialog
        {
            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            XamlRoot = MainWindow.Content.XamlRoot,
            Title = "ExitBtn_Title".GetLocalized(),
            PrimaryButtonText = "ExitBtn_PrimaryButtonText".GetLocalized(),
            CloseButtonText = "ExitBtn_CloseButtonText".GetLocalized(),
            SecondaryButtonText = "ExitBtn_SecondaryButtonText".GetLocalized(),
            DefaultButton = ContentDialogButton.Primary,
            RequestedTheme = theme.Theme,
            Content = new AppQuitPage()
        };

        var result = await dialog.ShowAsync();


        if (result == ContentDialogResult.Primary)
        {
            try
            {
                ElectronBotHelper.Instance.InvokeClockCanvasStop();

                var service = Ioc.Default.GetRequiredService<EmoticonActionFrameService>();

                service.ClearQueue();

                Thread.Sleep(1000);

                IntelligenceService.Current.CleanUp();

                await CameraService.Current.CleanupMediaCaptureAsync();

                await CameraFrameService.Current.CleanupMediaCaptureAsync();

                ElectronBotHelper.Instance?.ElectronBot?.Disconnect();

                Hw75Helper.Instance.Hw75DynamicDevice?.Close();
            }
            catch (Exception)
            {

            }

            MainWindow.Close();
            Hid.Exit();
        }
        else if (result == ContentDialogResult.Secondary)
        {
            MainWindow.Hide();
        }
    }
}
