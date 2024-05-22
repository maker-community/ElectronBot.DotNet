using System.Net.Http.Json;
using System.Text.Json;
using Contracts.Services;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using Models;
using Sdcb.SparkDesk;

namespace Services;
public class SparkDeskChatbotClient : IChatbotClient
{
    public string Name => "SparkDesk";

    SparkDeskClient? client;

    private readonly ILocalSettingsService _localSettingsService;
    public SparkDeskChatbotClient(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }


    public async Task<string> AskQuestionResultAsync(string message)
    {

        var botSetting = await _localSettingsService.ReadSettingAsync<BotSetting>(Constants.BotSettingKey);
        if (botSetting == null)
        {
            throw new Exception("配置为空");
        }

        client ??= new SparkDeskClient(botSetting.SparkDeskAppId, botSetting.SparkDeskAPIKey, botSetting.SparkDeskAPISecret);

        try
        {
            ChatResponse response = await client.ChatAsync(ModelVersion.V3_5, new ChatMessage[]
         {
            ChatMessage.FromUser(message),

         }, new ChatRequestParameters
         {
             MaxTokens = 200,
             Temperature = 0.5f,
             TopK = 4,
         });
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(response.Text, TimeSpan.FromSeconds(10));
            });
            return response.Text;
        }
        catch (Exception ex)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(10));
            });
        }
        return "";
    }
}

