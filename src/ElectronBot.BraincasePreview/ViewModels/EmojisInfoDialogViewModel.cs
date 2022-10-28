using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Controls;
using ElectronBot.BraincasePreview.Helpers;
using ElectronBot.BraincasePreview.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Verdure.ElectronBot.Core.Models;
using Windows.Storage;

namespace ElectronBot.BraincasePreview.ViewModels;

public class EmojisInfoDialogViewModel : ObservableRecipient
{
    private ObservableCollection<EmoticonAction> _actions = new();

    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

    private ICommand _recordCommand;
    public ICommand RecordCommand => _recordCommand ??= new RelayCommand<bool>(RecordEmojisAction);


    private ICommand _addEmojisAvatarCommand;
    public ICommand AddEmojisAvatarCommand => _addEmojisAvatarCommand ??= new RelayCommand(AddEmojisAvatar);


    private ICommand _openEmojisEditDialogCommand;
    public ICommand OpenEmojisEditDialogCommand => _openEmojisEditDialogCommand ??= new RelayCommand(EmojisEditDialogEmojis);

    private ICommand _saveEmojisCommand;
    public ICommand SaveEmojisCommand => _saveEmojisCommand ??= new RelayCommand(SaveEmojis);

    private readonly ILocalSettingsService _localSettingsService;

    private ObservableCollection<ElectronBotAction> actions = new();

    private readonly DispatcherTimer _dispatcherTimer;

    private int _interval = 500;
    public EmojisInfoDialogViewModel(ILocalSettingsService localSettingsService, DispatcherTimer dispatcherTimer)
    {
        _localSettingsService = localSettingsService;
        _dispatcherTimer = dispatcherTimer;

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private void DispatcherTimer_Tick(object? sender, object e)
    {
        if (ElectronBotHelper.Instance.EbConnected)
        {
            var data = new byte[240 * 240 * 3];

            var frame = new EmoticonActionFrame(data);

            ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);

            var jointAngles = ElectronBotHelper.Instance.ElectronBot.GetJointAngles();

            var actionData = new ElectronBotAction()
            {
                Id = Guid.NewGuid().ToString(),
                J1 = (int)jointAngles[0],
                J2 = (int)jointAngles[1],
                J3 = (int)jointAngles[2],
                J4 = (int)jointAngles[3],
                J5 = (int)jointAngles[4],
                J6 = (int)jointAngles[5]
            };

            ActionList.Add(actionData);
        }
    }

    public ObservableCollection<ElectronBotAction> ActionList
    {
        get => actions;
        set => SetProperty(ref actions, value);
    }

    public int Interval
    {
        get => _interval;
        set => SetProperty(ref _interval, value);
    }
    public EmoticonAction EmoticonAction
    {
        get; set;
    }

    private void RecordEmojisAction(bool obj)
    {
        if (!obj)
        {
            if (!ElectronBotHelper.Instance.EbConnected)
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                // await ResetActionAsync();

                EmojiPlayHelper.Current.Interval = 0;

                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Interval);
                _dispatcherTimer.Start();
            }
        }
        else
        {
            _dispatcherTimer.Stop();
        }
    }

    private async void SaveEmojis()
    {
        //EmoticonAction = new EmoticonAction()
        //{
        //    Avatar = EmojisAvatar,
        //    Desc = EmojisDesc,
        //    Name = EmojisName,
        //    NameId = EmojisNameId,
        //    EmojisVideoPath = EmojisVideoUrl,
        //    EmojisType = EmojisType.Custom
        //};

        //var list = (await _localSettingsService
        //    .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        //var actions = new List<EmoticonAction>()
        //{
        //    EmoticonAction
        //};

        //list.AddRange(actions);

        //await _localSettingsService.SaveSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey, list);
    }

    private async void EmojisEditDialogEmojis()
    {
        try
        {
            var addEmojisContentDialog = new AddEmojisContentDialog
            {
                Title = "AddEmojisTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            addEmojisContentDialog.DataContextChanged += AddEmojisContentDialog_DataContextChanged;

            await addEmojisContentDialog.ShowAsync();
        }
        catch (Exception)
        {

        }
    }

    private void AddEmojisContentDialog_DataContextChanged(Microsoft.UI.Xaml.FrameworkElement sender, Microsoft.UI.Xaml.DataContextChangedEventArgs args)
    {
        var value = args.NewValue;
    }

    private string _mojisName;
    private string _mojisNameId;
    private string _emojisDesc;
    private string _mojisAvatar;
    private string _emojisVideoUrl;

    private async void AddEmojisVideo()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("请设置表情id", TimeSpan.FromSeconds(3));
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

        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"{EmojisNameId}.mp4", CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteBytesAsync(storageFile, await file.ReadBytesAsync());

        EmojisVideoUrl = storageFile.Path;
    }

    private async void AddEmojisAvatar()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("请设置表情id", TimeSpan.FromSeconds(3));
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

        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"{EmojisNameId}.{file.FileType}", CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteBytesAsync(storageFile, await file.ReadBytesAsync());

        EmojisAvatar = storageFile.Path;
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
        var emoticonActions = Constants.EMOJI_ACTION_LIST;
        Actions = new ObservableCollection<EmoticonAction>(emoticonActions);
    }
}
