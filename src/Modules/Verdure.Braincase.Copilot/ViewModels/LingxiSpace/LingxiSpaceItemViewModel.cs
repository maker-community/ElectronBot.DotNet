using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Verdure.Braincase.Core.Models.Lingxi;

namespace Verdure.Braincase.Copilot.ViewModels;
public partial class LingxiSpaceItemViewModel : ObservableRecipient
{
    private readonly Func<LingxiSpace, Task> _editFunc;
    private readonly Func<LingxiSpace, Task> _deleteFunc;

    private readonly LingxiSpace Data;

    public LingxiSpaceItemViewModel(LingxiSpace space,
        Func<LingxiSpace, Task> editFunc,
        Func<LingxiSpace, Task> deleteFunc)
    {
        Data = space;
        Id = space.Id;
        Name = space.Name;
        Desc = space.Desc;
        ImageData = space.Type == LingxiSpaceType.Image ? space.Content?.RootElement.GetProperty("imageData").GetString() : "";
        CreatedTime = space.CreatedTime;
        ConversationId = space.ConversationId;
        _editFunc = editFunc;
        _deleteFunc = deleteFunc;
    }
}
