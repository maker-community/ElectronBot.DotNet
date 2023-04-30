using System.Net.Http.Headers;
using System.Text.Json;
using Contracts.Services;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Models;
using Verdure.ElectronBot.Core.Helpers;
using Windows.Storage;

namespace Services;
public class EmojiseShopService : IEmojiseShopService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private const string ProfileImageUploadUri = "http://api.douwp.club";

    private readonly IEmojisFileService _emojisFileService;

    private readonly ILocalSettingsService _localSettingsService;
    public EmojiseShopService(IHttpClientFactory httpClientFactory,
        IEmojisFileService emojisFileService,
        ILocalSettingsService localSettingsService)
    {

        _httpClientFactory = httpClientFactory;
        _emojisFileService = emojisFileService;
        _localSettingsService = localSettingsService;
    }
    public async Task<EmoticonAction> DownloadEmojisAsync(string id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync($"{ProfileImageUploadUri}/api/GridFS/Download/{id}");

            if (response.IsSuccessStatusCode)
            {

                var contentDisposition = response?.Content?.Headers?.ContentDisposition.FileName;

                var fileName = contentDisposition.Replace("\"", "");

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var folder = ApplicationData.Current.LocalFolder;

                    var storageFolder = await folder.CreateFolderAsync(Constants.EmojisTempFileFolder, CreationCollisionOption.OpenIfExists);

                    using (var fileStream = new FileStream($"{storageFolder.Path}/{fileName}", FileMode.Create))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                    ZipFileCreatorHelper.ExtractZipFile($"{storageFolder.Path}/{fileName}", storageFolder.Path);

                    var file = await storageFolder.GetFileAsync(fileName);

                    if (file != null)
                    {
                        await file.DeleteAsync();
                    }

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

                                    await storageFolder.DeleteAsync();
                                    return null;
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

                        await storageFolder.DeleteAsync();

                        return new EmoticonAction();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ToastHelper.SendToast($"表情下载失败--{ex.Message}", TimeSpan.FromSeconds(3));

            return null;
        }
        return null;
    }
    public Task<EmojisItemDto> GetEmojisItemAsync(string id) => throw new NotImplementedException();
    public async Task<List<EmojisItemDto>> GetEmojisListAsync(EmojisItemQuery itemQuery)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync($"{ProfileImageUploadUri}/api/v1/Catalog/items/by_page?typeId=-1&brandId=-1&pageSize={itemQuery.PageSize}&pageIndex={itemQuery.PageIndex}");

        var list = new List<EmojisItemDto>();

        if (response.IsSuccessStatusCode)
        {
            var resultData = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var emojisData = JsonSerializer.Deserialize<EmojisHttpData>(resultData, options);

            if (emojisData is not null)
            {
                foreach (var emojisItem in emojisData.Items)
                {
                    var item = new EmojisItemDto
                    {
                        Id = emojisItem.Id,
                        Name = emojisItem.Name,
                        Desc = emojisItem.Desc,
                        PictureFileId = emojisItem.PictureFileId,
                        PictureFileName = $"{ProfileImageUploadUri}/api/Pics/{emojisItem.PictureFileName}",
                        Price = emojisItem.Price,
                        VideoFileId = emojisItem.VideoFileId,
                        CreateTime = emojisItem.CreateTime,
                        Author = emojisItem.Author,
                    };
                    list.Add(item);
                }
            }
        }
        return list;
    }
    public async Task<bool> UploadEmojisAsync(EmoticonAction emoticon)
    {
        var pathName = await _emojisFileService.ExportEmojisFileToTempAsync(emoticon);

        var videoFileId = await UploadEmojisFileAsync(pathName);

        var fileName = Path.GetFileName(emoticon.Avatar);

        var pictureFileName = await UploadEmojisImageAsync(emoticon.Avatar, fileName);

        var ret = await CreateEmojisInfoAsync(new EmojisItemRequest
        {

            Name = emoticon.Name,
            Desc = emoticon.Desc ?? emoticon.Name,
            PictureFileId = pictureFileName,
            VideoFileId = videoFileId,
            PictureFileName = pictureFileName,
            Author = emoticon.EmojisAuthor
        });

        return ret;
    }

    private async Task<bool> CreateEmojisInfoAsync(EmojisItemRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.PostAsync($"{ProfileImageUploadUri}/api/v1/Catalog/CreateCatalogItem", content);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }

    private async Task<string> UploadEmojisImageAsync(string path, string name)
    {
        if (path == null)
        {
            return string.Empty;
        }

        var httpClient = _httpClientFactory.CreateClient();

        var pictureFileName = string.Empty;

        using var videoContent = new MultipartFormDataContent();
        //replace with your own file path
        var filePath = Path.GetFullPath(path);

        var file = System.IO.File.ReadAllBytes(filePath);
        var byteArrayContent = new ByteArrayContent(file);
        videoContent.Add(byteArrayContent, "file", name);

        var result = await httpClient.PostAsync($"{ProfileImageUploadUri}/api/GridFS/UploadPicture", videoContent);

        if (result.IsSuccessStatusCode)
        {
            var resultData = await result.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var imageData = JsonSerializer.Deserialize<EmojisImageDto>(resultData, options);

            pictureFileName = imageData?.FileName ?? "";
        }

        return pictureFileName;
    }

    private async Task<string> UploadEmojisFileAsync((string path, string name) data)
    {
        if (data.path == null)
        {
            return string.Empty;
        }

        var httpClient = _httpClientFactory.CreateClient();

        var videoFIleId = string.Empty;

        using var videoContent = new MultipartFormDataContent();
        //replace with your own file path
        var filePath = Path.GetFullPath(data.path);

        var file = System.IO.File.ReadAllBytes(filePath);
        var byteArrayContent = new ByteArrayContent(file);
        byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

        videoContent.Add(byteArrayContent, "fs", data.name);

        var result = await httpClient.PostAsync($"{ProfileImageUploadUri}/api/GridFS/UploadSingle", videoContent);

        if (result.IsSuccessStatusCode)
        {
            videoFIleId = await result.Content.ReadAsStringAsync();
        }

        return videoFIleId;
    }
}
