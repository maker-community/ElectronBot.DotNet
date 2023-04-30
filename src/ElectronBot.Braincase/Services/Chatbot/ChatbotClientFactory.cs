using Contracts.Services;

namespace Services;
public class ChatbotClientFactory : IChatbotClientFactory
{
    private readonly Dictionary<string, IChatbotClient> _chatBots = new(StringComparer.Ordinal);
    public ChatbotClientFactory(IEnumerable<IChatbotClient> chatbots)
    {
        foreach (var chatbot in chatbots)
        {
            _chatBots.Add(chatbot.Name, chatbot);
        }
    }
    public IChatbotClient CreateChatbotClient(string? clientName = null)
    {
        if (clientName == null)
        {
            return _chatBots.FirstOrDefault().Value;
        }

        if (_chatBots.TryGetValue(clientName, out var client))
        {
            return client;
        }
        else
        {
            return _chatBots.FirstOrDefault().Value;
        }
    }
}
