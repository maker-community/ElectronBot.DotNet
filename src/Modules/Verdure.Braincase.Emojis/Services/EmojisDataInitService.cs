using System.Text.Json;
using Microsoft.Extensions.Logging;
using Verdure.Braincase.Core.Contracts.Services.EmojisFile;
using Verdure.Braincase.Core.Models.Emojis;
using Windows.ApplicationModel;

namespace Verdure.Braincase.Emojis.Services;
public class EmojisDataInitService : IDataInitService
{

    private readonly IEmojisFileService _emojisFileService;

    private readonly ILogger _logger;
    public EmojisDataInitService(
        IEmojisFileService emojisFileService,
        ILogger<EmojisDataInitService> logger)
    {
        _emojisFileService = emojisFileService;
        _logger = logger;
    }
    public async Task InitializeDataAsync()
    {
        var isExist = await _emojisFileService.ExistEmojisAsync("normal");
        if (!isExist)
        {
            await ResetEmojisFileAsync();
        }
    }

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

                        await _emojisFileService.SaveEmojisAsync(action);

                        await storageFolder.DeleteAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reset emojis file failed.");
        }
    }
}
