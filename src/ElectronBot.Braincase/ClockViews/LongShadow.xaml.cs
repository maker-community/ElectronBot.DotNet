using System.Numerics;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.ClockViews;
public sealed partial class LongShadow : UserControl
{
    public ClockViewModel ViewModel
    {
        get;
    }
    public LongShadow()
    {
        this.InitializeComponent();

        ViewModel = App.GetService<ClockViewModel>();

        MakeLongShadow(188, 0.3f, InWorkCountDown, InworkBackground, Color.FromArgb(255, 250, 110, 93));
        MakeLongShadow(188, 0.3f, InWorkCountDownSecond, InworkSecondBackground, Color.FromArgb(255, 250, 110, 93));

        MakeLongShadow(188, 0.3f, CustomTitleTb, CustomTitleBackground, Color.FromArgb(255, 250, 110, 93));
        MakeLongShadow(188, 0.3f, Day, DayBackground, Color.FromArgb(255, 250, 110, 93));

        FlipSide.IsFlipped = false;
    }



    private void MakeLongShadow(int depth, float opacity, TextBlock textElement, FrameworkElement shadowElement, Color color)
    {
        var textVisual = ElementCompositionPreview.GetElementVisual(textElement);
        var compositor = textVisual.Compositor;
        var containerVisual = compositor.CreateContainerVisual();
        var mask = textElement.GetAlphaMask();
        Vector3 background = new Vector3(color.R, color.G, color.B);

        for (int i = 0; i < depth; i++)
        {
            var maskBrush = compositor.CreateMaskBrush();
            var shadowColor = background - ((background - new Vector3(0, 0, 0)) * opacity);
            shadowColor = Vector3.Max(Vector3.Zero, shadowColor);
            shadowColor += (background - shadowColor) * i / depth;
            maskBrush.Mask = mask;
            maskBrush.Source = compositor.CreateColorBrush(Color.FromArgb(255, (byte)shadowColor.X, (byte)shadowColor.Y, (byte)shadowColor.Z));
            var visual = compositor.CreateSpriteVisual();
            visual.Brush = maskBrush;
            visual.Offset = new Vector3(i + 1, i + 1, 0);
            var bindSizeAnimation = compositor.CreateExpressionAnimation("textVisual.Size");
            bindSizeAnimation.SetReferenceParameter("textVisual", textVisual);
            visual.StartAnimation("Size", bindSizeAnimation);

            containerVisual.Children.InsertAtBottom(visual);
        }

        ElementCompositionPreview.SetElementChildVisual(shadowElement, containerVisual);
    }
}
