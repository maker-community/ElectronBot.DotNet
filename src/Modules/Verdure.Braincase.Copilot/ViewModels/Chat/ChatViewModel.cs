using System.Collections.Specialized;
using System.Linq;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Users;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;


namespace Verdure.Braincase.Copilot.ViewModels;

public partial class ChatViewModel : ObservableRecipient
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
        MessageList.CollectionChanged += OnMessageCountChanged;
    }

    private void OnMessageCountChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        }

        CheckChatEmpty();
        CheckLastMessageTime();
    }

    private void CheckChatEmpty()
    => IsChatEmpty = MessageList?.Count == 0;

    private void CheckLastMessageTime()
    {
        var lastMsg = MessageList.LastOrDefault();
        LastMessageTime = lastMsg is not null ? lastMsg.TimeStr ?? string.Empty : string.Empty;
    }
}
