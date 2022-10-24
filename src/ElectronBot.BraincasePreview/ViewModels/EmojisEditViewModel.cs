using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Controls;
using ElectronBot.BraincasePreview.Helpers;
using ElectronBot.BraincasePreview.Services;
using Microsoft.UI.Xaml.Controls;
using Verdure.ElectronBot.Core.Models;
using Windows.Storage;

namespace ElectronBot.BraincasePreview.ViewModels;

public class EmojisEditViewModel : ObservableRecipient
{
    private ObservableCollection<EmoticonAction> _actions = new();

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

    private string _mojisName;
    private string _mojisNameId;
    private string _emojisDesc;
    private string _mojisAvatar;
    private string _emojisVideoUrl;

    private readonly ILocalSettingsService _localSettingsService;


    public EmojisEditViewModel(IActionExpressionProvider actionExpressionProvider, ILocalSettingsService localSettingsService)
    {
        _actionExpressionProvider = actionExpressionProvider;
        _localSettingsService = localSettingsService;
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
                if (emojis.EmojisType == EmojisType.Default)
                {
                    _actionExpressionProvider.PlayActionExpressionAsync(emojis.NameId);
                }
                else
                {
                    _actionExpressionProvider.PlayActionExpressionAsync(emojis);
                }
            }
            catch (Exception)
            {

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
            var addEmojisContentDialog = new AddEmojisContentDialog
            {
                Title = "AddEmojisTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            addEmojisContentDialog.Closed += AddEmojisContentDialog_Closed;

            await addEmojisContentDialog.ShowAsync();
        }
        catch (Exception)
        {

        }
    }

    private void AddEmojisContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        if (sender.DataContext is AddEmojisDialogViewModel viewModel)
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
        var emoticonActions = Constants.EMOJI_ACTION_LIST;
        Actions = new ObservableCollection<EmoticonAction>(emoticonActions);


        var list = (await _localSettingsService
            .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        foreach (var item in list)
        {
            Actions.Add(item);
        }
    }
}
