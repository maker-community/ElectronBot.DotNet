using ChatGPT.Net;
using ChatGPT.Net.DTO;
using ChatGPT.Net.Enums;
using ChatGPT.Net.Session;
using Contracts.Services;
using ElectronBot.BraincasePreview;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Models;

namespace Services;
public class ChatGPTService : IChatGPTService
{
    private readonly ILocalSettingsService _localSettingsService;

    private readonly ChatGpt chatGpt;

    private ChatGptClient? chatGptClient;

    public ChatGPTService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        chatGpt = new ChatGpt(new ChatGptConfig
        {
            UseCache = true
        });
    }
    public async Task<string> AskQuestionResultAsync(string message)
    {
        var result = await _localSettingsService
              .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        if (result == null)
        {
            throw new Exception("配置为空");
        }

        chatGptClient ??= await chatGpt.CreateClient(new ChatGptClientConfig
        {
            SessionToken = result.ChatGPTSessionKey,
            AccountType = AccountType.Free
        });
        var conversationId = "a-unique-string-id";
        return await chatGptClient.Ask(message, conversationId);
    }
}
