using Contracts.Services;
using ElectronBot.BraincasePreview.Models;
using Models;

namespace Services;
public class EmojiseShopService : IEmojiseShopService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private const string ProfileImageUploadUri = "";
    public EmojiseShopService(IHttpClientFactory httpClientFactory)
    {

        _httpClientFactory = httpClientFactory;

    }
    public Task<EmoticonAction> DownloadEmojisAsync(string id) => throw new NotImplementedException();
    public Task<EmojisItemDto> GetEmojisItemAsync(string id) => throw new NotImplementedException();
    public Task<List<EmojisItemDto>> GetEmojisListAsync(EmojisItemQuery itemQuery) => throw new NotImplementedException();
    public async Task<bool> UploadEmojisAsync(EmoticonAction emoticon)
    {
        var httpClient = _httpClientFactory.CreateClient();

        using (var content = new MultipartFormDataContent())
        {
            //replace with your own file path
            string filePath = Path.GetFullPath(emoticon.EmojisVideoPath);
            byte[] file = System.IO.File.ReadAllBytes(filePath);
            var byteArrayContent = new ByteArrayContent(file);
            content.Add(byteArrayContent, "file");
            var result = await httpClient.PostAsync(ProfileImageUploadUri, content);
        }
        return true;
    }
}
