using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.UI.Xaml;
using Verdure.ElectronBot.Core.Models;
using Windows.ApplicationModel;
using Windows.Storage;

namespace ElectronBot.Braincase.ViewModels;

public class EmojisInfoDialogViewModel : ObservableRecipient
{
    private ObservableCollection<EmoticonAction> _actions = new();

    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand<string>(OnLoaded);

    private ICommand _recordCommand;
    public ICommand RecordCommand => _recordCommand ??= new RelayCommand<bool>(RecordEmojisAction);


    private ICommand _addEmojisAvatarCommand;
    public ICommand AddEmojisAvatarCommand => _addEmojisAvatarCommand ??= new RelayCommand(AddEmojisAvatar);


    private ICommand _saveEmojisCommand;
    public ICommand SaveEmojisCommand => _saveEmojisCommand ??= new RelayCommand<string>(SaveEmojis);

    private readonly ILocalSettingsService _localSettingsService;

    private ObservableCollection<ElectronBotAction> actions = new();

    private readonly DispatcherTimer _dispatcherTimer;

    private bool _toggleIsOn = false;

    private int _interval = 500;
    public EmojisInfoDialogViewModel(ILocalSettingsService localSettingsService,
        DispatcherTimer dispatcherTimer)
    {
        _localSettingsService = localSettingsService;
        _dispatcherTimer = dispatcherTimer;

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private ICommand _clearCommand;

    public ICommand ClearCommand
    {
        get
        {
            _clearCommand ??= new RelayCommand(
                    () =>
                    {
                        ActionList.Clear();

                        ToastHelper.SendToast("PlayClearToastText".GetLocalized(), TimeSpan.FromSeconds(3));
                    });

            return _clearCommand;
        }
    }

    public bool ToggleIsOn
    {
        get => _toggleIsOn;
        set => SetProperty(ref _toggleIsOn, value);
    }


    private void DispatcherTimer_Tick(object? sender, object e)
    {
        if (ElectronBotHelper.Instance.EbConnected)
        {
            var data = new byte[240 * 240 * 3];

            var frame = new EmoticonActionFrame(data);

            ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);

            var jointAngles = ElectronBotHelper.Instance?.ElectronBot?.GetJointAngles();

            if (jointAngles != null)
            {
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
    } = new();

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

                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Interval);
                _dispatcherTimer.Start();
            }
        }
        else
        {
            _dispatcherTimer.Stop();
        }
    }

    private async void SaveEmojis(string? obj)
    {
        if (string.IsNullOrWhiteSpace(obj))
        {
            ToastHelper.SendToast("EmojisEmpty".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }

        if (ActionList.Count <= 0)
        {
            ToastHelper.SendToast("EmojisListEmpty".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }
        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"{obj}.json", CreationCollisionOption.ReplaceExisting);


        var content = JsonSerializer
            .Serialize(ActionList, options: new JsonSerializerOptions { WriteIndented = true });

        await FileIO.WriteTextAsync(storageFile, content);

        var list = (await _localSettingsService
           .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        foreach (var action in list)
        {
            if (action.NameId == obj)
            {
                action.EmojisActionPath = storageFile.Path;
                action.HasAction = true;
            }
        }

        EmoticonAction.HasAction = true;

        EmoticonAction.NameId = obj;

        EmoticonAction.EmojisActionPath = storageFile.Path;

        await _localSettingsService.SaveSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey, list);
    }

    private string _mojisName;
    private string _mojisNameId;
    private string _emojisDesc;
    private string _mojisAvatar;
    private string _emojisVideoUrl;


    private async void AddEmojisAvatar()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));
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

    private void OnLoaded(string? obj)
    {
        if (!string.IsNullOrWhiteSpace(obj))
        {
            var actionJson = obj;
            if (obj == "defaultaction.json")
            {
                actionJson = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\defaultaction.json";
            }
            var json = File.ReadAllText(actionJson);

            try
            {
                var actionList = JsonSerializer.Deserialize<List<ElectronBotAction>>(json);

                if (actionList != null && actionList.Count > 0)
                {
                    ActionList = new ObservableCollection<ElectronBotAction>(actionList);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
