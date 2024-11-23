using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class LingxiSpaceViewModel : ObservableRecipient
{
    private readonly ILingxiSpaceService _lingxiSpaceService;
    public LingxiSpaceViewModel(ILingxiSpaceService lingxiSpaceService)
    {
        _lingxiSpaceService = lingxiSpaceService;
    }

    [ObservableProperty]
    private bool _isLingxiEmpty;

    [ObservableProperty]
    ObservableCollection<LingxiSpaceItemViewModel> _lingxiSpaceList = new();

    /// <summary>
    /// 请求滚动到底部.
    /// </summary>
    public event EventHandler RequestScrollToBottom;

    [RelayCommand]
    public async Task OnLoaded()
    {
        var lingxiSpaceList = await _lingxiSpaceService.GetAllAsync(new Core.Models.Lingxi.Filters.LingxiSpaceFilter
        {
            ConversationId = null
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
}
