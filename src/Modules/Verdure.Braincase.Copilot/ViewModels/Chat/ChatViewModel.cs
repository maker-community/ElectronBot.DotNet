using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using BotSharp.Abstraction.Agents;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Users;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Abstraction.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Dispatching;
using Verdure.Braincase.WinUI.Common.Contracts.ViewModels;


namespace Verdure.Braincase.Copilot.ViewModels;

public partial class ChatViewModel : ObservableRecipient, INavigationAware
{
    private readonly IConversationService _conversationService;
    private readonly IUserIdentity _userIdentity;
    private readonly IUserService _userService;
    private readonly IServiceProvider _services;
    private readonly DispatcherQueue _dispatcherQueue;
    public ChatViewModel(IConversationService conversationService,
        IUserIdentity userIdentity,
        IUserService userService, IServiceProvider services)
    {
        _conversationService = conversationService;
        _userIdentity = userIdentity;
        _userService = userService;
        _services = services;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        ChatMessageList.CollectionChanged += OnMessageCountChanged;
    }

    private void OnMessageCountChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }

        CheckChatEmpty();
        CheckLastMessageTime();
    }

    private void CheckChatEmpty()
    => IsChatEmpty = ChatMessageList.Count == 0;

    private void CheckLastMessageTime()
    {
        var lastMsg = ChatMessageList.LastOrDefault();
        LastMessageTime = lastMsg is not null ? lastMsg.CreatedAt.ToLocalTime().ToString("MM/dd") ?? string.Empty : string.Empty;
    }
    public void OnNavigatedFrom()
    {

    }
    public async void OnNavigatedTo(object parameter)
    {
        var user = await _userService.GetUser(_userIdentity.Id);

        if (user == null)
        {
            await _userService.CreateUser(new BotSharp.Abstraction.Users.Models.User
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

        var agentService = Ioc.Default.GetRequiredService<IAgentService>();

        Agents = (await agentService.GetAgents(new AgentFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 200
            }
        })).Items.ToList();

        var convList = (await _conversationService.GetConversations(new ConversationFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 200
            }
        })).Items.ToList();

        SelectedConversation = convList.FirstOrDefault();
        ConversationList = new ObservableCollection<Conversation>(convList);

        if (SelectedConversation == null) return;
        _conversationService.SetConversationId(SelectedConversation.Id, new List<MessageState>());
        var history = _conversationService.GetDialogHistory(fromBreakpoint: false);
        ChatMessageList = new ObservableCollection<RoleDialogModel>(history);

        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
    }
}
