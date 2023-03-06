using ChatGPT.Net;
using ChatGPT.Net.DTO;
using ChatGPT.Net.Session;
using Contracts.Services;
using ElectronBot.BraincasePreview;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Models;
using OpenAI.Net;
using OpenAI.Net.Completions;

namespace Services;
public class ChatGPTChatbotClient : IChatbotClient
{
    public string Name => "ChatGPT";

    private readonly ILocalSettingsService _localSettingsService;

    private readonly ChatGpt chatGpt;

    private ChatGptClient? chatGptClient;

    private VerdureOpenAIClient? openAIClient;

    private IHttpClientFactory _httpClientFactory;
    public ChatGPTChatbotClient(ILocalSettingsService localSettingsService,
        IHttpClientFactory httpClientFactory)
    {
        _localSettingsService = localSettingsService;
        _httpClientFactory = httpClientFactory;
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

        openAIClient ??= new VerdureOpenAIClient(_httpClientFactory, result.ChatGPTSessionKey);

        var completion = await openAIClient.CreateCompletionAsync(new CompletionRequest()
        {
            Prompt = new string[] { message },
            Model = "text-davinci-003",
            MaxTokens = 50,
            Temperature = 0,
            TopP = 1,
            N = 1
        });

        return completion.Choices.FirstOrDefault()?.Text ?? "";
    }
}
