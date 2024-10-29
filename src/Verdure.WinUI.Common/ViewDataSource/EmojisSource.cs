using System.Threading;
using CommunityToolkit.WinUI.Collections;
using Contracts.Services;
using ElectronBot.Braincase.Models;

namespace Verdure.WinUI.Common.ViewDataSource;
public class EmojisSource : IIncrementalSource<EmoticonAction>
{
    private readonly IEmojisFileService _emojisFileService;
    public EmojisSource(IEmojisFileService emojisFileService)
    {
        _emojisFileService = emojisFileService;
    }

    public async Task<IEnumerable<EmoticonAction>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _emojisFileService.GetEmojisFileListAsync(pageIndex, pageSize);
    }
}