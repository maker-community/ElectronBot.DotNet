using System.Threading;
using CommunityToolkit.WinUI.Collections;
using Contracts.Services;
using Microsoft.UI.Xaml.Media.Imaging;
using Verdure.WinUI.Common.Models;

namespace Verdure.WinUI.Common.ViewDataSource;
public class EmojisSource : IIncrementalSource<EmoticonActionUIModel>
{
    private readonly IEmojisFileService _emojisFileService;
    public EmojisSource(IEmojisFileService emojisFileService)
    {
        _emojisFileService = emojisFileService;
    }

    public async Task<IEnumerable<EmoticonActionUIModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var emojis = await _emojisFileService.GetEmojisFileListAsync(pageIndex, pageSize);
        var retData = new List<EmoticonActionUIModel>();

        foreach (var item in emojis)
        {
            var data = new EmoticonActionUIModel
            {
                Name = item.Name,
                NameId = item.NameId,
                Desc = item.Desc,
                EmojisActionJson = item.EmojisActionJson,
                EmojisActionPath = item.EmojisActionPath,
                EmojisAuthor = item.EmojisAuthor,
                EmojisVideoPath = item.EmojisVideoPath,
                HasAction = item.HasAction,
                Type = item.Type
            };

            if (item.Avatar != null)
            {
                var bitmapImage = new BitmapImage();

                await bitmapImage.SetSourceAsync(item.Avatar.AsRandomAccessStream());

                data.Avatar = bitmapImage;
            }

            retData.Add(data);
        }

        return retData;
    }
}