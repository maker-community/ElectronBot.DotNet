namespace Contracts.Services;
/// <summary>
/// 聊天机器人工厂
/// </summary>
public interface IChatbotClientFactory
{
    /// <summary>
    /// 创建聊天机器人客户端
    /// </summary>
    /// <param name="clientName"></param>
    /// <returns></returns>
    IChatbotClient CreateChatbotClient(string? clientName = null);
}
