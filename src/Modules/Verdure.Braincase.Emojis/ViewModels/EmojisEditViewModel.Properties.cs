namespace Verdure.Braincase.ViewModels;
public partial class EmojisEditViewModel : ObservableRecipient
{
    [ObservableProperty]
    private ObservableCollection<EmoticonAction> _actions = new();

    [ObservableProperty]
    private ObservableCollection<EmoticonActionUIModel> _emojis = new();

    /// <summary>
    /// 表情名称
    /// </summary>
    [ObservableProperty]
    private string _emojisName = string.Empty;
    /// <summary>
    /// 表情标识
    /// </summary>
    [ObservableProperty]
    private string _emojisNameId = string.Empty;

    /// <summary>
    /// 表情描述
    /// </summary>
    [ObservableProperty]
    private string _emojisDesc = string.Empty;

    /// <summary>
    /// 表情图片
    /// </summary>
    [ObservableProperty]
    private string _emojisAvatar = string.Empty;

    /// <summary>
    /// 表情视频存储地址
    /// </summary>
    [ObservableProperty]
    private string _emojisVideoUrl = string.Empty;

}
