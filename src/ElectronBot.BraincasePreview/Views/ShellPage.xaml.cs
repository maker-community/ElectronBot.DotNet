using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Helpers;
using ElectronBot.BraincasePreview.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Verdure.NotificationArea;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.System;
using Windows.UI.Popups;

namespace ElectronBot.BraincasePreview.Views;

// TODO: Update NavigationViewItem titles and icons in ShellPage.xaml.
public sealed partial class ShellPage : Page
{
    public NotificationAreaIcon NotificationAreaIcon { get; set; } = new NotificationAreaIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets/pig.ico"), "AppDisplayName".GetLocalized());
    public ShellViewModel ViewModel
    {
        get;
    }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();
        InitializeNotificationAreaIcon();
        App.RootFrame = NavigationFrame;
    }

    private async void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
        ViewModel.Initialize();
        await ElectronBotHelper.Instance.InitAsync();

        //await RegisterTaskAysnc();
    }


    private void InitializeNotificationAreaIcon()
    {
        NotificationAreaIcon.InitializeNotificationAreaMenu();
        NotificationAreaIcon.AddMenuItemText(1, "显示或者隐藏");
        //NotificationAreaIcon.AddMenuItemText(2, "设置");
        NotificationAreaIcon.AddMenuItemSeperator();
        NotificationAreaIcon.AddMenuItemText(3, "退出");

        NotificationAreaIcon.DoubleClick = () =>
        {
            DispatcherQueue.TryEnqueue(() => { ViewModel.ShowOrHideWindowCommand.Execute(null); });
        };
        NotificationAreaIcon.RightClick = () =>
        {
            DispatcherQueue.TryEnqueue(() => { NotificationAreaIcon.ShowContextMenu(); });
        };
        NotificationAreaIcon.MenuCommand = (menuid) =>
        {
            switch (menuid)
            {
                case 1:
                    {
                        DispatcherQueue.TryEnqueue(() => { ViewModel.ShowOrHideWindowCommand.Execute(null); });
                        break;
                    }
                case 2:
                    {
                        DispatcherQueue.TryEnqueue(() => { ViewModel.SettingsCommand.Execute(null); });
                        break;
                    }
                case 3:
                    {
                        DispatcherQueue.TryEnqueue(() => { ViewModel.ExitCommand.Execute(null); });
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        };
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

        AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }

    async Task RegisterTaskAysnc()
    {

        var taskRegistered = false;
        var exampleTaskName = "EbToastBgTask";
        taskRegistered = BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name == exampleTaskName);


        if (!taskRegistered)
        {
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if (access == BackgroundAccessStatus.DeniedBySystemPolicy)
            {
                await new MessageDialog("后台任务已经被禁止了").ShowAsync();
            }
            else
            {
                var builder = new BackgroundTaskBuilder
                {
                    Name = "EbToastBgTask",
                    TaskEntryPoint = "ElectronBot.BraincasePreview.BgTaskComponent.ToastBgTask"
                };
                builder.SetTrigger(new TimeTrigger(15, false));

                var task = builder.Register();
            }

        }
        else
        {
            var cur = BackgroundTaskRegistration.AllTasks.FirstOrDefault(x => x.Value.Name == exampleTaskName);
            BackgroundTaskRegistration task = (BackgroundTaskRegistration)(cur.Value);
            //    task.Completed += task_Completed;
        }
    }
}
