using BotSharp.Abstraction.MLTasks;
using BotSharp.Abstraction.MLTasks.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BotSharp.Core.Infrastructures;

public class LocalSettingLlmProviderService : ILlmProviderService
{
    private readonly IServiceProvider _services;
    private readonly ILogger _logger;
    private readonly ILocalSettingsService _localSettingsService;

    public LocalSettingLlmProviderService(IServiceProvider services,
        ILogger<LocalSettingLlmProviderService> logger,
        ILocalSettingsService localSettingsService)
    {
        _services = services;
        _logger = logger;
        _localSettingsService = localSettingsService;
    }

    public List<string> GetProviders()
    {
        var providers = new List<string>();
        var services1 = _services.GetServices<ITextCompletion>();
        providers.AddRange(services1
            .Where(x => GetProviderModels(x.Provider).Any())
            .Select(x => x.Provider));

        var services2 = _services.GetServices<IChatCompletion>();
        providers.AddRange(services2
            .Where(x => GetProviderModels(x.Provider).Any())
            .Select(x => x.Provider));

        var services3 = _services.GetServices<ITextEmbedding>();
        providers.AddRange(services3
            .Where(x => GetProviderModels(x.Provider).Any())
            .Select(x => x.Provider));

        return providers.Distinct().ToList();
    }

    public List<LlmModelSetting> GetProviderModels(string provider)
    {
        var modelList = _localSettingsService.ReadSettingAsync<List<LlmProviderSetting>>(Constants.LlmProviders).Result;

        return modelList.FirstOrDefault(x => x.Provider.Equals(provider))?.Models ?? new List<LlmModelSetting>();
    }

    public LlmModelSetting GetProviderModel(string provider, string id, bool? multiModal = null, bool? realTime = false, bool imageGenerate = false)
    {
        var models = GetProviderModels(provider)
            .Where(x => x.Id == id);

        if (multiModal.HasValue)
        {
            models = models.Where(x => x.MultiModal == multiModal);
        }

        if (realTime.HasValue)
        {
            models = models.Where(x => x.RealTime == realTime);
        }

        models = models.Where(x => x.ImageGeneration == imageGenerate);

        var random = new Random();
        var index = random.Next(0, models.Count());
        var modelSetting = models.ElementAt(index);
        return modelSetting;
    }

    public LlmModelSetting? GetSetting(string provider, string model)
    {
        var settings = _localSettingsService.ReadSettingAsync<List<LlmProviderSetting>>(Constants.LlmProviders).Result;
        var providerSetting = settings.FirstOrDefault(p =>
            p.Provider.Equals(provider, StringComparison.CurrentCultureIgnoreCase));
        if (providerSetting == null)
        {
            _logger.LogError($"Can't find provider settings for {provider}");
            return null;
        }

        var modelSetting = providerSetting.Models.FirstOrDefault(m =>
            m.Name.Equals(model, StringComparison.CurrentCultureIgnoreCase));
        if (modelSetting == null)
        {
            _logger.LogError($"Can't find model settings for {provider}.{model}");
            return null;
        }

        // load balancing
        if (!string.IsNullOrEmpty(modelSetting.Group))
        {
            // find the models in the same group
            var models = providerSetting.Models
                .Where(m => !string.IsNullOrEmpty(m.Group) &&
                    m.Group.Equals(modelSetting.Group, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            // pick one model randomly
            var random = new Random();
            var index = random.Next(0, models.Count());
            modelSetting = models.ElementAt(index);
        }

        return modelSetting;
    }
}
