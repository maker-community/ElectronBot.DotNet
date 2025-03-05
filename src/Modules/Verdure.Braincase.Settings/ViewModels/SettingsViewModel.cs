using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Models;
using Verdure.Braincase.Services;
using Verdure.Braincase.Settings.Models;
using Verdure.Braincase.ViewModels;
using Verdure.Braincase.WinUI.Common.Players;
using Windows.ApplicationModel;
using Windows.System;

namespace Verdure.Braincase.Settings.ViewModels;

public partial class SettingsViewModel : ObservableRecipient, INavigationAware
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly ILocalSettingsService _localSettingsService;

    private readonly IdentityService _identityService;

    private readonly UserDataService _userDataService;

    private RelayCommand _logInCommand;
    private RelayCommand _logOutCommand;

    private readonly WindowEx _windowEx;

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
        _chatBotComboxModels = GetChatBotClientComboxList();
        _chatGPTVersionomboxModels = comboxDataService.GetChatGPTVersionComboxList();
        _llmVoiceComboxModels = comboxDataService.GetLlmVoiceComboxList();
        _windowEx = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow();
    }


    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private UserViewModel? _user;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private ObservableCollection<ComboxItemModel> _cameras;

    [ObservableProperty]
    private ObservableCollection<ComboxItemModel> _audioDevs;

    [ObservableProperty]
    private ObservableCollection<ComboxItemModel> _haSwitchs = new();

    /// <summary>
    /// 选中的相机
    /// </summary>
    [ObservableProperty]
    private ComboxItemModel _cameraSelect;

    /// <summary>
    /// 选中的开关设备
    /// </summary>
    [ObservableProperty]
    private ComboxItemModel _haSwitchSelect;

    /// <summary>
    /// 选中的音频设备
    /// </summary>

    [ObservableProperty]
    private ComboxItemModel _audioSelect;

    [ObservableProperty]
    private WriteableBitmap _emojisAvatarBitMap;

    [ObservableProperty]
    private string _customClockTitle;

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

    [ObservableProperty]

    private CustomClockTitleConfig _clockTitleConfig = new();

    [ObservableProperty]

    private BotSetting _botSetting = new();

    [ObservableProperty]

    private HaSetting _haSetting = new();

    public async void ToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            BotSetting.CustomViewContentIsVisibility = isVisual;
        }
        await _localSettingsService.SaveSettingAsync(Constants.BotSettingKey, BotSetting);
    }

    public async void IsHelloToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            BotSetting.IsHelloEnabled = isVisual;
        }
        await _localSettingsService.SaveSettingAsync(Constants.BotSettingKey, BotSetting);
    }

    public async void IsSessionSwitchToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            HaSetting.IsSessionSwitchEnabled = isVisual;
        }
        await _localSettingsService.SaveSettingAsync(Constants.HaSettingKey, HaSetting);
    }

    public async void Hw75ContentToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            ClockTitleConfig.Hw75CustomContentIsVisibility = isVisual;
        }

        await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
    }

    public async void Hw75TimeToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            ClockTitleConfig.Hw75TimeIsVisibility = isVisual;
        }

        await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
    }

    public async void Hw75ToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isVisual = toggleSwitch.IsOn;
            ClockTitleConfig.Hw75IsOpen = isVisual;
        }

        await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
    }

    public async void RangeBase_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {

        await _localSettingsService.SaveSettingAsync(Constants.BotSettingKey, BotSetting);
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
    public ObservableCollection<ComboxItemModel> _chatBotComboxModels;

    /// <summary>
    /// 语音服务列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> _llmVoiceComboxModels;

    /// <summary>
    /// 聊天机器人选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel _llmVoiceSelect;

    /// <summary>
    /// CHatGPTVersion选中数据
    /// </summary>
    [ObservableProperty]
    private ComboxItemModel _chatGPTVersionSelect;

    /// <summary>
    /// CHatGPTVersion列表
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ComboxItemModel> _chatGPTVersionomboxModels;


    [ObservableProperty]
    private string _versionDescription;


    [RelayCommand]
    private async Task AddEmojisAvatar()
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_windowEx);

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
            BotSetting.CustomViewPicturePath = storageFile.Path;
            await _localSettingsService.SaveSettingAsync(Constants.BotSettingKey, BotSetting);
            EmojisAvatar = storageFile.Path;
        }
    }

    [RelayCommand]
    private async Task RemoveEmojisAvatar()
    {
        BotSetting.CustomViewPicturePath = "";
        await _localSettingsService.SaveSettingAsync(Constants.BotSettingKey, BotSetting);
        EmojisAvatar = "";
    }

    [RelayCommand]
    public async Task ChatBotChangedAsync()
    {
        var chatBotName = ChatBotSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(chatBotName))
        {
            var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders);

            if (modelList != null)
            {
                CustomLlmModelSetting model = null;
                var models = modelList.FirstOrDefault(m => m.Provider == chatBotName);
                if (models != null)
                {
                    model = models.Models.FirstOrDefault();
                }
                else
                {
                    models = modelList.Where(m => m.Provider == "openai").FirstOrDefault();
                    if (models != null)
                    {
                        model = models.Models.FirstOrDefault(m => m.Provider == chatBotName);
                    }
                }
                if (model != null && !string.IsNullOrEmpty(model.ApiKey))
                {
                    var agentService = Ioc.Default.GetRequiredService<IAgentService>();

                    var agents = (await agentService.GetAgents(new AgentFilter
                    {
                        Pager = new Pagination
                        {
                            Page = 1,
                            Size = 100
                        }
                    })).Items.ToList();

                    foreach (var agent in agents)
                    {
                        agent.LlmConfig.Provider = model.Provider
                            .Replace("tongyi", "openai");

                        agent.LlmConfig.Model = model.Name;

                        await agentService.UpdateAgent(agent, AgentField.LlmConfig);
                    }
                    await _localSettingsService.SaveSettingAsync(Constants.DefaultChatBotNameKey, ChatBotSelect);
                    ToastHelper.SendToast("Save Ok", TimeSpan.FromSeconds(3));
                }
                else
                {
                    ChatBotSelect = null;
                    ToastHelper.SendToast("ApiKey is null", TimeSpan.FromSeconds(3));
                }
            }
        }
    }

    [RelayCommand]
    public async Task ChatGPTVersionChangedAsync()
    {
        var chatGPTName = ChatGPTVersionSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(chatGPTName))
        {
            var modelList = await _localSettingsService.ReadSettingAsync<List<CustomLlmProviderSetting>>(Constants.LlmProviders);

            BotSetting.ChatGPTVersion = chatGPTName;
            await _localSettingsService.SaveSettingAsync(Constants.DefaultChatGPTNameKey, chatGPTName);

            await _localSettingsService.SaveSettingAsync(Constants.BotSettingKey, BotSetting);
        }
    }

    [RelayCommand]
    public async Task LlmVoiceChangedAsync()
    {
        var llmVoiceName = LlmVoiceSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(llmVoiceName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultLlmVoiceNameKey, LlmVoiceSelect);
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


    /// <summary>
    /// TextChanged
    /// </summary>
    [RelayCommand]
    private async Task SwitchTheme(ElementTheme param)
    {
        if (ElementTheme != param)
        {
            ElementTheme = param;
            await _themeSelectorService.SetThemeAsync(param);
        }
    }

    /// <summary>
    /// TextChanged
    /// </summary>
    [RelayCommand]
    private async Task TextChanged()
    {
        await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
    }

    /// <summary>
    /// BotSetting
    /// </summary>
    [RelayCommand]
    private async Task SaveBotSetting()
    {
        await _localSettingsService.SaveSettingAsync(Constants.BotSettingKey, BotSetting);
    }

    /// <summary>
    /// HaSetting
    /// </summary>
    [RelayCommand]
    private async Task SaveHaSetting()
    {
        await _localSettingsService.SaveSettingAsync(Constants.HaSettingKey, HaSetting);
    }

    /// <summary>
    /// 音频切换方法
    /// </summary>
    [RelayCommand]
    private async Task Audio()
    {
        var audioName = AudioSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(audioName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultAudioNameKey, AudioSelect);
        }
    }
    /// <summary>
    /// 相机选择方法
    /// </summary>
    [RelayCommand]
    private async Task Camera()
    {
        var cameraName = CameraSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(cameraName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultCameraNameKey, CameraSelect);
        }
    }

    /// <summary>
    /// ha开关设备选择方法
    /// </summary>
    [RelayCommand]
    private async Task OnHaSwitchLoading()
    {
        if (HaSetting != null && !string.IsNullOrWhiteSpace(HaSetting.HaToken))
        {
            //try
            //{
            //    var client = new HomeAssistantClient(HaSetting.BaseUrl, HaSetting.HaToken);

            //    var stateAll = await client.GetStateAsync();

            //    var haSwitchs = stateAll.Where(d => d.entity_id.StartsWith("switch")).ToList();

            //    if (haSwitchs.Any())
            //    {
            //        foreach (var haSwitch in haSwitchs)
            //        {
            //            HaSwitchs.Add(new ComboxItemModel
            //            {
            //                DataKey = haSwitch.entity_id,
            //                DataValue = haSwitch.attributes.friendly_name
            //            });
            //        }
            //    }

            //    var haSwitchModel = await _localSettingsService.ReadSettingAsync<ComboxItemModel>(Constants.DefaultHaSwitchNameKey);

            //    if (haSwitchModel != null)
            //    {
            //        HaSwitchSelect = HaSwitchs.FirstOrDefault(c => c.DataValue == haSwitchModel.DataValue);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ToastHelper.SendToast($"home assistant error,{ex.Message}", TimeSpan.FromSeconds(4));
            //}
        }
    }

    /// <summary>
    /// ha开关设备选择方法
    /// </summary>
    [RelayCommand]
    private async Task HaSwitch()
    {
        var switchName = HaSwitchSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(switchName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultHaSwitchNameKey, HaSwitchSelect);
        }
    }

    public RelayCommand LogInCommand => _logInCommand ??= new RelayCommand(OnLogIn, () => !IsBusy);

    public RelayCommand LogOutCommand => _logOutCommand ??= new RelayCommand(OnLogOut, () => !IsBusy);


    //public bool IsBusy
    //{
    //    get => _isBusy;
    //    set
    //    {
    //        SetProperty(ref _isBusy, value);
    //        LogInCommand.NotifyCanExecuteChanged();
    //        LogOutCommand.NotifyCanExecuteChanged();
    //    }
    //}

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

    private async Task InitAsync()
    {
        try
        {
            var config = await _localSettingsService
                .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);
            ClockTitleConfig = config ?? new CustomClockTitleConfig();

            var botSetting = await _localSettingsService.ReadSettingAsync<BotSetting>(Constants.BotSettingKey);
            BotSetting = botSetting ?? new BotSetting();

            var haSetting = await _localSettingsService.ReadSettingAsync<HaSetting>(Constants.HaSettingKey);
            HaSetting = haSetting ?? new HaSetting();

            EmojisAvatar = BotSetting.CustomViewPicturePath;

            Hw75ImagePath = ClockTitleConfig.CustomHw75ImagePath;

            var camera = await ElectronBotPlayer.FindCameraDeviceListAsync();

            Cameras = new ObservableCollection<ComboxItemModel>(camera);

            var audioDevs = await ElectronBotPlayer.FindAudioDeviceListAsync();

            AudioDevs = new ObservableCollection<ComboxItemModel>(audioDevs);

            var cameraModel = await _localSettingsService.ReadSettingAsync<ComboxItemModel>(Constants.DefaultCameraNameKey);

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
                ChatBotSelect = ChatBotComboxModels.FirstOrDefault(c => c.DataValue == chatBotModel.DataValue);
            }

            var llmVoiceModel = await _localSettingsService
                .ReadSettingAsync<ComboxItemModel>(Constants.DefaultLlmVoiceNameKey);

            if (llmVoiceModel != null)
            {
                LlmVoiceSelect = LlmVoiceComboxModels.FirstOrDefault(c => c.DataValue == llmVoiceModel.DataValue);
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
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_windowEx);

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
        };

        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".bmp");

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
            await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
            Hw75ImagePath = storageFile.Path;
        }
    }

    [RelayCommand]
    private async Task RemoveHw75Image()
    {
        ClockTitleConfig.CustomHw75ImagePath = "ms-appx:///Assets/Images/Hw75CustomViewDefault.png";
        await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
        Hw75ImagePath = "";
    }


    [RelayCommand]
    private async Task Hw75CustomContentFontsize()
    {
        await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
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
    private ObservableCollection<ComboxItemModel> GetChatBotClientComboxList()
    {
        return new ObservableCollection<ComboxItemModel>
            {

                new() { DataKey = "azure-openai", DataValue = "AzureOpenai" },
                new() { DataKey = "openai", DataValue = "Openai" },
                new() { DataKey = "deepseek-ai", DataValue ="DeepseekAi" },
                new() { DataKey = "tongyi", DataValue ="Tongyi" }
            };
    }
}
