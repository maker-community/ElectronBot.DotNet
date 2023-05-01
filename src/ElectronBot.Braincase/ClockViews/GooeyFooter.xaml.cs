using System.Numerics;
using ElectronBot.Braincase.AnimationTimelines;
using ElectronBot.Braincase.ViewModels;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.ClockViews;
public sealed partial class GooeyFooter : UserControl
{
    private GaussianBlurEffect _blurEffect;
    private ICanvasBrush _brush;
    private readonly List<GooeyBubble> _bubbles;
    private Vector2 _centerPoint;
    private ICanvasImage _image;
    private DateTime _startTime;

    public ClockViewModel ViewModel
    {
        get;
    }

    public GooeyFooter()
    {
        InitializeComponent();
        ViewModel = App.GetService<ClockViewModel>();
        var easingFunction = new ExponentialEase { EasingMode = EasingMode.EaseInOut };
        _bubbles = new List<GooeyBubble>();
        var unit = 16;
        for (var i = 0; i < 168; i++)
        {
            var random = new Random();
            var seconds = 2 + random.NextDouble() * 2;
            var delay = TimeSpan.FromSeconds(2 + random.NextDouble() * 2);

            var offsetTimeline =
                new DoubleTimeline(-(6 + random.NextDouble() * 4) * unit, 10 * unit, seconds, delay, false);
            var sizeTimeline = new DoubleTimeline((2 + random.NextDouble() * 4) * unit, 0, seconds, delay, false);
            var x = random.NextDouble();
            _bubbles.Add(new GooeyBubble { X = x, OffsetTimeline = offsetTimeline, SizeTimeline = sizeTimeline });
        }
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _centerPoint = Canvas.ActualSize / 2;
    }

    private void OnCreateResource(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        _startTime = DateTime.Now;

        _brush = new CanvasSolidColorBrush(sender, Color.FromArgb(255, 169, 77, 193));

        _blurEffect = new GaussianBlurEffect
        {
            BlurAmount = 10f
        };

        _image = new ColorMatrixEffect
        {
            ColorMatrix = new Matrix5x4
            {
                M11 = 1,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = 1,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = 1,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 19,
                M51 = 0,
                M52 = 0,
                M53 = 0,
                M54 = -9
            },
            ClampOutput = true,
            Source = _blurEffect
        };
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        using var source = new CanvasCommandList(sender);

        var totalTime = DateTime.Now - _startTime;

        using var darwingSession = source.CreateDrawingSession();

        darwingSession.FillRectangle(-100, _centerPoint.Y, _centerPoint.X * 2 + 200, _centerPoint.Y + 100, _brush);

        foreach (var bubble in _bubbles)
        {
            var x = bubble.X * _centerPoint.X * 2;
            var y = _centerPoint.Y - bubble.OffsetTimeline.GetCurrentProgress(totalTime);
            var size = bubble.SizeTimeline.GetCurrentProgress(totalTime) / 4;
            darwingSession.FillCircle(new Vector2((float)x, (float)y), (float)size, _brush);
        }
        _blurEffect.Source = source;

        args.DrawingSession.DrawImage(_image);
        Canvas.Invalidate();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        _brush.Dispose();
        _blurEffect.Dispose();
    }
}
