using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Models;
using BotSharp.Abstraction.Repositories.Filters;
using BotSharp.Abstraction.Routing;
using BotSharp.Abstraction.Users.Enums;
using BotSharp.Abstraction.Utilities;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Copilot.Enums;
using Windows.ApplicationModel.DataTransfer;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class ChatViewModel
{
    [RelayCommand]
    public Task ConvSelectAsync(Conversation? conv)
    {
        if (conv == null) return Task.CompletedTask;
        _conversationService.SetConversationId(conv.Id, new List<MessageState>());
        var history = _conversationService.GetDialogHistory(fromBreakpoint: false);

        ChatMessageList = new ObservableCollection<RoleDialogModel>(history);

        foreach (var item in history)
        {
            var msgItem = new ChatMessageItemViewModel(item, null, null);
            MessageList.Add(msgItem);
        }
        SelectedConversation = conv;
        //RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    [RelayCommand]
    public Task ConvViewModelSelectAsync(ConversationViewModel? conv)
    {
        if (conv == null) return Task.CompletedTask;
        _conversationService.SetConversationId(conv.Id, new List<MessageState>());
        var history = _conversationService.GetDialogHistory(fromBreakpoint: false);

        //ChatMessageList = new ObservableCollection<RoleDialogModel>(history);
        MessageList.Clear();
        foreach (var item in history)
        {
            var msgItem = new ChatMessageItemViewModel(item, null, null);
            MessageList.Add(msgItem);
        }
        SelectedConv = conv;
        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }


    [RelayCommand]
    public async Task SendChatAsync()
    {
        if (SelectedConv == null) return;

        if (string.IsNullOrEmpty(SendText)) return;

        var inputMsg = new RoleDialogModel(AgentRole.User, SendText)
        {
            MessageId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };

        IsResponding = true;

        var msgItem = new ChatMessageItemViewModel(inputMsg, null, null);
        MessageList.Add(msgItem);

        ChatMessageList.Add(inputMsg);

        //RequestScrollToBottom?.Invoke(this, EventArgs.Empty);

        var routing = _services.GetRequiredService<IRoutingService>();
        routing.Context.SetMessageId(SelectedConv.Id, inputMsg.MessageId);

        _conversationService.SetConversationId(SelectedConv.Id, new());

        SendText = string.Empty;

        await Task.Run(async () =>
        {
            await _conversationService.SendMessage(SelectedConv.AgentId, inputMsg,
                replyMessage: null,
                async msg =>
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        var msgItem = new ChatMessageItemViewModel(msg, null, null);
                        MessageList.Add(msgItem);
                        ChatMessageList.Add(msg);
                        IsResponding = false;
                        //RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
                    });
                });
        });

    }

    [RelayCommand]
    public Task StartChatAsync(string? sendText)
    {
        if (string.IsNullOrEmpty(sendText)) return Task.CompletedTask;
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task NewSessionAsync()
    {
        var result = await _conversationService.NewConversation(new Conversation
        {
            AgentId = VerdureAgentId.VerdureId,
            UserId = _userIdentity.Id
        });

        _conversationService.SetConversationId(result.Id, new List<MessageState>());

        if (result != null)
        {
            var conv = new ConversationViewModel(result, null, null);
            ConversationList.Insert(0, result);
            ConvList.Add(conv);
            SelectedConversation = result;
            SelectedConv = conv;

            await ConvViewModelSelectAsync(conv);
        }
    }


    [RelayCommand]
    public async Task OnLoaded()
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

        //var agentService = Ioc.Default.GetRequiredService<IAgentService>();

        //Agents = (await agentService.GetAgents(new AgentFilter
        //{
        //    Pager = new Pagination
        //    {
        //        Page = 1,
        //        Size = 200
        //    }
        //})).Items.ToList();

        var convList = (await _conversationService.GetConversations(new ConversationFilter
        {
            Pager = new Pagination
            {
                Page = 1,
                Size = 200
            }
        })).Items.ToList();

        foreach (var conv in convList)
        {
            var convVm = new ConversationViewModel(conv, null, null);
            ConvList.Add(convVm);
        }

        SelectedConversation = convList.FirstOrDefault();

        SelectedConv = new ConversationViewModel(convList.FirstOrDefault() ?? new Conversation(), null, null);
        //ConversationList = new ObservableCollection<Conversation>(convList);

        if (SelectedConv == null) return;
        _conversationService.SetConversationId(SelectedConv.Id, new List<MessageState>());
        var historys = _conversationService.GetDialogHistory(fromBreakpoint: false);
        foreach (var history in historys)
        {
            var msgItem = new ChatMessageItemViewModel(history, null, null);
            MessageList.Add(msgItem);
            ChatMessageList.Add(history);
        }
        CheckChatEmpty();
        CheckLastMessageTime();
        //RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
    }




    [RelayCommand]
    private void Copy()
    {
        var dp = new DataPackage();
        //dp.SetText(Content);
        Clipboard.SetContent(dp);
    }

    [RelayCommand]
    private Task EditAsync()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void Delete()
    {
    }
}
