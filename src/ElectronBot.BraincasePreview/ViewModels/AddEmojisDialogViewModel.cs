using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Controls;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace ElectronBot.Braincase.ViewModels;

public partial class AddEmojisDialogViewModel : ObservableRecipient
{
    private ObservableCollection<EmoticonAction> _actions = new();

    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

    private ICommand _addEmojisVideoCommand;
    public ICommand AddEmojisVideoCommand => _addEmojisVideoCommand ??= new RelayCommand(AddEmojisVideo);

    private ICommand _addEmojisActionCommand;
    public ICommand AddEmojisActionCommand => _addEmojisActionCommand ??= new RelayCommand(AddEmojisAction);

    private ICommand _addEmojisAvatarCommand;
    public ICommand AddEmojisAvatarCommand => _addEmojisAvatarCommand ??= new RelayCommand(AddEmojisAvatar);


    private ICommand _openEmojisEditDialogCommand;
    public ICommand OpenEmojisEditDialogCommand => _openEmojisEditDialogCommand ??= new RelayCommand(EmojisEditDialogEmojis);

    private ICommand _saveEmojisCommand;
    public ICommand SaveEmojisCommand => _saveEmojisCommand ??= new RelayCommand(SaveEmojis);

    private readonly ILocalSettingsService _localSettingsService;

    private WriteableBitmap _emojisAvatarBitMap;
    public AddEmojisDialogViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public EmoticonAction EmoticonAction
    {
        get; set;
    }

    public WriteableBitmap EmojisAvatarBitMap
    {
        get => _emojisAvatarBitMap;
        set => SetProperty(ref _emojisAvatarBitMap, value);
    }
    public async void SaveEmojis()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        if (string.IsNullOrWhiteSpace(EmojisName))
        {
            ToastHelper.SendToast("SetEmojisName".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }


        var list = (await _localSettingsService
            .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (list.Where(e => e.NameId == EmojisNameId).Any() || Constants.EMOJI_ACTION_LIST.Where(e => e.NameId == EmojisNameId).Any())
        {
            ToastHelper.SendToast("EmojisNameIdAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        EmoticonAction = new EmoticonAction()
        {
            Avatar = EmojisAvatar,
            Desc = EmojisDesc,
            Name = EmojisName,
            NameId = EmojisNameId,
            EmojisVideoPath = EmojisVideoUrl,
            EmojisType = EmojisType.Custom,
            EmojisActionPath = EmojisActionPath,
            EmojisAuthor = EmojisAuthor
        };


        if (!string.IsNullOrWhiteSpace(EmojisActionPath))
        {
            EmoticonAction.HasAction = true;
        }

        var actions = new List<EmoticonAction>()
        {
            EmoticonAction
        };

        list.AddRange(actions);

        await _localSettingsService.SaveSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey, list);
    }

    private async void EmojisEditDialogEmojis()
    {
        try
        {
            var theme = App.GetService<IThemeSelectorService>();
            var addEmojisContentDialog = new AddEmojisContentDialog
            {
                Title = "AddEmojisTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                RequestedTheme = theme.Theme
            };


            await addEmojisContentDialog.ShowAsync();
        }
        catch (Exception)
        {

        }
    }

    private string _mojisName;
    private string _mojisNameId;
    private string _emojisDesc;
    private string _mojisAvatar;
    private string _emojisVideoUrl;
    private string _emojisActionPath;

    private async void AddEmojisVideo()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));
            return;
        }

        if (string.IsNullOrWhiteSpace(EmojisName))
        {
            ToastHelper.SendToast("SetEmojisName".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        if (!Regex.IsMatch(EmojisNameId, "^[A-Za-z0-9]+$"))
        {
            ToastHelper.SendToast("EmojisNameIdOnlyEn".GetLocalized(), TimeSpan.FromSeconds(3));
            return;
        }

        var list = (await _localSettingsService
         .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (list.Where(e => e.NameId == EmojisNameId).Any() || Constants.EMOJI_ACTION_LIST.Where(e => e.NameId == EmojisNameId).Any())
        {
            ToastHelper.SendToast("EmojisNameIdAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
        };

        picker.FileTypeFilter.Add(".mp4");

        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file is null)
        {
            return;
        }

        var propList = await file.GetBasicPropertiesAsync();

        var size = propList.Size;

        if (size > 1 * 1000 * 1000)
        {
            ToastHelper.SendToast("EmojisFileSize".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"{EmojisNameId}.mp4", CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteBytesAsync(storageFile, await file.ReadBytesAsync());

        EmojisVideoUrl = storageFile.Path;
    }


    private async void AddEmojisAction()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));
            return;
        }

        if (string.IsNullOrWhiteSpace(EmojisName))
        {
            ToastHelper.SendToast("SetEmojisName".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        if (!Regex.IsMatch(EmojisNameId, "^[A-Za-z0-9]+$"))
        {
            ToastHelper.SendToast("EmojisNameIdOnlyEn".GetLocalized(), TimeSpan.FromSeconds(3));
            return;
        }

        var list = (await _localSettingsService
         .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (list.Where(e => e.NameId == EmojisNameId).Any() || Constants.EMOJI_ACTION_LIST.Where(e => e.NameId == EmojisNameId).Any())
        {
            ToastHelper.SendToast("EmojisNameIdAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
        };

        picker.FileTypeFilter.Add(".json");

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

        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"{EmojisNameId}.json", CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteBytesAsync(storageFile, await file.ReadBytesAsync());

        EmojisActionPath = storageFile.Path;
    }

    private async void AddEmojisAvatar()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));
            return;
        }

        if (string.IsNullOrWhiteSpace(EmojisName))
        {
            ToastHelper.SendToast("SetEmojisName".GetLocalized(), TimeSpan.FromSeconds(3));
            return;
        }


        if (!Regex.IsMatch(EmojisNameId, "^[A-Za-z0-9]+$"))
        {
            ToastHelper.SendToast("EmojisNameIdOnlyEn".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        var list = (await _localSettingsService
     .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (list.Where(e => e.NameId == EmojisNameId).Any() || Constants.EMOJI_ACTION_LIST.Where(e => e.NameId == EmojisNameId).Any())
        {
            ToastHelper.SendToast("EmojisNameIdAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

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

        if (size > 1 * 1000 * 1000)
        {
            ToastHelper.SendToast("EmojisFileSize".GetLocalized(), TimeSpan.FromSeconds(3));

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
            .CreateFileAsync($"{EmojisNameId}{file.FileType}", CreationCollisionOption.ReplaceExisting);

        if (await ImageHelper.SaveWriteableBitmapImageFileAsync(croppedImage, storageFile))
        {
            EmojisAvatar = storageFile.Path;
        }
    }

    /// <summary>
    /// 表情名称
    /// </summary>
    public string EmojisName
    {
        get => _mojisName;
        set => SetProperty(ref _mojisName, value);
    }

    /// <summary>
    /// 表情标识
    /// </summary>
    public string EmojisNameId
    {
        get => _mojisNameId;
        set => SetProperty(ref _mojisNameId, value);
    }

    /// <summary>
    /// 表情图片
    /// </summary>
    public string EmojisAvatar
    {
        get => _mojisAvatar;
        set => SetProperty(ref _mojisAvatar, value);
    }

    /// <summary>
    /// 表情描述
    /// </summary>
    public string EmojisDesc
    {
        get => _emojisDesc;
        set => SetProperty(ref _emojisDesc, value);
    }

    [ObservableProperty]
    string emojisAuthor;

    /// <summary>
    /// 表情动作存储地址
    /// </summary>
    public string EmojisActionPath
    {
        get => _emojisActionPath;
        set => SetProperty(ref _emojisActionPath, value);
    }

    /// <summary>
    /// 表情视频存储地址
    /// </summary>
    public string EmojisVideoUrl
    {
        get => _emojisVideoUrl;
        set => SetProperty(ref _emojisVideoUrl, value);
    }

    public ObservableCollection<EmoticonAction> Actions
    {
        get => _actions;
        set => SetProperty(ref _actions, value);
    }

    private void OnLoaded()
    {
        //var emoticonActions = Constants.EMOJI_ACTION_LIST;
        //Actions = new ObservableCollection<EmoticonAction>(emoticonActions);
    }
}
