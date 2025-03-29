using System.Text.Json;
using Controls;
using Verdure.Braincase.Core.Models.Emojis;
using Verdure.Braincase.Emojis.eShop;
using Windows.ApplicationModel;


namespace Verdure.Braincase.ViewModels;
public partial class EmojisEditViewModel : ObservableRecipient
{
    /// <summary>
    /// 导出表情
    /// </summary>
    /// <param name="obj"></param>
    [RelayCommand]
    public async Task ExportEmojisAsync(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个表情", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is EmoticonActionUIModel emojis)
        {
            try
            {
                if (emojis.Type == EmojisFileType.Default)
                {
                    ToastHelper.SendToast("默认表情禁止导出", TimeSpan.FromSeconds(3));
                    return;
                }

                var destinationFolder = await KnownFolders.PicturesLibrary
                     .CreateFolderAsync("ElectronBot\\EmojisFiles", CreationCollisionOption.OpenIfExists);

                var emojiData = await _emojisFileService.GetEmojisAsync(emojis.NameId);

                var path = await _emojisFileService.ExportEmojisFileToLocalAsync(emojiData, destinationFolder.Path);
                var text = "ExportToastText".GetLocalized();

                ToastHelper.SendToast($"{text}-{path}", TimeSpan.FromSeconds(5));

            }
            catch (Exception ex)
            {
                ToastHelper.SendToast($"导出错误-{ex.Message}", TimeSpan.FromSeconds(3));
            }
        }
    }

    [RelayCommand]
    public async Task MarketplaceAsync()
    {
        try
        {
            var marketplaceDialog = new ContentDialog()
            {
                Title = "MarketplaceDialogTitle".GetLocalized(),
                CloseButtonText = "MarketplaceDialogClose".GetLocalized(),
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Content.XamlRoot,
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
                            action.Type = emojisFileInfo.Type;
                            action.HasAction = emojisFileInfo.HasAction;
                        }
                        else
                        {
                            var actionFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

                            var storageFile = await actionFolder.CreateFileAsync(fileItem.Name, CreationCollisionOption.OpenIfExists);

                            await FileIO.WriteBytesAsync(storageFile, await fileItem.ReadBytesAsync());

                            if (storageFile.FileType == ".mp4")
                            {
                                var videoPath = await _emojisFileService.SaveEmojisFileAsync(storageFile.Path, storageFile.Name, storageFile.FileType);
                                action.EmojisVideoPath = videoPath;
                            }
                            else if (storageFile.FileType == ".png" ||
                                storageFile.FileType == ".jpg" ||
                                storageFile.FileType == ".jpeg")
                            {
                                var avatarPath = await _emojisFileService.SaveEmojisFileAsync(storageFile.Path, storageFile.Name, storageFile.FileType);
                                action.Avatar = avatarPath;
                            }
                            else if (storageFile.FileType == ".json")
                            {
                                var acitonContent = await FileIO.ReadTextAsync(fileItem);
                                action.EmojisActionPath = storageFile.Path;
                                action.EmojisActionContent = acitonContent;
                            }
                        }
                    }

                    var emojiModel = await _emojisFileService.SaveEmojisAsync(action);

                    if (emojiModel != null)
                    {
                        var data = new EmoticonActionUIModel
                        {
                            Name = emojiModel.Name,
                            NameId = emojiModel.NameId,
                            Desc = emojiModel.Desc,
                            EmojisActionJson = emojiModel.EmojisActionJson,
                            EmojisActionPath = emojiModel.EmojisActionPath,
                            EmojisAuthor = emojiModel.EmojisAuthor,
                            EmojisVideoPath = emojiModel.EmojisVideoPath,
                            HasAction = emojiModel.HasAction,
                            Type = emojiModel.Type
                        };

                        if (emojiModel.Avatar != null)
                        {
                            var bitmapImage = new BitmapImage();

                            await bitmapImage.SetSourceAsync(emojiModel.Avatar.AsRandomAccessStream());

                            data.Avatar = bitmapImage;
                        }

                        Emojis.Add(data);
                    }

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

    /// <summary>
    /// 重置表情文件
    /// </summary>
    [RelayCommand]
    public async Task ResetEmojisFileAsync()
    {
        try
        {
            var emojisPath = Package.Current.InstalledLocation.Path + "\\Assets\\Emoji";
            var folder = await StorageFolder.GetFolderFromPathAsync(emojisPath);
            var zipFiles = await folder.GetFilesAsync();

            foreach (var file in zipFiles)
            {
                if (file.FileType == ".zip")
                {
                    var localFolder = ApplicationData.Current.LocalFolder;
                    var storageFolder = await localFolder.CreateFolderAsync(Constants.DefaultEmojisTempFileFolder, CreationCollisionOption.OpenIfExists);

                    ZipFileCreatorHelper.ExtractZipFile(file.Path, storageFolder.Path);

                    var fileFolder = await storageFolder.GetFolderAsync(file.Name.Replace(".zip", ""));

                    var fileNames = await fileFolder.GetFilesAsync();

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
                                action.Type = emojisFileInfo.Type;
                                action.HasAction = emojisFileInfo.HasAction;
                            }
                            else
                            {
                                var actionFolder = await localFolder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

                                var storageFile = await actionFolder.CreateFileAsync(fileItem.Name, CreationCollisionOption.OpenIfExists);

                                await FileIO.WriteBytesAsync(storageFile, await fileItem.ReadBytesAsync());

                                if (storageFile.FileType == ".mp4")
                                {
                                    var videoPath = await _emojisFileService.SaveEmojisFileAsync(storageFile.Path, storageFile.Name, storageFile.FileType);
                                    action.EmojisVideoPath = videoPath;
                                }
                                else if (storageFile.FileType == ".png" ||
                                    storageFile.FileType == ".jpg" ||
                                    storageFile.FileType == ".jpeg")
                                {
                                    var avatarPath = await _emojisFileService.SaveEmojisFileAsync(storageFile.Path, storageFile.Name, storageFile.FileType);
                                    action.Avatar = avatarPath;
                                }
                                else if (storageFile.FileType == ".json")
                                {
                                    var acitonContent = await FileIO.ReadTextAsync(fileItem);
                                    action.EmojisActionPath = storageFile.Path;
                                    action.EmojisActionContent = acitonContent;
                                }
                            }
                        }

                        var emojiModel = await _emojisFileService.SaveEmojisAsync(action);

                        if (emojiModel != null)
                        {
                            var data = new EmoticonActionUIModel
                            {
                                Name = emojiModel.Name,
                                NameId = emojiModel.NameId,
                                Desc = emojiModel.Desc,
                                EmojisActionJson = emojiModel.EmojisActionJson,
                                EmojisActionPath = emojiModel.EmojisActionPath,
                                EmojisAuthor = emojiModel.EmojisAuthor,
                                EmojisVideoPath = emojiModel.EmojisVideoPath,
                                HasAction = emojiModel.HasAction,
                                Type = emojiModel.Type
                            };

                            if (emojiModel.Avatar != null)
                            {
                                var bitmapImage = new BitmapImage();

                                await bitmapImage.SetSourceAsync(emojiModel.Avatar.AsRandomAccessStream());

                                data.Avatar = bitmapImage;
                            }

                            Emojis.Remove(Emojis.Where(e => e.NameId == data.NameId).FirstOrDefault());
                            Emojis.Add(data);
                        }

                        await storageFolder.DeleteAsync();
                    }
                }
            }
            ToastHelper.SendToast("导入成功", TimeSpan.FromSeconds(3));
        }
        catch (Exception ex)
        {
            ToastHelper.SendToast($"导入失败-{ex.Message}", TimeSpan.FromSeconds(3));
        }
    }


    [RelayCommand]
    public async Task AddEmojisVideoAsync()
    {
        if (string.IsNullOrWhiteSpace(EmojisNameId))
        {
            ToastHelper.SendToast("SetEmojisNameId".GetLocalized(), TimeSpan.FromSeconds(3));

            return;
        }
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow());

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

    [RelayCommand]
    public async Task DelEmojisAsync(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个表情", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is EmoticonActionUIModel emojis)
        {
            try
            {
                if (emojis.Type == EmojisFileType.Default)
                {
                    ToastHelper.SendToast("默认表情禁止删除", TimeSpan.FromSeconds(3));
                    return;
                }
                Emojis.Remove(emojis);

                await _emojisFileService.RemoveEmojisAsync(emojis.NameId);
            }
            catch (Exception ex)
            {
                ToastHelper.SendToast($"删除失败-{ex.Message}", TimeSpan.FromSeconds(3));
            }
        }
    }

    [RelayCommand]
    public async Task PlayEmojisAsync(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个表情播放", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is EmoticonActionUIModel emojis)
        {
            try
            {
                List<ElectronBotAction> actions = new();

                if (emojis.HasAction)
                {
                    if (!string.IsNullOrWhiteSpace(emojis.EmojisActionJson))
                    {
                        try
                        {
                            var actionList = JsonSerializer.Deserialize<List<ElectronBotAction>>(emojis.EmojisActionJson);

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

                if (emojis.Type == EmojisFileType.Default)
                {
                    videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.NameId}.mp4";
                }
                else
                {
                    videoPath = emojis.EmojisVideoPath;
                }
                await _electronBotPlayer.PlayVideoByPathAsync(videoPath, actions);
            }
            catch (Exception)
            {

            }
        }
    }

    [RelayCommand]
    public async Task EmojisInfoAsync(object? obj)
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
                    XamlRoot = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Content.XamlRoot,
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
    private async Task UploadEmojisAsync(object? obj)
    {
        try
        {
            if (obj is EmoticonAction emojis)
            {
                if (emojis.Type == EmojisFileType.Default)
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
                    XamlRoot = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Content.XamlRoot,
                    RequestedTheme = _elementTheme,
                    Content = new UploadEmojisPage
                    {
                        EmoticonAction = emojis
                    }
                };

                var result = await uploadEmojisContentDialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    var eshpService = Ioc.Default.GetRequiredService<IEmojiseShopService>();

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

    [RelayCommand]
    public void SaveEmojisAsync()
    {
        Actions.Add(new EmoticonAction()
        {
            Avatar = EmojisAvatar,
            Desc = EmojisDesc,
            Name = EmojisName,
            NameId = EmojisNameId,
            Type = EmojisFileType.Custom
        });
    }

    [RelayCommand]
    public async Task OpenEmojisEditDialogAsync()
    {
        try
        {
            var addEmojisContentDialog = new ContentDialog()
            {
                Title = "AddEmojisTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Content.XamlRoot,
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
}
