using System.Text.Encodings.Web;
using System.Text.Json;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.Messaging;
using Verdure.Braincase.Copilot.Plugin.Models;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Models;

namespace Verdure.Braincase.Copilot.Plugin.Functions;

public class LearnWordsFn : IFunctionCallback
{
    public string Name => "learn_words";

    private readonly IServiceProvider _service;
    private readonly IBotToolService _botToolService;
    private readonly JsonSerializerOptions _options;
    private readonly ILingxiSpaceService _lingxiSpaceService;
    private readonly IConversationService _conversationService;
    public LearnWordsFn(IServiceProvider service,
        IBotToolService botToolService,
        ILingxiSpaceService lingxiSpaceService,
        IConversationService conversationService)
    {
        _service = service;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowTrailingCommas = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        _botToolService = botToolService;
        _lingxiSpaceService = lingxiSpaceService;
        _conversationService = conversationService;
    }

    public async Task<bool> Execute(RoleDialogModel message)
    {
        var args = JsonSerializer.Deserialize<LearnWordsFunctionArgs>(message.FunctionArgs ?? "", _options) ?? new LearnWordsFunctionArgs();

        var wordContent = new LearnWordsContent
        {
            Word = args.Word,
            WordDescription = args.WordDescription,
        };
        await _botToolService.SendWordsToBotAsync(wordContent);

        var strBuilder = new StringBuilder();
        strBuilder.AppendLine($"µ•¥ √˚◊÷£∫{args.Word}");
        strBuilder.AppendLine($"µ•¥ ΩÈ…‹£∫{args.WordDescription}");
        message.Content = strBuilder.ToString();
        message.StopCompletion = true;

        var lingxiSpace = await _lingxiSpaceService.AddAsync(new LingxiSpace
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = _conversationService.ConversationId,
            Content = JsonSerializer.SerializeToDocument(wordContent, _options),
            Name = args.Word,
            Desc = args.WordDescription,
            Type = LingxiSpaceType.Word,
            CreatedTime = DateTime.UtcNow
        });

        WeakReferenceMessenger.Default.Send(lingxiSpace);
        return true;
    }
}
