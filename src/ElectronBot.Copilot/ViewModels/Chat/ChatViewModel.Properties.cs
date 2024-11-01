using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotSharp.Abstraction.Agents.Models;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class ChatViewModel
{

    [ObservableProperty]
    List<Agent> _agents = new();

    [ObservableProperty]
    ObservableCollection<Conversation> _conversationList = new();

    [ObservableProperty]
    ObservableCollection<RoleDialogModel> _chatMessageList = new();

    [ObservableProperty]
    Conversation? _selectedConversation;

    [ObservableProperty]
    string? _sendText;


    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isUser;

    [ObservableProperty]
    private bool _isAssistant;

    [ObservableProperty]
    private string _time;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _author;

    [ObservableProperty]
    private string _agentId;

    [ObservableProperty]
    private bool _isEnterSend = true;


    [ObservableProperty]
    private bool _isResponding;

    [ObservableProperty]
    private string _tempMessage;

    [ObservableProperty]
    private double _extraColumnWidth;

    [ObservableProperty]
    private bool _extraColumnVisible;

    [ObservableProperty]
    private double _extraRowHeight;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _model;

    [ObservableProperty]
    private string _userInput;

    [ObservableProperty]
    private string _errorText;


    [ObservableProperty]
    private bool _isChatEmpty;

    [ObservableProperty]
    private bool _isRegenerateButtonShown;

    [ObservableProperty]
    private string _lastMessageTime;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isSupportTool;

    [ObservableProperty]
    private bool _isSupportVision;

    [ObservableProperty]
    private int _maxRounds;

    [ObservableProperty]
    private bool _isSessionPreset;

    [ObservableProperty]
    private bool _isAgentPreset;

    [ObservableProperty]
    private bool _isNormalSession;

    [ObservableProperty]
    private string _generatingTipText;

    [ObservableProperty]
    private int _totalTokenUsage;

    [ObservableProperty]
    private int _remainderTokenCount;

    [ObservableProperty]
    private int _systemTokenCount;

    [ObservableProperty]
    private int _userInputWordCount;

    [ObservableProperty]
    private int _userInputTokenCount;

    [ObservableProperty]
    private int _totalTokenCount;
    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler RequestScrollToBottom;
}
