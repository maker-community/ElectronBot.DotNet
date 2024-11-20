using System.Threading.Tasks;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Verdure.Braincase.Copilot.ViewModels;

public sealed partial class ConversationViewModel : ObservableRecipient
{
    private readonly Func<Conversation, Task> _editFunc;
    private readonly Func<Conversation, Task> _deleteFunc;

    private readonly Conversation Data;

    [ObservableProperty]
    private string _id;

    [ObservableProperty]
    private string _agentId;
    
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private DateTime _createdTime;

    [ObservableProperty]
    private string _timeStr;
    public ConversationViewModel(
        Conversation conversation,
        Func<Conversation, Task> editFunc,
        Func<Conversation, Task> deleteFunc)
    {
        _editFunc = editFunc;
        _deleteFunc = deleteFunc;
        Id = conversation.Id;
        Title = conversation.Title;
        AgentId = conversation.AgentId;
        Data = conversation;
        CreatedTime = conversation.CreatedTime;
        TimeStr = conversation.CreatedTime.ToLocalTime().ToString("MM/dd HH:mm:ss");
    }

    [RelayCommand]
    private void Copy()
    {
        var dp = new DataPackage();

    }

    [RelayCommand]
    private async Task EditAsync()
    {
        if (string.IsNullOrEmpty(Title))
        {
            Delete();
            return;
        }
        await _editFunc?.Invoke(Data);
    }

    [RelayCommand]
    private void Delete()
        => _deleteFunc?.Invoke(Data);
}
