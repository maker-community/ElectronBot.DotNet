using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contracts.Services;
using Controls;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Controls;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Models;
using Verdure.ElectronBot.Core.Helpers;
using Windows.ApplicationModel;
using Windows.Storage;

namespace ElectronBot.Braincase.ViewModels;

public partial class EmojisEditViewModel : ObservableRecipient
{
    private ObservableCollection<EmoticonAction> _actions = new();

    private ElementTheme _elementTheme;

    private readonly IActionExpressionProvider _actionExpressionProvider;

    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

    private ICommand _addEmojisVideoCommand;
    public ICommand AddEmojisVideoCommand => _addEmojisVideoCommand ??= new RelayCommand(AddEmojisVideo);


    private ICommand _openEmojisEditDialogCommand;
    public ICommand OpenEmojisEditDialogCommand => _openEmojisEditDialogCommand ??= new RelayCommand(EmojisEditDialogEmojis);

    private ICommand _saveEmojisCommand;
    public ICommand SaveEmojisCommand => _saveEmojisCommand ??= new RelayCommand(SaveEmojis);

    private ICommand _playEmojisCommand;
    public ICommand PlayEmojisCommand => _playEmojisCommand ??= new RelayCommand<object>(PlayEmojis);

    private ICommand _emojisInfoCommand;
    public ICommand EmojisInfoCommand => _emojisInfoCommand ??= new RelayCommand<object>(EmojisInfo);

    private string _mojisName;
    private string _mojisNameId;
    private string _emojisDesc;
    private string _mojisAvatar;
    private string _emojisVideoUrl;

    private readonly ILocalSettingsService _localSettingsService;

    private readonly IEmojisFileService _emojisFileService;


    private readonly IntPtr _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
    public EmojisEditViewModel(IActionExpressionProvider actionExpressionProvider,
        ILocalSettingsService localSettingsService,
        IEmojisFileService emojisFileService,
        IThemeSelectorService themeSelectorService)
    {
        _actionExpressionProvider = actionExpressionProvider;
        _localSettingsService = localSettingsService;
        _emojisFileService = emojisFileService;
        _elementTheme = themeSelectorService.Theme;
    }

    /// <summary>
    /// 导出表情
    /// </summary>
    /// <param name="obj"></param>
    [RelayCommand]
    public async void ExportEmojis(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个表情", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is EmoticonAction emojis)
        {
            try
            {
                if (emojis.EmojisType == EmojisType.Default)
                {
                    ToastHelper.SendToast("默认表情禁止导出", TimeSpan.FromSeconds(3));
                    return;
                }
                await _emojisFileService.ExportEmojisFileToLocalAsync(emojis);
            }
            catch (Exception ex)
            {
                ToastHelper.SendToast($"导出错误-{ex.Message}", TimeSpan.FromSeconds(3));
            }
            //ToastHelper.SendToast("导出成功", TimeSpan.FromSeconds(3));
        }
    }

    [RelayCommand]
    public async void Marketplace()
    {
        try
        {
            var marketplaceDialog = new ContentDialog()
            {
                Title = "MarketplaceDialogTitle".GetLocalized(),
                CloseButtonText = "MarketplaceDialogClose".GetLocalized(),
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                RequestedTheme = _elementTheme,
                Content = new MarketplacePage(),
            };
            marketplaceDialog.Closed += MarketplaceDialog_Closed;
            await marketplaceDialog.ShowAsync();
        }
        catch (Exception)
        {

        }
    }

    private void MarketplaceDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        OnLoaded();
    }

    /// <summary>
    /// 导入表情文件
    /// </summary>
    [RelayCommand]
    public async Task ImportEmojisFileAsync()
    {
        try
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
            };

            picker.FileTypeFilter.Add(".zip");

            WinRT.Interop.InitializeWithWindow.Initialize(picker, _hwnd);

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var folder = ApplicationData.Current.LocalFolder;

                var storageFolder = await folder.CreateFolderAsync(Constants.EmojisTempFileFolder, CreationCollisionOption.OpenIfExists);

                ZipFileCreatorHelper.ExtractZipFile(file.Path, storageFolder.Path);

                var fileNames = await storageFolder.GetFilesAsync();

                var list = (await _localSettingsService
                    .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

                var action = new EmoticonAction();

                if (fileNames != null && fileNames.Count > 0)
                {
                    foreach (var fileItem in fileNames)
                    {
                        if (fileItem.Name.Contains("manifest"))
                        {
                            var text = await FileIO.ReadTextAsync(fileItem);

                            var emojisFileInfo = JsonSerializer.Deserialize<EmojisFileManifest>(text) ?? throw new Exception("表情不存在");
                            action.Name = emojisFileInfo.Name;
                            action.NameId = emojisFileInfo.NameId;
                            action.Desc = emojisFileInfo.Description;
                            action.EmojisType = emojisFileInfo.EmojisType;
                            action.HasAction = emojisFileInfo.HasAction;
                        }
                        else
                        {
                            if (list.Where(e => e.NameId == fileItem.DisplayName).Any() || Constants.EMOJI_ACTION_LIST.Where(e => e.NameId == fileItem.DisplayName).Any())
                            {
                                ToastHelper.SendToast("EmojisNameIdAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));

                                return;
                            }

                            var actionFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

                            var storageFile = await actionFolder
                                .CreateFileAsync(fileItem.Name, CreationCollisionOption.OpenIfExists);

                            await FileIO.WriteBytesAsync(storageFile, await fileItem.ReadBytesAsync());

                            if (storageFile.FileType == ".mp4")
                            {
                                action.EmojisVideoPath = storageFile.Path;
                            }
                            else if (storageFile.FileType == ".png" ||
                                storageFile.FileType == ".jpg" ||
                                storageFile.FileType == ".jpeg")
                            {
                                action.Avatar = storageFile.Path;
                            }
                            else if (storageFile.FileType == ".json")
                            {
                                action.EmojisActionPath = storageFile.Path;
                            }
                        }
                    }

                    var actions = new List<EmoticonAction>()
                    {
                        action
                    };

                    list.AddRange(actions);

                    await _localSettingsService.SaveSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey, list);

                    Actions.Add(action);

                    await storageFolder.DeleteAsync();
                }
                ToastHelper.SendToast("导入成功", TimeSpan.FromSeconds(3));
            }
            else
            {
                ToastHelper.SendToast("取消导入", TimeSpan.FromSeconds(3));
            }
           
        }
        catch (Exception ex)
        {
            ToastHelper.SendToast($"导入失败-{ex.Message}", TimeSpan.FromSeconds(3));
        }
    }

    [RelayCommand]
    public async void DelEmojis(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个表情", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is EmoticonAction emojis)
        {
            try
            {
                if (emojis.EmojisType == EmojisType.Default)
                {
                    ToastHelper.SendToast("默认表情禁止删除", TimeSpan.FromSeconds(3));
                    return;
                }
                Actions.Remove(emojis);

                await _localSettingsService.SaveSettingAsync(Constants.EmojisActionListKey, Actions.ToList());

                var folder = ApplicationData.Current.LocalFolder;

                var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

                var avatarName = Path.GetFileName(emojis.Avatar);

                var avatarFile = await storageFolder.GetFileAsync(avatarName);

                await avatarFile.DeleteAsync();

                var videoName = Path.GetFileName(emojis.EmojisVideoPath);

                var videoFile = await storageFolder.GetFileAsync(videoName);

                await videoFile.DeleteAsync();

                if (emojis.HasAction)
                {
                    var actionName = Path.GetFileName(emojis.EmojisActionPath);

                    var actionFile = await storageFolder.GetFileAsync(actionName);

                    await actionFile.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                ToastHelper.SendToast($"删除失败-{ex.Message}", TimeSpan.FromSeconds(3));
            }
        }
    }

    private void PlayEmojis(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个表情播放", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is EmoticonAction emojis)
        {
            try
            {
                List<ElectronBotAction> actions = new();

                if (emojis.HasAction)
                {
                    if (!string.IsNullOrWhiteSpace(emojis.EmojisActionPath))
                    {
                        try
                        {
                            var path = string.Empty;

                            if (emojis.EmojisType == EmojisType.Default)
                            {
                                path = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.EmojisActionPath}";
                            }
                            else
                            {
                                path = emojis.EmojisActionPath;
                            }


                            var json = File.ReadAllText(path);


                            var actionList = JsonSerializer.Deserialize<List<ElectronBotAction>>(json);

                            if (actionList != null && actionList.Count > 0)
                            {
                                actions = actionList;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                string? videoPath;

                if (emojis.EmojisType == EmojisType.Default)
                {
                    videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.NameId}.mp4";
                }
                else
                {
                    videoPath = emojis.EmojisVideoPath;
                }
                _actionExpressionProvider.PlayActionExpressionAsync(emojis, actions);
                _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundAsync(videoPath);
            }
            catch (Exception)
            {

            }
        }
    }

    private async void EmojisInfo(object? obj)
    {
        try
        {
            if (obj is EmoticonAction emojis)
            {
                var emojisInfoContentDialog = new ContentDialog()
                {
                    Title = "EmojisInfoTitle".GetLocalized(),
                    CloseButtonText = "MarketplaceDialogClose".GetLocalized(),
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = App.MainWindow.Content.XamlRoot,
                    RequestedTheme = _elementTheme,
                    Content = new EmojisInfoPage
                    {
                        EmoticonAction = emojis
                    }
                };

                emojisInfoContentDialog.Closed += EmojisInfoContentDialog_Closed;

                await emojisInfoContentDialog.ShowAsync();
            }

        }
        catch (Exception)
        {

        }
    }

    [RelayCommand]
    private async void UploadEmojis(object? obj)
    {
        try
        {
            if (obj is EmoticonAction emojis)
            {
                if (emojis.EmojisType == EmojisType.Default)
                {
                    ToastHelper.SendToast("默认表情不能分享", TimeSpan.FromSeconds(3));
                    return;
                }

                var uploadEmojisContentDialog = new ContentDialog()
                {
                    Title = "UploadEmojisTitle".GetLocalized(),
                    PrimaryButtonText = "UploadEmojisOKBtn".GetLocalized(),
                    CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = App.MainWindow.Content.XamlRoot,
                    RequestedTheme = _elementTheme,
                    Content = new UploadEmojisPage
                    {
                        EmoticonAction = emojis
                    }
                };

                var result = await uploadEmojisContentDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    var eshpService = App.GetService<IEmojiseShopService>();

                    var ret = await eshpService.UploadEmojisAsync(emojis);

                    if (ret)
                    {
                        ToastHelper.SendToast("表情分享成功，审核通过即可显示。", TimeSpan.FromSeconds(3));
                    }
                    else
                    {
                        ToastHelper.SendToast("表情分享失败。", TimeSpan.FromSeconds(3));
                    }
                }
            }

        }
        catch (Exception)
        {

        }
    }

    private void EmojisInfoContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (sender.Content is EmojisInfoPage page)
        {
            if (page.DataContext is EmojisInfoDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    var emotion = viewModel.EmoticonAction;

                    if (emotion is not null)
                    {
                        var act = Actions.Where(a => a.NameId == emotion.NameId).FirstOrDefault();
                        if (act is not null)
                        {
                            act.HasAction = emotion.HasAction;
                        }
                    }
                }
            }
        }
    }

    private void SaveEmojis()
    {
        Actions.Add(new EmoticonAction()
        {
            Avatar = EmojisAvatar,
            Desc = EmojisDesc,
            Name = EmojisName,
            NameId = EmojisNameId,
            EmojisType = EmojisType.Custom
        });
    }

    private async void EmojisEditDialogEmojis()
    {
        try
        {
            var addEmojisContentDialog = new ContentDialog()
            {
                Title = "AddEmojisTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Content = new AddEmojisPage(),
                RequestedTheme = _elementTheme
            };

            addEmojisContentDialog.PrimaryButtonClick += AddEmojisContentDialog_PrimaryButtonClick;

            addEmojisContentDialog.Closed += AddEmojisContentDialog_Closed;

            var result = await addEmojisContentDialog.ShowAsync();
        }
        catch (Exception)
        {

        }
    }

    private async void AddEmojisContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (sender.Content is AddEmojisPage page)
        {
            if (page.DataContext is AddEmojisDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    if (string.IsNullOrWhiteSpace(viewModel.EmojisNameId))
                    {
                        ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(viewModel.EmojisName))
                    {
                        ToastHelper.SendToast("SetEmojisName".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }


                    var list = (await _localSettingsService
                        .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

                    if (list.Where(e => e.NameId == viewModel.EmojisNameId).Any() || Constants.EMOJI_ACTION_LIST.Where(e => e.NameId == viewModel.EmojisNameId).Any())
                    {
                        ToastHelper.SendToast("EmojisNameIdAlreadyExists".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }
                    viewModel.SaveEmojis();
                }
            }
        }
    }

    private void AddEmojisContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        if (sender.Content is AddEmojisPage page)
        {
            if (page.DataContext is AddEmojisDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    viewModel.SaveEmojis();
                }
            }
        }
    }

    private void AddEmojisContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (sender.Content is AddEmojisPage page)
        {
            if (page.DataContext is AddEmojisDialogViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    var emotion = viewModel.EmoticonAction;

                    if (emotion is not null)
                    {
                        Actions.Add(emotion);
                    }
                }
            }
        }
    }


    private async void AddEmojisVideo()
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

    private async void OnLoaded()
    {
        Actions.Clear();
        var list = (await _localSettingsService
            .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (!list.Any(a => a.EmojisType == EmojisType.Default))
        {
            var emoticonActions = Constants.EMOJI_ACTION_LIST;
            Actions = new ObservableCollection<EmoticonAction>(emoticonActions);


            await _localSettingsService.SaveSettingAsync(Constants.EmojisActionListKey, emoticonActions.ToList());
        }
        else
        {
            var isShouldUpdate = false;

            var emoticonActions = Constants.EMOJI_ACTION_LIST;

            foreach (var emotion in emoticonActions)
            {
                var emotionData = list.FirstOrDefault(e => e.NameId == emotion.NameId);

                if (emotionData != null)
                {
                    if (emotionData.EmojisActionPath != emotion.EmojisActionPath)
                    {
                        emotionData.EmojisActionPath = emotion.EmojisActionPath;
                        isShouldUpdate = true;
                    }
                }
                else
                {
                    list.Add(emotion);
                    isShouldUpdate = true;
                }
            }

            if (isShouldUpdate)
            {
                await _localSettingsService.SaveSettingAsync(Constants.EmojisActionListKey, list);
            }
        }


        await Task.Delay(TimeSpan.FromMilliseconds(500));

        foreach (var item in list)
        {
            Actions.Add(item);
        }
    }
}
