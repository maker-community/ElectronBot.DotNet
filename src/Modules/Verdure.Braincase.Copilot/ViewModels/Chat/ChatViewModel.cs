using System.Collections.Specialized;
using System.Linq;
using BotSharp.Abstraction.Conversations;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Users;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models.Lingxi;


namespace Verdure.Braincase.Copilot.ViewModels;

public partial class ChatViewModel : ObservableRecipient, IRecipient<RoleDialogModel>
{
    private readonly IConversationService _conversationService;
    private readonly IUserIdentity _userIdentity;
    private readonly IUserService _userService;
    private readonly IServiceProvider _services;
    private readonly DispatcherQueue _dispatcherQueue;

    private readonly ILocalSettingsService _localSettingsService;
    public ChatViewModel(IConversationService conversationService,
        IUserIdentity userIdentity,
        IUserService userService, IServiceProvider services, ILocalSettingsService localSettingsService)
    {
        _conversationService = conversationService;
        _userIdentity = userIdentity;
        _userService = userService;
        _services = services;
        WeakReferenceMessenger.Default.Register<RoleDialogModel>(this);
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        MessageList.CollectionChanged += OnMessageCountChanged;
        _localSettingsService = localSettingsService;
    }

    public void Receive(RoleDialogModel message)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var msgItem = new ChatMessageItemViewModel(message, null, null);
            MessageList.Add(msgItem);
        });
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
