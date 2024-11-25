using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.MLTasks;
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

        var clientFactory = _service.GetRequiredService<IHttpClientFactory>();
        using var httpClient = clientFactory.CreateClient();
        var llmProviderService = _service.GetRequiredService<ILlmProviderService>();
        var model = llmProviderService.GetSetting("tongyi", "wanx-v1");
        if (model == null)
        {
            return false;
        }
        var request = new GenerateImageRequest
        {
            Model = "wanx-v1",
            Input = new GenerateImageInput
            {
                Prompt = args.ImageDescription
            },
            Parameters = new GenerateImageParameters
            {
                Style = "<auto>",
                Size = "1024*1024",
                N = 1
            }
        };
        var generateImageUrl = $"{model.Endpoint.TrimEnd('/')}/services/aigc/text2image/image-synthesis";

        // 添加认证头部请求头
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", model.ApiKey);
        httpClient.DefaultRequestHeaders.Add("X-DashScope-Async", "enable");
        var result = await httpClient.PostAsJsonAsync(generateImageUrl, request);
        if (!result.IsSuccessStatusCode)
        {
            return false;
        }
        var taskContent = await result.Content.ReadAsStringAsync();
        var resultData = JsonSerializer.Deserialize<GenerateImageResponse>(taskContent, _options);

        var taskUrl = $"{model.Endpoint.TrimEnd('/')}/tasks/{resultData?.Output.TaskId}";

        var maxRetries = 5;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            var taskResult = await httpClient.GetAsync(taskUrl);
            if (!taskResult.IsSuccessStatusCode)
            {
                return false;
            }
            var taskResultContent = await taskResult.Content.ReadAsStringAsync();
            var taskResponse = JsonSerializer.Deserialize<ImageTaskResponse>(taskResultContent, _options);
            if (taskResponse?.Output.TaskStatus == "SUCCEEDED")
            {
                var lingxiSpace = await _lingxiSpaceService.AddAsync(new LingxiSpace
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = _conversationService.ConversationId,
                    //Content = JsonSerializer.SerializeToDocument(wordContent, _options),
                    Name = args.ImageName,
                    Desc = taskResponse?.Output.Results.FirstOrDefault()?.Url,
                    Type = LingxiSpaceType.Image,
                    CreatedTime = DateTime.UtcNow
                });

                WeakReferenceMessenger.Default.Send(lingxiSpace);
                break;
            }
            await Task.Delay(10000); // 等待5秒后再次轮询
            retryCount++;
        }
        return retryCount < maxRetries;
    }
}
