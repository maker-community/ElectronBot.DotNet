
using ChatGPTSharp;
using Contracts.Services;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;

namespace Services;
public class ChatGPTChatbotCustomClient : IChatbotClient
{
    public string Name => "ChatGPT-Custom";

    private readonly ILocalSettingsService _localSettingsService;

    private ChatGPTClient? _chatGptClient;
    public ChatGPTChatbotCustomClient(ILocalSettingsService localSettingsService)
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

        _chatGptClient.Settings.OpenAIAPIBaseUri = result.OpenAIBaseUrl;

        var msg = await _chatGptClient.SendMessage(message);

        return msg.Response ?? "";
    }
}
