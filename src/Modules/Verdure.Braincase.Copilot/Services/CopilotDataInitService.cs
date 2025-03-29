using System.Linq;
using System.Threading.Tasks;
using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Plugins.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Users;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Abstraction.Users.Models;
using BotSharp.Abstraction.Utilities;
using BotSharp.Core.Plugins;
using CommunityToolkit.Mvvm.DependencyInjection;
using ElectronBot.Copilot.Enums;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models;

namespace Verdure.Braincase.Copilot.Services;
public class CopilotDataInitService : IDataInitService
{
    private readonly IAgentService _agentService;
    private readonly IServiceProvider _services;
    private readonly IUserIdentity _userIdentity;
    private readonly IUserService _userService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IConversationService _conversationService;
    public CopilotDataInitService(IServiceProvider services,
        IAgentService agentService,
        IUserIdentity userIdentity,
        IUserService userService,
        ILocalSettingsService localSettingsService,
        IConversationService conversationService)
    {
        _services = services;
        _agentService = agentService;
        _userIdentity = userIdentity;
        _userService = userService;
        _localSettingsService = localSettingsService;
        _conversationService = conversationService;
    }
    public async Task InitializeDataAsync()
    {
        var user = await _userService.GetUser(_userIdentity.Id);

        if (user == null)
        {
            await _userService.CreateUser(new User
            {
                Id = _userIdentity.Id,
                Email = _userIdentity.Email,
                UserName = _userIdentity.UserName,
                FirstName = _userIdentity.FirstName,
                LastName = _userIdentity.LastName,
                Role = UserRole.Admin,
                Type = UserType.Client,
            });
        }

        var agents = await _agentService.GetAgents(new AgentFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 100
            }
        });
        if (!agents.Items.Any())
        {
            _ = await _agentService.RefreshAgents();

            var loader = Ioc.Default.GetRequiredService<PluginLoader>();
            var plugins = loader.GetPagedPlugins(_services, new PluginFilter
            {
                Pager = new Pagination
                {
                    Page = 1,
                    Size = 100
                }
            }).Items.ToList();

            foreach (var plugin in plugins)
            {
                _ = loader.UpdatePluginStatus(_services, plugin.Id, true);
            }
            await _localSettingsService.SaveSettingAsync(Constants.DefaultChatBotNameKey, new ComboxItemModel());
        }

        var saveConv = await _localSettingsService
                   .ReadSettingAsync<Conversation>(Constants.CurrentConversationKey);

        if (saveConv == null)
        {
            var result = await _conversationService.NewConversation(new Conversation
            {
                AgentId = VerdureAgentId.VerdureId,
                UserId = _userIdentity.Id
            });

            if (result != null)
            {
                await _localSettingsService.SaveSettingAsync(Constants.CurrentConversationKey, result);
            }
        }

    }
}
