
using ChatGPTSharp;
using Contracts.Services;
using ElectronBot.BraincasePreview;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Models;

namespace Services;
public class ChatGPTChatbotClient : IChatbotClient
{
    public string Name => "ChatGPT";

    private readonly ILocalSettingsService _localSettingsService;

    private ChatGPTClient? _chatGptClient;
    public ChatGPTChatbotClient(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }
    public async Task<string> AskQuestionResultAsync(string message)
    {
        var result = await _localSettingsService
              .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        if (result == null)
        {
            throw new Exception("配置为空");
        }

        _chatGptClient ??= new ChatGPTClient(result.ChatGPTSessionKey, "gpt-3.5-turbo");

        var msg = await _chatGptClient.SendMessage(message);

        return msg.Response ?? "";
    }
}
