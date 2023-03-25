using System.Text.Json;
using Contracts.Services;
using ElectronBot.BraincasePreview.Models;
using Models;

namespace Services;
public class EmojiseShopService : IEmojiseShopService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private const string ProfileImageUploadUri = "http://api.douwp.club";

    private readonly IEmojisFileService _emojisFileService;
    public EmojiseShopService(IHttpClientFactory httpClientFactory, IEmojisFileService emojisFileService)
    {

        _httpClientFactory = httpClientFactory;
        _emojisFileService = emojisFileService;
    }
    public Task<EmoticonAction> DownloadEmojisAsync(string id) => throw new NotImplementedException();
    public Task<EmojisItemDto> GetEmojisItemAsync(string id) => throw new NotImplementedException();
    public Task<List<EmojisItemDto>> GetEmojisListAsync(EmojisItemQuery itemQuery) => throw new NotImplementedException();
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
            PictureFileName = pictureFileName
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
        videoContent.Add(byteArrayContent, "fs", data.name);

        var result = await httpClient.PostAsync($"{ProfileImageUploadUri}/api/GridFS/UploadSingle", videoContent);

        if (result.IsSuccessStatusCode)
        {
            videoFIleId = await result.Content.ReadAsStringAsync();
        }

        return videoFIleId;
    }
}
