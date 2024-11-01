using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Conversations.Models;
using BotSharp.Abstraction.Models;
using BotSharp.Abstraction.Routing;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Copilot.Enums;
using Microsoft.Extensions.DependencyInjection;
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
        SelectedConversation = conv;
        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    [RelayCommand]
    public async Task SendChatAsync()
    {
        if (SelectedConversation == null) return;

        if (string.IsNullOrEmpty(SendText)) return;

        var inputMsg = new RoleDialogModel(AgentRole.User, SendText)
        {
            MessageId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };

        IsResponding = true;

        ChatMessageList.Add(inputMsg);

        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);

        var routing = _services.GetRequiredService<IRoutingService>();
        routing.Context.SetMessageId(SelectedConversation.Id, inputMsg.MessageId);

        _conversationService.SetConversationId(SelectedConversation.Id, new());

        SendText = string.Empty;

        await Task.Run(async () =>
        {
            await _conversationService.SendMessage(SelectedConversation.AgentId, inputMsg,
                replyMessage: null,
                async msg =>
                {
                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        ChatMessageList.Add(msg);
                        IsResponding = false;
                        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
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
            ConversationList.Insert(0, result);
            SelectedConversation = result;
        }
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
