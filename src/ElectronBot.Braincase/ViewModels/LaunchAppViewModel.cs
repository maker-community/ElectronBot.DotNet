using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Models;
using Windows.ApplicationModel;
using Windows.Management.Deployment;


namespace ElectronBot.Braincase.ViewModels;

public partial class LaunchAppViewModel : ObservableRecipient
{
    private readonly PackageManager _packageManager = new();

    [ObservableProperty] private ObservableCollection<Package> _appPackages;

    [ObservableProperty] private string _win32Path;

    [ObservableProperty] private string _VoiceText;

    [ObservableProperty] private string _appNameText;

    [ObservableProperty] private bool _IsMsix = true;

    [ObservableProperty] private Package? _selectPackage;

    private readonly ILocalSettingsService _localSettingsService;

    public LaunchAppViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public void ComboBox_OnTextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
    {
        if (args.Text is not null)
        {
            var text = args.Text;
            var apps = _packageManager.FindPackagesForUser(string.Empty)
                .Where(p => p.IsFramework == false && p.DisplayName.Contains(text)).ToList();

            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                AppPackages = new ObservableCollection<Package>(apps);
                SelectPackage = apps.FirstOrDefault();
            });
        }
        else
        {
            Task.Run(() =>
            {
                var apps = _packageManager.FindPackagesForUser(string.Empty)
                    .Where(p => p.IsFramework == false && !string.IsNullOrEmpty(p.DisplayName)).ToList();
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
               {
                   AppPackages = new ObservableCollection<Package>(apps);
                   SelectPackage = apps.FirstOrDefault();
               });
            });
        }
    }

    public void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var radioButtons = (RadioButtons)sender;

        if (radioButtons.SelectedIndex == 0)
        {
            Win32PathVisibility = Visibility.Collapsed;
            MsixVisibility = Visibility.Visible;
            IsMsix = true;
            Task.Run(() =>
            {
                var apps = _packageManager.FindPackagesForUser(string.Empty)
                    .Where(p => p.IsFramework == false && !string.IsNullOrEmpty(p.DisplayName)).ToList();
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    AppPackages = new ObservableCollection<Package>(apps);
                    SelectPackage = apps.FirstOrDefault();
                });
            });
        }
        else
        {
            Win32PathVisibility = Visibility.Visible;
            MsixVisibility = Visibility.Collapsed;
            IsMsix = false;
        }
    }


    [ObservableProperty] private Visibility _win32PathVisibility = Visibility.Collapsed;

    [ObservableProperty] private Visibility _msixVisibility = Visibility.Visible;

    [RelayCommand]
    private async Task AddWin32AppPath()
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
        };

        picker.FileTypeFilter.Add(".exe");

        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file is null)
        {
            return;
        }
        var propList = await file.GetBasicPropertiesAsync();

        Win32Path = file.Path;
    }

    public void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb)
        {
            if (cb.SelectedItem is Package pa)
            {
                SelectPackage = pa;
                AppNameText = SelectPackage!.DisplayName;
            }
        }
    }

    //[RelayCommand]
    public async Task SaveLaunchApp()
    {
        var launchAppConfigs = (await _localSettingsService.ReadSettingAsync<List<LaunchAppConfig>>
       (Constants.LaunchAppConfigKey)) ?? new List<LaunchAppConfig>();

        var launchAppConfig = new LaunchAppConfig
        {
            VoiceText = VoiceText,
            Win32Path = Win32Path,
            AppNameText = AppNameText,
            IsMsix = IsMsix
        };
        launchAppConfigs.Add(launchAppConfig);

        await _localSettingsService.SaveSettingAsync<List<LaunchAppConfig>>(Constants.LaunchAppConfigKey, launchAppConfigs);
    }
}
