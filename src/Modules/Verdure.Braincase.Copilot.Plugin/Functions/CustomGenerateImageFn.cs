using System.Text.Encodings.Web;
using System.Text.Json;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.Messaging;
using Verdure.Braincase.Copilot.Plugin.Models;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.ElectronBot.Core.Contracts.Services;

namespace Verdure.Braincase.Copilot.Plugin.Functions;

public class CustomGenerateImageFn : IFunctionCallback
{
    public string Name => "custom_generate_image";

    private readonly IServiceProvider _service;
    private readonly IBotToolService _botToolService;
    private readonly JsonSerializerOptions _options;
    private readonly ILingxiSpaceService _lingxiSpaceService;
    private readonly IConversationService _conversationService;
    public CustomGenerateImageFn(IServiceProvider service,
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
        var args = JsonSerializer.Deserialize<CustomGenerateImageFunctionArgs>(message.FunctionArgs ?? "", _options) ?? new CustomGenerateImageFunctionArgs();

        message.StopCompletion = true;

        var lingxiSpace = await _lingxiSpaceService.AddAsync(new LingxiSpace
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = _conversationService.ConversationId,
            //Content = JsonSerializer.SerializeToDocument(wordContent, _options),
            Name = args.ImageName,
            Desc = "https://dashscope-result-sh.oss-cn-shanghai.aliyuncs.com/1d/67/20241124/9d34e27a/c2fb7b01-44af-4abd-9929-d520d6c06e39-1.png?Expires=1732515865&OSSAccessKeyId=LTAI5tQZd8AEcZX6KZV4G8qL&Signature=z8%2FYLCgTbFbwDZdVVQapFMgQVzY%3D",//args.ImageDescription,
            Type = LingxiSpaceType.Image,
            CreatedTime = DateTime.UtcNow
        });

        WeakReferenceMessenger.Default.Send(lingxiSpace);
        return true;
    }
}
