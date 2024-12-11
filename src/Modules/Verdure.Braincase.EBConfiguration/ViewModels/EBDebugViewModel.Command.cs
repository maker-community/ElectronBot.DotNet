using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf.WellKnownTypes;
using Microsoft.UI.Xaml;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.WinUI.Common.Helpers;
using Verdure.Braincase.WinUI.Common.Models;
using Windows.Storage;

namespace Verdure.Braincase.EBConfiguration.ViewModels;
public partial class EBDebugViewModel : ObservableRecipient
{
    /// <summary>
    /// 导入动作列表
    /// </summary>
    [RelayCommand]
    public async Task ImportAsync()
    {
        //var list = await EbHelper.ImportActionListAsync(_hwnd);

        //Actions = new ObservableCollection<ElectronBotAction>(list);
    }

    [RelayCommand]
    public async Task PlayAsync()
    {
        //if (modeNo == 1)
        //{
        //    if (actions.Count > 0)
        //    {
        //        await ResetActionAsync();

        //        await EbHelper.PlayActionListAsync(Actions.ToList(), Interval);

        //    }
        //    else
        //    {
        //        ToastHelper.SendToast("PlayEmptyToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        //    }

        //}
        //else
        //{
        //    ToastHelper.SendToast("PlayErrorToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        //}
    }

    [RelayCommand]
    public void Stop()
    {
        //_dispatcherTimer.Stop();
    }

    [RelayCommand]
    public void Clear()
    {
        //actions.Clear();

        //count = 0;

        //actionCount = 0;

        //ToastHelper.SendToast("PlayClearToastText".GetLocalized(), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public void Reconnect()
    {
        //try
        //{
        //    _dispatcherTimer.Stop();
        //    //ElectronBotHelper.Instance?.ElectronBot?.Disconnect();
        //    ElectronBotHelper.Instance?.ElectronBot?.ResetDevice();
        //}
        //catch (Exception)
        //{

        //}


        //ToastHelper.SendToast("ReconnectText".GetLocalized(), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public async Task ResetAsync()
    {
        //if (modeNo == 1)
        //{
        //    if (ElectronBotHelper.Instance.EbConnected)
        //    {
        //        await ResetActionAsync();

        //        ToastHelper.SendToast("PlayResetToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        //    }
        //    else
        //    {
        //        ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        //    }

        //}
        //else
        //{
        //    ToastHelper.SendToast("PlayErrorToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        //}
    }

    [RelayCommand]
    public async Task ExportAsync()
    {
        StorageFolder destinationFolder = null;

        try
        {
            destinationFolder = await KnownFolders.PicturesLibrary
            .CreateFolderAsync("ElectronBot", CreationCollisionOption.OpenIfExists);
        }
        catch (Exception ex)
        {
            return;
        }

        if (Actions != null && Actions.Count > 0)
        {
            var fileName = $"electronbot-action-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.json";

            var destinationFile = await destinationFolder
                .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            var content = JsonSerializer
                .Serialize(Actions, options: new JsonSerializerOptions { WriteIndented = true });

            await FileIO.WriteTextAsync(destinationFile, content);

            var text = "ExportToastText".GetLocalized();

            ToastHelper.SendToast($"{text}-{destinationFile.Path}", TimeSpan.FromSeconds(5));
        }
        else
        {
            ToastHelper.SendToast("PlayEmptyToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        }
    }

    [RelayCommand]
    public void Add()
    {
        if (SelectIndex < 0)
        {
            SelectIndex = 0;
        }
        else if (SelectIndex > Actions.Count)
        {
            SelectIndex = Actions.Count;
        }

        if (Actions.Count > 0)
        {
            Actions.Insert(SelectIndex + 1, new ElectronBotAction
            {
                J1 = J1,
                J2 = J2,
                J3 = J3,
                J4 = J4,
                J5 = J5,
                J6 = J6
            });
        }
        else
        {
            Actions.Add(new ElectronBotAction
            {
                J1 = J1,
                J2 = J2,
                J3 = J3,
                J4 = J4,
                J5 = J5,
                J6 = J6
            });
        }
    }

    [RelayCommand]
    public void RemoveAction()
    {
        if (SelectIndex < 0)
        {
            SelectIndex = 0;
        }
        else if (SelectIndex > Actions.Count)
        {
            SelectIndex = Actions.Count;
        }

        Actions.RemoveAt(SelectIndex);
    }

    [RelayCommand]
    public async Task AddPictureAsync()
    {
        //var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        //var picker = new Windows.Storage.Pickers.FileOpenPicker
        //{
        //    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

        //    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
        //};

        //picker.FileTypeFilter.Add(".png");
        //picker.FileTypeFilter.Add(".jpg");
        //picker.FileTypeFilter.Add(".jpeg");

        //WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        //var file = await picker.PickSingleFileAsync();

        //if (file != null)
        //{
        //    var config = new ImageCropperConfig
        //    {
        //        ImageFile = file,
        //        AspectRatio = 1
        //    };

        //    var croppedImage = await ImageHelper.CropImage(config);

        //    if (croppedImage != null)
        //    {
        //        SelectdAction.BitmapImageData = croppedImage;

        //        var act = Actions.Where(i => i.Id == selectdAction.Id).FirstOrDefault();

        //        if (act != null)
        //        {
        //            var bytes = croppedImage.PixelBuffer.ToArray();

        //            var imageData = await EbHelper.ToBase64Async(
        //                bytes, (uint)croppedImage.PixelWidth, (uint)croppedImage.PixelWidth);

        //            act.ImageData = $"data:image/png;base64,{imageData}";

        //            act.BitmapImageData = croppedImage;
        //        }
        //    }
        //}
    }
}
