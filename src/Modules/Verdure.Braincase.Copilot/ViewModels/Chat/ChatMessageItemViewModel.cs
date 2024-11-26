using System.Threading.Tasks;
using BotSharp.Abstraction.Agents.Enums;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Verdure.Braincase.Copilot.ViewModels;

public sealed partial class ChatMessageItemViewModel : ObservableRecipient
{
    private readonly Func<RoleDialogModel, Task> _editFunc;
    private readonly Func<RoleDialogModel, Task> _deleteFunc;

    private readonly RoleDialogModel Data;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isUser;

    [ObservableProperty]
    private bool _isAssistant;

    [ObservableProperty]
    private string _timeStr;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _author;

    [ObservableProperty]
    private string _agentId;
    public ChatMessageItemViewModel(
        RoleDialogModel message,
        Func<RoleDialogModel, Task> editFunc,
        Func<RoleDialogModel, Task> deleteFunc)
    {
        Data = message;
        AgentId = message.CurrentAgentId ?? string.Empty;
        Author = message.Role ?? string.Empty;
        Content = message.Content;
        IsAssistant = message.Role == AgentRole.Assistant;
        IsUser = message.Role == AgentRole.User;
        TimeStr = message.CreatedAt.ToLocalTime().ToString("MM/dd HH:mm:ss");
        _editFunc = editFunc;
        _deleteFunc = deleteFunc;
    }

    [RelayCommand]
    private void Copy()
    {
        var dp = new DataPackage();

    }

    [RelayCommand]
    private async Task EditAsync()
    {
        if (string.IsNullOrEmpty(Content))
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
