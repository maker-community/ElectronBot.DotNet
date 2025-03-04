using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
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
using Verdure.ElectronBot.Core.Models;

namespace Verdure.Braincase.Copilot.Plugin.Functions;

public class CustomGenerateImageFn : IFunctionCallback
{
    public string Name => "custom_generate_image";

    private readonly IServiceProvider _service;
    private readonly IBotToolService _botToolService;
    private readonly IBotSpeech _botSpeech;
    private readonly JsonSerializerOptions _options;
    private readonly ILingxiSpaceService _lingxiSpaceService;
    private readonly IConversationService _conversationService;
    public CustomGenerateImageFn(IServiceProvider service,
        IBotToolService botToolService,
        ILingxiSpaceService lingxiSpaceService,
        IConversationService conversationService,
        IBotSpeech botSpeech)
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
        _botSpeech = botSpeech;
    }

    public async Task<bool> Execute(RoleDialogModel message)
    {
        var args = JsonSerializer.Deserialize<CustomGenerateImageFunctionArgs>(message.FunctionArgs ?? "", _options) ?? new CustomGenerateImageFunctionArgs();

        message.StopCompletion = true;

        var clientFactory = _service.GetRequiredService<IHttpClientFactory>();
        using var httpClient = clientFactory.CreateClient();
        var llmProviderService = _service.GetRequiredService<ILlmProviderService>();
        var model = llmProviderService.GetSetting("openai", "wanx-v1");
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

        await _botSpeech.SpeakAsync("正在生成图片，请稍等片刻");

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
                var url = taskResponse?.Output.Results.FirstOrDefault()?.Url;

                if (string.IsNullOrEmpty(url))
                {
                    return false;
                }

                // 下载图片并转换为Base64
                var imageBytes = await httpClient.GetByteArrayAsync(url);
                var base64Image = Convert.ToBase64String(imageBytes);

                var generateImageContent = new GenerateImageContent
                {
                    Name = args.ImageName,
                    Description = args.ImageDescription,
                    ImageData = $"data:{MediaTypeNames.Image.Png};base64,{base64Image}"
                };
                var lingxiSpace = await _lingxiSpaceService.AddAsync(new LingxiSpace
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = _conversationService.ConversationId,
                    Content = JsonSerializer.SerializeToDocument(generateImageContent, _options),
                    Name = args.ImageName,
                    Desc = args.ImageDescription,
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
