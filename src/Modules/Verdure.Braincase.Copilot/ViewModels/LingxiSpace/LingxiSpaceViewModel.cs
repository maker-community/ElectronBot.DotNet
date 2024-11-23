using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class LingxiSpaceViewModel : ObservableRecipient, IRecipient<Conversation>
{
    private readonly ILingxiSpaceService _lingxiSpaceService;
    private readonly ILocalSettingsService _localSettingsService;
    public LingxiSpaceViewModel(ILingxiSpaceService lingxiSpaceService, 
        ILocalSettingsService localSettingsService)
    {
        _lingxiSpaceService = lingxiSpaceService;
        WeakReferenceMessenger.Default.Register<Conversation>(this);
        _localSettingsService = localSettingsService;
    }

    [ObservableProperty]
    private bool _isLingxiEmpty;

    [ObservableProperty]
    ObservableCollection<LingxiSpaceItemViewModel> _lingxiSpaceList = new();

    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler? RequestScrollToBottom;

    [RelayCommand]
    public async Task OnLoaded()
    {
        var saveConvId = await _localSettingsService
            .ReadSettingAsync<string>(Constants.CurrentConversationKey);
        var lingxiSpaceList = await _lingxiSpaceService.GetAllAsync(new Core.Models.Lingxi.Filters.LingxiSpaceFilter
        {
            ConversationId = saveConvId ?? string.Empty
        });


        foreach (var space in lingxiSpaceList)
        {
            var spaceVm = new LingxiSpaceItemViewModel(space, null, null);
            LingxiSpaceList.Add(spaceVm);
        }

        CheckChatEmpty();
        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
    }

    private void CheckChatEmpty()
        => IsLingxiEmpty = LingxiSpaceList?.Count == 0;
    public async void Receive(Conversation conv)
    {
        var lingxiSpaceList = await _lingxiSpaceService.GetAllAsync(new Core.Models.Lingxi.Filters.LingxiSpaceFilter
        {
            ConversationId = conv.Id
        });

        foreach (var space in lingxiSpaceList)
        {
            var spaceVm = new LingxiSpaceItemViewModel(space, null, null);
            LingxiSpaceList.Add(spaceVm);
        }

        CheckChatEmpty();
        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
    }
}
