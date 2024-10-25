using System.Text.Encodings.Web;
using System.Text.Json;
using BotSharp.Abstraction.Conversations.Models;
using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Models;
using Verdure.Plugin.Copilot.Models;

namespace Verdure.Plugin.Copilot.Functions;

public class LearnWordsFn : IFunctionCallback
{
    public string Name => "learn_words";

    private readonly IServiceProvider _service;
    private readonly IBotToolService _botToolService;
    private readonly JsonSerializerOptions _options;
    public LearnWordsFn(IServiceProvider service, IBotToolService botToolService)
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

        return true;
    }
}
