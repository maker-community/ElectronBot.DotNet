using ElectronBot.Braincase.Models;
using Models;

namespace Contracts.Services;
public interface IEmojiseShopService
{
    Task<List<EmojisItemDto>> GetEmojisListAsync(EmojisItemQuery itemQuery);

    Task<EmojisItemDto> GetEmojisItemAsync(string id);

    Task<EmoticonAction> DownloadEmojisAsync(string id);

    Task<bool> UploadEmojisAsync(EmoticonAction emoticon);
}
