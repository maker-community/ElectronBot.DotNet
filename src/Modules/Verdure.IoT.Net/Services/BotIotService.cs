using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.IoT.Net.Services;
public class BotIotService : IBotIotService
{
    private readonly ILocalSettingsService _localSettingsService;
    public BotIotService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }
    public async Task PostServiceAync(IotRequest request)
    {
        var haSetting = await _localSettingsService.ReadSettingAsync<HaSetting>("HaSettingKeyKey");

        if (haSetting == null)
        {
            return;
        }

        var client = new HomeAssistantClient(haSetting.BaseUrl, haSetting.HaToken);

        await client.PostServiceAync(request.Domain, request.Service, request.EntityId);
    }
}
