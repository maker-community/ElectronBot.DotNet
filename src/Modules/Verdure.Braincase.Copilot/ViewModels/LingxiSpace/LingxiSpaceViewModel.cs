using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.Braincase.Core.Models.Lingxi.Filters;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class LingxiSpaceViewModel : ObservableRecipient, IRecipient<Conversation>, IRecipient<LingxiSpace>
{
    private readonly ILingxiSpaceService _lingxiSpaceService;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly DispatcherQueue _dispatcherQueue;
    public LingxiSpaceViewModel(ILingxiSpaceService lingxiSpaceService,
        ILocalSettingsService localSettingsService)
    {
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        _lingxiSpaceService = lingxiSpaceService;
        WeakReferenceMessenger.Default.Register<Conversation>(this);
        WeakReferenceMessenger.Default.Register<LingxiSpace>(this);
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
            .ReadSettingAsync<Conversation>(Constants.CurrentConversationKey);
        var lingxiSpaceList = await _lingxiSpaceService.GetAllAsync(new Core.Models.Lingxi.Filters.LingxiSpaceFilter
        {
            ConversationId = saveConvId?.Id ?? string.Empty
        });


        foreach (var space in lingxiSpaceList)
        {
            var spaceVm = new LingxiSpaceItemViewModel(space, null, null);
            LingxiSpaceList.Add(spaceVm);
        }

        CheckSpaceEmpty();
        RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
    }

    private void CheckSpaceEmpty()
        => IsLingxiEmpty = LingxiSpaceList?.Count == 0;

    public void Receive(Conversation conv)
    {
        _dispatcherQueue.TryEnqueue(async () =>
        {
            LingxiSpaceList.Clear();
            var lingxiSpaceList = await _lingxiSpaceService.GetAllAsync(new LingxiSpaceFilter
            {
                ConversationId = conv.Id
            });

            foreach (var space in lingxiSpaceList)
            {
                var spaceVm = new LingxiSpaceItemViewModel(space, null, null);
                LingxiSpaceList.Add(spaceVm);
            }

            CheckSpaceEmpty();
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        });

    }

    public void Receive(LingxiSpace space)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            var spaceVm = new LingxiSpaceItemViewModel(space, null, null);
            LingxiSpaceList.Add(spaceVm);
            CheckSpaceEmpty();
            RequestScrollToBottom?.Invoke(this, EventArgs.Empty);
        });
    }
}
