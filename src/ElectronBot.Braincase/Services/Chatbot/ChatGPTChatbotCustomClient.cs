using System.Net.Http.Headers;
using Contracts.Services;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Copilot;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Models;

namespace Services;
public class ChatGPTChatbotCustomClient : IChatbotClient
{
    public string Name => "ChatGPT-Custom";

    private readonly ILocalSettingsService _localSettingsService;

    private Kernel? _kernel;
    public ChatGPTChatbotCustomClient(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }
    public async Task<string> AskQuestionResultAsync(string message)
    {
        var result = await _localSettingsService.ReadSettingAsync<BotSetting>(Constants.BotSettingKey);
        if (result == null)
        {
            throw new Exception("配置为空");
        }


        var hasCustomEndpoint = !string.IsNullOrEmpty(result.OpenAIBaseUrl) && Uri.TryCreate(result.OpenAIBaseUrl, UriKind.Absolute, out var _);
        var customHttpClient = hasCustomEndpoint
            ? GetProxyClient(result.OpenAIBaseUrl)
            : default;

        _kernel ??= Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(result.ChatGPTVersion, result.ChatGPTSessionKey, httpClient: customHttpClient)
            .Build();

        var chat = _kernel.GetRequiredService<IChatCompletionService>()
            ?? throw new KernelException("not init chat");

        var resMessage = string.Empty;

        try
        {
            await Task.Run(async () =>
            {
                var history = new ChatHistory();
                history.AddMessage(AuthorRole.User, message);
                var response = chat.GetStreamingChatMessageContentsAsync(history);
                await foreach (var item in response)
                {
                    resMessage += item;
                    //streamHandler?.Invoke(resMessage);
                }
            });

            resMessage = resMessage.Trim();

            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(resMessage, TimeSpan.FromSeconds(10));
            });

            if (string.IsNullOrEmpty(resMessage))
            {
                throw new KernelException("chat is empty");
            }
        }
        catch (Exception ex)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(10));
            });
        }

        return resMessage;
    }

    private static HttpClient GetProxyClient(string baseUrl)
    {
        var httpClient = new HttpClient(new ProxyOpenAIHttpClientHandler(baseUrl));
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        return httpClient;
    }
}
