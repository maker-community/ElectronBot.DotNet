using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using ElectronBot.BraincasePreview.Controls;
using ElectronBot.BraincasePreview.Core.Models;
using Microsoft.Graph;
using Microsoft.UI.Xaml.Controls;
using OpenCvSharp;
using Windows.Storage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ElectronBot.BraincasePreview.ViewModels;

public class EmojisEditViewModel : ObservableRecipient
{
    private ObservableCollection<EmoticonAction> _actions = new();

    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

    private ICommand _addEmojisCommand;
    public ICommand AddEmojisCommand => _addEmojisCommand ??= new RelayCommand(AddEmojis);


    private ICommand _openEmojisEditDialogCommand;
    public ICommand OpenEmojisEditDialogCommand => _openEmojisEditDialogCommand ??= new RelayCommand(EmojisEditDialogEmojis);

    private async void EmojisEditDialogEmojis()
    {
        try
        {
            var addEmojisContentDialog = new AddEmojisContentDialog();
            addEmojisContentDialog.Title = "Save your work?";
            addEmojisContentDialog.PrimaryButtonText = "Save";
            addEmojisContentDialog.CloseButtonText = "Cancel";
            addEmojisContentDialog.DefaultButton = ContentDialogButton.Primary;


            addEmojisContentDialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            await addEmojisContentDialog.ShowAsync();
        }
        catch (Exception ex)
        {

        }
    }

    private string _serverName;
    private string _serverUrl;
    public EmojisEditViewModel()
    {
    }


    private async void AddEmojis()
    {
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
            .CreateFileAsync("happy.mp4", CreationCollisionOption.ReplaceExisting);

        await FileIO.WriteBytesAsync(storageFile, await file.ReadBytesAsync());
    }

    public string AddEmojisTitle => "AddEmojisTitle".GetLocalized();

    public string AddEmojisOkBtnContent => "AddEmojisOkBtnContent".GetLocalized();

    public string AddEmojisCancelBtnContent => "AddEmojisCancelBtnContent".GetLocalized();


    public string ServerName
    {
        get => _serverName;
        set => SetProperty(ref _serverName, value);
    }
    public string ServerUrl
    {
        get => _serverUrl;
        set => SetProperty(ref _serverUrl, value);
    }

    public ObservableCollection<EmoticonAction> Actions
    {
        get => _actions;
        set => SetProperty(ref _actions, value);
    }

    private void OnLoaded()
    {
        var emoticonActions = new List<EmoticonAction>()
        {
            new EmoticonAction
            {
                Name = "开心1",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy1"
            },
            new EmoticonAction
            {
                Name = "开心2",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy2"
            },
            new EmoticonAction
            {
                Name = "开心3",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy3"
            },
            new EmoticonAction
            {
                Name = "开心4",
                Avatar = "ms-appx:///Assets/cat_with_wry_smile_3d.png",
                Desc = "这是开心的表情",
                NameKey ="happy4"
            },

        };
        Actions = new ObservableCollection<EmoticonAction>(emoticonActions);
    }
}
