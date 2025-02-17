using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.WinUI.Common.Models;
using Microsoft.UI.Xaml.Controls;

namespace Verdure.Braincase.EBConfiguration.ViewModels;
public partial class EBDebugViewModel : ObservableRecipient
{
    [ObservableProperty]
    int selectIndex;

    [ObservableProperty]
    int interval;

    /// <summary>
    /// 时钟选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel clockComBoxSelect;

    /// <summary>
    /// 表盘列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> clockComboxModels;

    /// <summary>
    /// 当前播放表情
    /// </summary>
    [ObservableProperty]
    ImageSource emojiImageSource;

    /// <summary>
    /// 表盘内容
    /// </summary>
    [ObservableProperty]
    UIElement element;

    [ObservableProperty]
    string resultLabel;


    [ObservableProperty]
    string _sendText;

    /// <summary>
    /// 头部舵机
    /// </summary>
    [ObservableProperty]
    float j1;

    /// <summary>
    /// 左臂展开
    /// </summary>
    [ObservableProperty]
    float j2;
    /// <summary>
    /// 左臂旋转
    /// </summary>
    [ObservableProperty]
    float j3;
    /// <summary>
    /// 右臂展开
    /// </summary>
    [ObservableProperty]
    float j4;
    /// <summary>
    /// 右臂旋转
    /// </summary>
    [ObservableProperty]
    float j5;
    /// <summary>
    /// 底盘转动
    /// </summary>
    [ObservableProperty]
    float j6;

    [ObservableProperty]
    public Image faceImage = new()
    {
        Source = new BitmapImage(new Uri("ms-appx:///Assets/LargeTile.scale-200.png"))
    };


    [ObservableProperty]
    ElectronBotAction selectdAction = new();

    [ObservableProperty]
    ObservableCollection<ElectronBotAction> actions = new();

}
