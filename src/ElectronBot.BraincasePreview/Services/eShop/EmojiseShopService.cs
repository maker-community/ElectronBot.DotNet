using System.Net.Http.Headers;
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
            var mixed = new MultipartContent("mixed") 
            {
            //CreateFileContent (imageStream, "image.jpg", "image/jpeg"),
            //CreateFileContent (signatureStream, "image.jpg.sig", "application/octet-stream")
            };
            content.Add(mixed, "files");
            var response = await httpClient.PostAsync(ProfileImageUploadUri, content);
            response.EnsureSuccessStatusCode();
        }

        return true;
    }

    private StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
    {
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("file") { FileName = fileName };
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return fileContent;
    }
}