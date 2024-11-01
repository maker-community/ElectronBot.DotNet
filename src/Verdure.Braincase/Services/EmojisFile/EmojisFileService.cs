using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Verdure.Braincase;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.Models;
using Models;
using Verdure.Braincase.Core.Models.Emojis;
using Verdure.Braincase.WinUI.Common.Helpers;
using Windows.ApplicationModel;
using Windows.Storage;
using Verdure.Braincase.Core.Contracts.Services.EmojisFile;

namespace Services;
public class EmojisFileService : IEmojisFileService
{
    public EmojisFileService()
    {

    }

    public Task<bool> ExistEmojisAsync(string nameId) => throw new NotImplementedException();

    public async Task<string> ExportEmojisFileToLocalAsync(EmoticonAction emoticonAction, string? targetPath)
    {
        StorageFolder? destinationFolder;

        var filePaths = new List<string>();
        try
        {
            destinationFolder = await KnownFolders.PicturesLibrary
            .CreateFolderAsync("ElectronBot\\EmojisFiles", CreationCollisionOption.OpenIfExists);

            var folder = ApplicationData.Current.LocalFolder;

            var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

            var storageFile = await storageFolder
                .CreateFileAsync($"{emoticonAction.NameId}-manifest.json", CreationCollisionOption.ReplaceExisting);

            var manifest = new EmojisFileManifest
            {
                Name = emoticonAction.Name,
                NameId = emoticonAction.NameId,
                Description = emoticonAction.Desc,
                HasAction = emoticonAction.HasAction,
                Type = emoticonAction.Type
            };


            var content = JsonSerializer
                .Serialize(manifest, options: new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), WriteIndented = true });

            await FileIO.WriteTextAsync(storageFile, content);

            filePaths.Add(storageFile.Path);
        }
        catch (Exception)
        {
            return string.Empty;
        }

        var fileName = $"{emoticonAction.NameId}-{DateTime.Now:yyyyMMddhhmmss}.zip";

        var destinationFile = await destinationFolder
            .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

        if (emoticonAction != null && emoticonAction.Type == EmojisFileType.Default)
        {
            var videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.mp4";
            filePaths.Add(videoPath);
            var picPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.png";
            filePaths.Add(picPath);
            var actionPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\defaultaction.json";
            filePaths.Add(actionPath);
        }
        else if (emoticonAction != null && emoticonAction.Type == EmojisFileType.Custom)
        {
            filePaths.Add(emoticonAction.Avatar);
            filePaths.Add(emoticonAction.EmojisVideoPath);
            if (emoticonAction.HasAction)
            {
                if (!string.IsNullOrWhiteSpace(emoticonAction.EmojisActionPath))
                {
                    filePaths.Add(emoticonAction.EmojisActionPath);
                }
            }
        }

        ZipFileCreatorHelper.CreateZipFile(filePaths, destinationFile.Path);

        var text = "ExportToastText".GetLocalized();

        ToastHelper.SendToast($"{text}-{destinationFile.Path}", TimeSpan.FromSeconds(5));

        return destinationFile.Path;
    }

    public async Task<(string path, string name)> ExportEmojisFileToTempAsync(EmoticonAction emoticonAction)
    {
        StorageFolder? destinationFolder;

        var filePaths = new List<string>();
        try
        {
            destinationFolder = await KnownFolders.PicturesLibrary
            .CreateFolderAsync("ElectronBot\\EmojisFilesTemp", CreationCollisionOption.OpenIfExists);

            var folder = ApplicationData.Current.LocalFolder;

            var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

            var storageFile = await storageFolder
                .CreateFileAsync($"{emoticonAction.NameId}-manifest.json", CreationCollisionOption.ReplaceExisting);

            var manifest = new EmojisFileManifest
            {
                Name = emoticonAction.Name,
                NameId = emoticonAction.NameId,
                Description = emoticonAction.Desc,
                HasAction = emoticonAction.HasAction,
                Type = emoticonAction.Type
            };


            var content = JsonSerializer
                .Serialize(manifest, options: new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), WriteIndented = true });

            await FileIO.WriteTextAsync(storageFile, content);

            filePaths.Add(storageFile.Path);


            var fileName = $"{emoticonAction.NameId}-{DateTime.Now:yyyyMMddhhmmss}.zip";

            var destinationFile = await destinationFolder
                .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            if (emoticonAction != null && emoticonAction.Type == EmojisFileType.Default)
            {
                var videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.mp4";
                filePaths.Add(videoPath);
                var picPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.png";
                filePaths.Add(picPath);
                var actionPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\defaultaction.json";
                filePaths.Add(actionPath);
            }
            else if (emoticonAction != null && emoticonAction.Type == EmojisFileType.Custom)
            {
                filePaths.Add(emoticonAction.Avatar);
                filePaths.Add(emoticonAction.EmojisVideoPath);
                if (emoticonAction.HasAction)
                {
                    if (!string.IsNullOrWhiteSpace(emoticonAction.EmojisActionPath))
                    {
                        filePaths.Add(emoticonAction.EmojisActionPath);
                    }
                }
            }

            ZipFileCreatorHelper.CreateZipFile(filePaths, destinationFile.Path);

            return (destinationFile.Path, destinationFile.Name);
        }
        catch (Exception)
        {
            return (string.Empty, string.Empty);
        }
    }

    public Task<EmoticonAction> GetEmojisAsync(string nameId) => throw new NotImplementedException();

    public Task<List<EmoticonAction>> GetEmojisFileListAsync()
    {
        return Task.FromResult(new List<EmoticonAction>());
    }

    public Task<List<EmoticonAction>> GetEmojisFileListAsync(int pageIndex, int pageSize) => throw new NotImplementedException();
    public Task<EmoticonActionModel> GetEmojisFileWithVideoStreamAsync(string nameId) => throw new NotImplementedException();
    public Task<bool> RemoveEmojisAsync(string nameId) => throw new NotImplementedException();
    public Task<EmoticonActionModel> SaveEmojisAsync(EmoticonAction emoticonAction) => throw new NotImplementedException();
    public Task<string> SaveEmojisFileAsync(Stream stream, string fileName, string fileType = ".mp4") => throw new NotImplementedException();
    public Task<string> SaveEmojisFileAsync(string path, string fileName, string fileType = ".mp4") => throw new NotImplementedException();
    Task<List<EmoticonActionModel>> IEmojisFileService.GetEmojisFileListAsync(int pageIndex, int pageSize) => throw new NotImplementedException();
}
