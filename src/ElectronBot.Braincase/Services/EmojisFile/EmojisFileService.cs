using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Contracts.Services;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Models;
using Verdure.ElectronBot.Core.Helpers;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Services;
public class EmojisFileService : IEmojisFileService
{
    public EmojisFileService()
    {

    }
    public async Task ExportEmojisFileToLocalAsync(EmoticonAction emoticonAction)
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
                EmojisType = emoticonAction.EmojisType
            };


            var content = JsonSerializer
                .Serialize(manifest, options: new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), WriteIndented = true });

            await FileIO.WriteTextAsync(storageFile, content);

            filePaths.Add(storageFile.Path);
        }
        catch (Exception)
        {
            return;
        }

        var fileName = $"{emoticonAction.NameId}-{DateTime.Now:yyyyMMddhhmmss}.zip";

        var destinationFile = await destinationFolder
            .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

        if (emoticonAction != null && emoticonAction.EmojisType == EmojisType.Default)
        {
            var videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.mp4";
            filePaths.Add(videoPath);
            var picPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.png";
            filePaths.Add(picPath);
            var actionPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\defaultaction.json";
            filePaths.Add(actionPath);
        }
        else if (emoticonAction != null && emoticonAction.EmojisType == EmojisType.Custom)
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
                EmojisType = emoticonAction.EmojisType
            };


            var content = JsonSerializer
                .Serialize(manifest, options: new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), WriteIndented = true });

            await FileIO.WriteTextAsync(storageFile, content);

            filePaths.Add(storageFile.Path);


            var fileName = $"{emoticonAction.NameId}-{DateTime.Now:yyyyMMddhhmmss}.zip";

            var destinationFile = await destinationFolder
                .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            if (emoticonAction != null && emoticonAction.EmojisType == EmojisType.Default)
            {
                var videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.mp4";
                filePaths.Add(videoPath);
                var picPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emoticonAction.NameId}.png";
                filePaths.Add(picPath);
                var actionPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\defaultaction.json";
                filePaths.Add(actionPath);
            }
            else if (emoticonAction != null && emoticonAction.EmojisType == EmojisType.Custom)
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
}
