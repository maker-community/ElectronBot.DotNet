using Verdure.ElectronBot.Core.Models;

namespace Verdure.ElectronBot.Core.Contracts.Services;
public interface IBotToolService
{
    /// <summary>
    /// 发送天气到Bot
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> SendWeatherToBotAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送单词到Bot
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendWordsToBotAsync(LearnWordsContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送B站粉丝到Bot
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendBliFansToBotAsync(CancellationToken cancellationToken = default);
}
