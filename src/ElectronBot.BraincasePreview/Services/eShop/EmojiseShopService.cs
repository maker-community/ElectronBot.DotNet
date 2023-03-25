using Contracts.Services;
using ElectronBot.BraincasePreview.Models;
using Models;

namespace Services;
public class EmojiseShopService : IEmojiseShopService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private const string ProfileImageUploadUri = "http://api.douwp.club/api/GridFS/UploadSingle";

    private readonly IEmojisFileService _emojisFileService;
    public EmojiseShopService(IHttpClientFactory httpClientFactory, IEmojisFileService emojisFileService)
    {

        _httpClientFactory = httpClientFactory;
        _emojisFileService = emojisFileService;
    }
    public Task<EmoticonAction> DownloadEmojisAsync(string id) => throw new NotImplementedException();
    public Task<EmojisItemDto> GetEmojisItemAsync(string id) => throw new NotImplementedException();
    public Task<List<EmojisItemDto>> GetEmojisListAsync(EmojisItemQuery itemQuery) => throw new NotImplementedException();
    public async Task<string> UploadEmojisAsync(EmoticonAction emoticon)
    {
        var pathName = await _emojisFileService.ExportEmojisFileToTempAsync(emoticon);

        if (pathName.path == null)
        {
            return string.Empty;
        }
        var httpClient = _httpClientFactory.CreateClient();

        var resultData = string.Empty;

        using var content = new MultipartFormDataContent();
        //replace with your own file path
        var filePath = Path.GetFullPath(pathName.path);

        var file = System.IO.File.ReadAllBytes(filePath);
        var byteArrayContent = new ByteArrayContent(file);
        content.Add(byteArrayContent, "fs",pathName.name);
        var result = await httpClient.PostAsync(ProfileImageUploadUri, content);

        if (result.IsSuccessStatusCode)
        {
            resultData = await result.Content.ReadAsStringAsync();
        }

        return resultData;
    }
}
