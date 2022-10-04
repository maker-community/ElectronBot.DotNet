using System.Collections.ObjectModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Contracts.ViewModels;
using ElectronBot.BraincasePreview.Core.Models;
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

    private ObservableCollection<ComboxItemModel> _cameras;

    private ObservableCollection<ComboxItemModel> _audioDevs;


    private ComboxItemModel _cameraSelect;

    private ComboxItemModel _audioSelect;


    private string _customClockTitle;

    public SettingsViewModel(
    IThemeSelectorService themeSelectorService,
    ILocalSettingsService localSettingsService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _localSettingsService = localSettingsService;
        VersionDescription = GetVersionDescription();
    }


    private ICommand _cameraCommand;
    public ICommand CameraCommand => _cameraCommand ??= new RelayCommand(CameraChanged);

    private ICommand _audioCommand;
    public ICommand AudioCommand => _audioCommand ??= new RelayCommand(AudioChanged);
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



    /// <summary>
    /// 选中的相机
    /// </summary>
    public ComboxItemModel CameraSelect
    {
        get => _cameraSelect;
        set => SetProperty(ref _cameraSelect, value);
    }

    /// <summary>
    /// 选中的音频设备
    /// </summary>
    public ComboxItemModel AudioSelect
    {
        get => _audioSelect;
        set => SetProperty(ref _audioSelect, value);
    }



    /// <summary>
    /// 相机列表
    /// </summary>
    public ObservableCollection<ComboxItemModel> Cameras
    {
        get => _cameras;
        set => SetProperty(ref _cameras, value);
    }

    /// <summary>
    /// 音频设备列表
    /// </summary>
    public ObservableCollection<ComboxItemModel> AudioDevs
    {
        get => _audioDevs;
        set => SetProperty(ref _audioDevs, value);
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

    /// <summary>
    /// 音频切换方法
    /// </summary>
    private async void AudioChanged()
    {
        var audioName = _audioSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(audioName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultAudioNameKey, _audioSelect);
        }
    }
    /// <summary>
    /// 相机选择方法
    /// </summary>
    private async void CameraChanged()
    {
        var cameraName = _cameraSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(cameraName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultCameraNameKey, _cameraSelect);
        }
    }

    private async Task InitAsync()
    {
        try
        {
            var ret2 = await _localSettingsService
                .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);
            ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

            var camera = await EbHelper.FindCameraDeviceListAsync();

            Cameras = new ObservableCollection<ComboxItemModel>(camera);


            var audioDevs = await EbHelper.FindAudioDeviceListAsync();

            AudioDevs = new ObservableCollection<ComboxItemModel>(audioDevs);

            var cameraModel = await _localSettingsService
                .ReadSettingAsync<ComboxItemModel>(Constants.DefaultCameraNameKey);

            if (cameraModel != null)
            {
                CameraSelect = Cameras.FirstOrDefault(c => c.DataValue == cameraModel.DataValue);

            }

            var audioModel = await _localSettingsService
                .ReadSettingAsync<ComboxItemModel>(Constants.DefaultAudioNameKey);

            if (audioModel != null)
            {
                AudioSelect = AudioDevs.FirstOrDefault(c => c.DataValue == audioModel.DataValue);
            }
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
