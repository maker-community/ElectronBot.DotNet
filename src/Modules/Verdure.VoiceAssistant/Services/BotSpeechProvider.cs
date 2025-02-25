using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.WinUI.Common;

namespace Verdure.VoiceAssistant.Services;
public class BotSpeechProvider
{
    public static async Task<IBotSpeech> GetBotSpeechAsync(IServiceProvider services, string? provider = null)
    {
        var botSpeechs = services.GetServices<IBotSpeech>();

        var localSetting = services.GetRequiredService<ILocalSettingsService>();

        var llmVoice = await localSetting.ReadSettingAsync<ComboxItemModel>(Constants.DefaultLlmVoiceNameKey);

        var botSpeech = botSpeechs.FirstOrDefault(x => x.Provider == llmVoice?.DataKey);

        if (botSpeech == null)
        {
            var logger = services.GetRequiredService<ILogger<BotSpeechProvider>>();
            logger.LogError($"Can't resolve botSpeech provider by {provider}");
            botSpeech = botSpeechs.First();
        }
        return botSpeech;
    }
}
