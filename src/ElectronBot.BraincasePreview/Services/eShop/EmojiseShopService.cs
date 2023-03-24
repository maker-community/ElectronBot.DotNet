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
    public async Task<bool> UploadEmojisAsync(EmoticonAction emoticon)
    {
        var path = await _emojisFileService.ExportEmojisFileToTempAsync(emoticon);

        if (path == null)
        {
            return false;
        }
        var httpClient = _httpClientFactory.CreateClient();

        using var content = new MultipartFormDataContent();
        //replace with your own file path
        var filePath = Path.GetFullPath(path);
        var file = System.IO.File.ReadAllBytes(filePath);
        var byteArrayContent = new ByteArrayContent(file);
        content.Add(byteArrayContent, "fs","a.zip");
        var result = await httpClient.PostAsync(ProfileImageUploadUri, content);

        var resultContent = await result.Content.ReadAsStringAsync();

        return true;
    }
}
