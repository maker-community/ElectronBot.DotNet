using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Agents.Enums;
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
using CommunityToolkit.Mvvm.Input;
using Verdure.Braincase.Copilot.Controls.Agents;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.WinUI.Common.Contracts.Services;

namespace Verdure.Braincase.Copilot.ViewModels;

public partial class AgentViewModel : ObservableRecipient, INavigationAware
{
    private readonly IConversationService _conversationService;
    private readonly IAgentService _agentService;
    private readonly IUserIdentity _userIdentity;
    private readonly IUserService _userService;
    private readonly IServiceProvider _services;
    private readonly ILocalSettingsService _localSettingsService;
    public AgentViewModel(IConversationService conversationService,
        IUserIdentity userIdentity,
        IUserService userService,
        IServiceProvider services,
        IAgentService agentService,
        ILocalSettingsService localSettingsService)
    {
        _conversationService = conversationService;
        _userIdentity = userIdentity;
        _userService = userService;
        _services = services;
        _agentService = agentService;
        _localSettingsService = localSettingsService;
    }

    [ObservableProperty]
    private List<Agent> _agents = new();

    [ObservableProperty]
    private Agent _selectedAgent;
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

        Agents = (await _agentService.GetAgents(new AgentFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 100
            }
        })).Items.ToList();
    }

    [RelayCommand]
    public async Task ResetAgentAsync()
    {
        _ = await _agentService.RefreshAgents();

        var loader = Ioc.Default.GetRequiredService<PluginLoader>();
        var plugins = loader.GetPagedPlugins(_services, new BotSharp.Abstraction.Plugins.Models.PluginFilter
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
        Agents = (await _agentService.GetAgents(new AgentFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 100
            }
        })).Items.ToList();
        await _localSettingsService.SaveSettingAsync(Constants.DefaultChatBotNameKey, new ComboxItemModel());
        ToastHelper.SendToast("Reset Agent OK", TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public async Task EditAgentAsync(Agent agent)
    {
        if (agent == null)
        {
            return;
        }

        SelectedAgent = agent;

        var theme = Ioc.Default.GetRequiredService<IThemeSelectorService>();


        // 创建编辑对话框
        var dialog = new ContentDialog
        {
            Title = "编辑智能体",
            PrimaryButtonText = "保存",
            CloseButtonText = "取消",
            DefaultButton = ContentDialogButton.Primary,
            Content = new AgentEditControl(SelectedAgent),
            XamlRoot = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Content.XamlRoot,
            RequestedTheme = theme.Theme
        };

        // 显示对话框并等待结果
        var result = await dialog.ShowAsync();

        // 处理用户点击保存的情况
        if (result == ContentDialogResult.Primary)
        {
            try
            {
                // 保存更改到服务
                await _agentService.UpdateAgent(SelectedAgent, AgentField.Instruction);

                await _agentService.UpdateAgent(SelectedAgent, AgentField.Name);

                await _agentService.UpdateAgent(SelectedAgent, AgentField.Description);

                // 刷新代理列表
                Agents = (await _agentService.GetAgents(new AgentFilter
                {
                    Pager = new Pagination
                    {
                        Page = 1,
                        Size = 100
                    }
                })).Items.ToList();

                ToastHelper.SendToast("智能体已更新", TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                ToastHelper.SendToast($"更新失败: {ex.Message}", TimeSpan.FromSeconds(5));
            }
        }
    }
}
