using System.Collections.Generic;
using System.Linq;
using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Users;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Abstraction.Users.Models;
using BotSharp.Abstraction.Utilities;
using BotSharp.Core.Plugins;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using ElectronBot.Copilot.Enums;

namespace Verdure.Braincase.Copilot.ViewModels;

public partial class AgentViewModel : ObservableRecipient, INavigationAware
{
    private readonly IConversationService _conversationService;
    private readonly IUserIdentity _userIdentity;
    private readonly IUserService _userService;
    private readonly IServiceProvider _services;
    public AgentViewModel(IConversationService conversationService, IUserIdentity userIdentity, IUserService userService, IServiceProvider services)
    {
        _conversationService = conversationService;
        _userIdentity = userIdentity;
        _userService = userService;
        _services = services;
    }

    [ObservableProperty]
    List<Agent> _agents = new();

    public void OnNavigatedFrom()
    {

    }
    public async void OnNavigatedTo(object parameter)
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

        var loader = Ioc.Default.GetRequiredService<PluginLoader>();
        var plugins = loader.GetPagedPlugins(_services, new BotSharp.Abstraction.Plugins.Models.PluginFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 100
            }
        }).Items.ToList();

        var agentService = Ioc.Default.GetRequiredService<IAgentService>();

        var resultData = await agentService.RefreshAgents();


        foreach (var plugin in plugins)
        {
            _ = loader.UpdatePluginStatus(_services, plugin.Id, true);
        }

        var result = await _conversationService.NewConversation(new BotSharp.Abstraction.Conversations.Models.Conversation
        {
            AgentId = VerdureAgentId.VerdureId,
            UserId = _userIdentity.Id
        });

        Agents = (await agentService.GetAgents(new AgentFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 10
            }
        })).Items.ToList();
    }
}
