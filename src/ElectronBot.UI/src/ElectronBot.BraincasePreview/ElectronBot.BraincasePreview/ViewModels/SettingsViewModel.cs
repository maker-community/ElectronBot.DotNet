using System;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Contracts.ViewModels;
using ElectronBot.BraincasePreview.Helpers;
using ElectronBot.BraincasePreview.Models;
using Microsoft.UI.Xaml;

using Windows.ApplicationModel;

namespace ElectronBot.BraincasePreview.ViewModels;

public class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalSettingsService _localSettingsService;
    private ElementTheme _elementTheme;

    private string _customClockTitle;
    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public string CustomClockTitle
    {
        get => _customClockTitle;
        set => SetProperty(ref _customClockTitle, value);
    }



    private CustomClockTitleConfig _clockTitleConfig = new();
    public CustomClockTitleConfig ClockTitleConfig
    {
        get => _clockTitleConfig;
        set => SetProperty(ref _clockTitleConfig, value);
    }


    private string _versionDescription;

    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    private ICommand _switchThemeCommand;

    public ICommand SwitchThemeCommand
    {
        get
        {
            if (_switchThemeCommand == null)
            {
                _switchThemeCommand = new RelayCommand<ElementTheme>(
                    async (param) =>
                    {
                        if (ElementTheme != param)
                        {
                            ElementTheme = param;
                            await _themeSelectorService.SetThemeAsync(param);
                        }
                    });
            }

            return _switchThemeCommand;
        }
    }

    private ICommand _textChangedCommand;
    public ICommand TextChangedCommand
    {
        get
        {
            if (_textChangedCommand == null)
            {
                _textChangedCommand = new RelayCommand(
                    async () =>
                    {
                        //await _localSettingsService
                        //  .SaveSettingAsync<string>(Constants.CustomClockTitleKey, _customClockTitle);

                        await _localSettingsService
                       .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
                    });
            }

            return _textChangedCommand;
        }
    }

    public SettingsViewModel(
        IThemeSelectorService themeSelectorService,
        ILocalSettingsService localSettingsService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _localSettingsService = localSettingsService;
        VersionDescription = GetVersionDescription();
    }

    private async Task InitAsync()
    {
        //var ret = await _localSettingsService
        //    .ReadSettingAsync<string>(Constants.CustomClockTitleKey);

        //CustomClockTitle = ret ?? "";
        try
        {
            var ret2 = await _localSettingsService
                .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

            ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();
        }
        catch (Exception ex)
        {
        }
    }

    private static string GetVersionDescription()
    {
        var appName = "AppDisplayName".GetLocalized();

        var version = Package.Current.Id.Version;

        return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    public async void OnNavigatedTo(object parameter)
    {
        await InitAsync();
    }
    public void OnNavigatedFrom()
    {
    }
}
