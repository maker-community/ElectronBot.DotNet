using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Verdure.ElectronBot.Core.Helpers;
using Verdure.ElectronBot.Core.Models;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;

namespace ElectronBot.Braincase.ViewModels;

public partial class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalSettingsService _localSettingsService;
    private ElementTheme _elementTheme;

    private ObservableCollection<ComboxItemModel> _cameras;

    private ObservableCollection<ComboxItemModel> _audioDevs;
    private readonly IdentityService _identityService;

    private readonly UserDataService _userDataService;


    private ComboxItemModel _cameraSelect;

    private ComboxItemModel _audioSelect;


    private WriteableBitmap _emojisAvatarBitMap;


    private string _customClockTitle;
    private RelayCommand _logInCommand;
    private RelayCommand _logOutCommand;
    private bool _isLoggedIn;
    private bool _isBusy;
    private UserViewModel _user;
    public SettingsViewModel(
    IThemeSelectorService themeSelectorService,
    ILocalSettingsService localSettingsService,
    ComboxDataService comboxDataService,
    IdentityService identityService,
    UserDataService userDataService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _localSettingsService = localSettingsService;
        VersionDescription = GetVersionDescription();
        _identityService = identityService;
        _userDataService = userDataService;
        chatBotComboxModels = comboxDataService.GetChatBotClientComboxList();
        _chatGPTVersionomboxModels = comboxDataService.GetChatGPTVersionComboxList();
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

    public WriteableBitmap EmojisAvatarBitMap
    {
        get => _emojisAvatarBitMap;
        set => SetProperty(ref _emojisAvatarBitMap, value);
    }

    [ObservableProperty]
    public WriteableBitmap _hw75BitMap;

    /// <summary>
    /// 表情图片
    /// </summary>
    [ObservableProperty] public string emojisAvatar;

    /// <summary>
    /// Hw75图片
    /// </summary>
    [ObservableProperty] public string _hw75ImagePath;

    private CustomClockTitleConfig _clockTitleConfig = new();
    public CustomClockTitleConfig ClockTitleConfig
    {
        get => _clockTitleConfig;
        set => SetProperty(ref _clockTitleConfig, value);
    }

    public async void ToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            ClockTitleConfig.CustomViewContentIsVisibility = isVisual;
        }

        await _localSettingsService
            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
    }

    public async void Hw75ContentToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            ClockTitleConfig.Hw75CustomContentIsVisibility = isVisual;
        }

        await _localSettingsService
            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
    }

    public async void Hw75TimeToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            ClockTitleConfig.Hw75TimeIsVisibility = isVisual;
        }

        await _localSettingsService
            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
    }

    public async void Hw75ToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            ClockTitleConfig.Hw75IsOpen = isVisual;
        }

        await _localSettingsService
            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
    }

    public async void RangeBase_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {

        await _localSettingsService
            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
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
    /// 聊天机器人选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel chatBotSelect;

    /// <summary>
    /// 聊天机器人列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> chatBotComboxModels;

    /// <summary>
    /// CHatGPTVersion选中数据
    /// </summary>
    [ObservableProperty]
    private ComboxItemModel? _chatGPTVersionSelect;

    /// <summary>
    /// CHatGPTVersion列表
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ComboxItemModel>? _chatGPTVersionomboxModels;


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

    [RelayCommand]
    private async void AddEmojisAvatar()
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
        };

        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");

        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file is null)
        {
            return;
        }

        var propList = await file.GetBasicPropertiesAsync();

        var size = propList.Size;

        if (size > 5 * 1000 * 1000)
        {
            ToastHelper.SendToast("EmojisActionFileSize".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        var config = new ImageCropperConfig
        {
            ImageFile = file,
            AspectRatio = 1
        };

        var croppedImage = await ImageHelper.CropImage(config);

        if (croppedImage is null)
        {
            return;
        }

        EmojisAvatarBitMap = croppedImage;

        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"CustomViewPicture-{DateTime.Now.Second}{file.FileType}", CreationCollisionOption.ReplaceExisting);

        if (await ImageHelper.SaveWriteableBitmapImageFileAsync(croppedImage, storageFile))
        {
            ClockTitleConfig.CustomViewPicturePath = storageFile.Path;
            await _localSettingsService
                .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
            EmojisAvatar = storageFile.Path;
        }
    }

    [RelayCommand]
    private async Task RemoveEmojisAvatar()
    {
        ClockTitleConfig.CustomViewPicturePath = "";
        await _localSettingsService
            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
        EmojisAvatar = "";
    }

    [RelayCommand]
    public async Task ChatBotChangedAsync()
    {
        var chatBotName = ChatBotSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(chatBotName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultChatBotNameKey, chatBotSelect);
        }
    }

    [RelayCommand]
    public async Task ChatGPTVersionChangedAsync()
    {
        var chatGPTName = ChatGPTVersionSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(chatGPTName))
        {
            ClockTitleConfig.ChatGPTVersion = chatGPTName;
            await _localSettingsService.SaveSettingAsync(Constants.DefaultChatGPTNameKey, chatGPTName);

            await _localSettingsService
                .SaveSettingAsync(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
        }
    }

    [RelayCommand]
    public async Task FeedbackBtnAsync()
    {
        await FeedbackAsync("gil.zhang.dev@outlook.com", "反馈", "这是一些反馈");
    }

    public async Task FeedbackAsync(string address, string subject, string body)
    {
        if (address == null)
        {
            return;
        }
        var mailto = new Uri($"mailto:{address}?subject={subject}&body={body}");
        await Launcher.LaunchUriAsync(mailto);
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

    public RelayCommand LogInCommand => _logInCommand ?? (_logInCommand = new RelayCommand(OnLogIn, () => !IsBusy));

    public RelayCommand LogOutCommand => _logOutCommand ?? (_logOutCommand = new RelayCommand(OnLogOut, () => !IsBusy));

    public bool IsLoggedIn
    {
        get
        {
            return _isLoggedIn;
        }
        set
        {
            SetProperty(ref _isLoggedIn, value);
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            SetProperty(ref _isBusy, value);
            LogInCommand.NotifyCanExecuteChanged();
            LogOutCommand.NotifyCanExecuteChanged();
        }
    }

    public void UnregisterEvents()
    {
        _identityService.LoggedIn -= OnLoggedIn;
        _identityService.LoggedOut -= OnLoggedOut;
        _userDataService.UserDataUpdated -= OnUserDataUpdated;
    }

    private void OnUserDataUpdated(object sender, UserViewModel userData)
    {
        User = userData;
    }

    private async void OnLogIn()
    {
        IsBusy = true;
        var loginResult = await _identityService.LoginAsync();
        if (loginResult != LoginResultType.Success)
        {
            await AuthenticationHelper.ShowLoginErrorAsync(loginResult);
            IsBusy = false;
        }
    }

    private async void OnLogOut()
    {
        IsBusy = true;
        await _identityService.LogoutAsync();
    }

    private void OnLoggedIn(object sender, EventArgs e)
    {
        IsLoggedIn = true;
        IsBusy = false;
    }

    private void OnLoggedOut(object sender, EventArgs e)
    {
        User = null;
        IsLoggedIn = false;
        IsBusy = false;
    }

    public UserViewModel User
    {
        get
        {
            return _user;
        }
        set
        {
            SetProperty(ref _user, value);
        }
    }


    private async Task InitAsync()
    {
        try
        {
            var ret2 = await _localSettingsService
                .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);
            ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

            EmojisAvatar = ClockTitleConfig.CustomViewPicturePath;

            Hw75ImagePath = ClockTitleConfig.CustomHw75ImagePath;

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

            var chatBotModel = await _localSettingsService
                .ReadSettingAsync<ComboxItemModel>(Constants.DefaultChatBotNameKey);

            if (chatBotModel != null)
            {
                ChatBotSelect = chatBotComboxModels.FirstOrDefault(c => c.DataValue == chatBotModel.DataValue);
            }

            var chatGPTModel = await _localSettingsService
                .ReadSettingAsync<string>(Constants.DefaultChatGPTNameKey);

            if (!string.IsNullOrWhiteSpace(chatGPTModel))
            {
                ChatGPTVersionSelect = ChatGPTVersionomboxModels?.FirstOrDefault(c => c.DataKey == chatGPTModel);
            }
            else
            {
                ChatGPTVersionSelect = ChatGPTVersionomboxModels?.FirstOrDefault(c => c.DataKey == ClockTitleConfig.ChatGPTVersion);
            }

            _identityService.LoggedIn += OnLoggedIn;
            _identityService.LoggedOut += OnLoggedOut;
            _userDataService.UserDataUpdated += OnUserDataUpdated;
            IsLoggedIn = _identityService.IsLoggedIn();
            User = await _userDataService.GetUserAsync();
        }
        catch (Exception ex)
        {
        }
    }



    #region 瀚文配置相关
    [RelayCommand]
    private async Task AddHw75Image()
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
        };

        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");

        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file is null)
        {
            return;
        }

        var propList = await file.GetBasicPropertiesAsync();

        var size = propList.Size;

        if (size > 5 * 1000 * 1000)
        {
            ToastHelper.SendToast("EmojisActionFileSize".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        var config = new ImageCropperConfig
        {
            ImageFile = file,
            AspectRatio = 128d / 296d
        };

        var croppedImage = await ImageHelper.CropImage(config);

        if (croppedImage is null)
        {
            return;
        }

        Hw75BitMap = croppedImage;

        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"CustomHw75Image-{DateTime.Now.Second}{file.FileType}", CreationCollisionOption.ReplaceExisting);

        if (await ImageHelper.SaveWriteableBitmapImageFileAsync(croppedImage, storageFile))
        {
            ClockTitleConfig.CustomHw75ImagePath = storageFile.Path;
            await _localSettingsService
                .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
            Hw75ImagePath = storageFile.Path;
        }
    }

    [RelayCommand]
    private async Task RemoveHw75Image()
    {
        ClockTitleConfig.CustomHw75ImagePath = "ms-appx:///Assets/Images/Hw75CustomViewDefault.png";
        await _localSettingsService
            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
        Hw75ImagePath = "";
    }


    [RelayCommand]
    private async Task Hw75CustomContentFontsize()
    {
        await _localSettingsService
                            .SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, _clockTitleConfig);
    }

    #endregion

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
        UnregisterEvents();
    }
}
