using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Controls;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using Windows.Management.Deployment;
using Windows.Storage;


namespace ElectronBot.Braincase.ViewModels;

public partial class LaunchAppViewModel : ObservableRecipient
{
    private readonly PackageManager _packageManager = new();

    [ObservableProperty] private ObservableCollection<Package> _appPackages;

    [ObservableProperty] private string _win32Path;

    [ObservableProperty] private string _VoiceText;

    [ObservableProperty] private Package _selectPackage;

    private readonly ILocalSettingsService _localSettingsService;

    public LaunchAppViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public void ComboBox_OnTextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
    {
        if(args.Text is not null)
        {
            var text = args.Text;
            var apps = _packageManager.FindPackagesForUser(string.Empty)
                .Where(p => p.IsFramework == false && p.DisplayName.Contains(text)).ToList();

            App.MainWindow.DispatcherQueue.TryEnqueue(() => AppPackages = new ObservableCollection<Package>(apps));
        }
        else
        {
            Task.Run(() =>
            {
                var apps = _packageManager.FindPackagesForUser(string.Empty)
                    .Where(p => p.IsFramework == false && !string.IsNullOrEmpty(p.DisplayName)).ToList();
                App.MainWindow.DispatcherQueue.TryEnqueue(() => AppPackages = new ObservableCollection<Package>(apps));
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
            Task.Run(() =>
            {
                var apps = _packageManager.FindPackagesForUser(string.Empty)
                    .Where(p => p.IsFramework == false && !string.IsNullOrEmpty(p.DisplayName)).ToList();
                App.MainWindow.DispatcherQueue.TryEnqueue(() => AppPackages = new ObservableCollection<Package>(apps));
            });
        }
        else
        {
            Win32PathVisibility = Visibility.Visible;
            MsixVisibility = Visibility.Collapsed;
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
      
    }

    [RelayCommand]
    private async Task SaveLaunchApp()
    {
       
    }
}
