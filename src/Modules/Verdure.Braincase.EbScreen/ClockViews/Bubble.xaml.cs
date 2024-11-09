using System.Numerics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Composition;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Verdure.Braincase.ClockViews;

public sealed partial class Bubble : UserControl
{
    public Bubble()
    {
        InitializeComponent();

        BottomRectangles.Add(Rectangle1);
        BottomRectangles.Add(Rectangle2);
        Loaded += OnLoaded;
    }

    private List<Rectangle> BottomRectangles { get; } = new List<Rectangle>();

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var compositor = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Compositor;

        var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        var linear = compositor.CreateLinearEasingFunction();
        rotationAnimation.InsertKeyFrame(1.0f, 360, linear);
        rotationAnimation.Duration = TimeSpan.FromSeconds(9);
        rotationAnimation.Target = nameof(Rectangle1.Rotation);
        rotationAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
        Rectangle1.CenterPoint = new Vector3((float)Rectangle1.Width / 2, (float)Rectangle1.Height / 2, 0);
        Rectangle2.CenterPoint = new Vector3((float)Rectangle2.Width / 2, (float)Rectangle2.Height / 2, 0);

        Rectangle1.StartAnimation(rotationAnimation);

        rotationAnimation.Duration = TimeSpan.FromSeconds(8);

        Rectangle2.StartAnimation(rotationAnimation);
    }



    //public bool Stop
    //{
    //    get { return (bool)GetValue(MyPropertyProperty); }
    //    set { SetValue(MyPropertyProperty, value); }
    //}

    //// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty MyPropertyProperty =
    //    DependencyProperty.Register("Stop", typeof(bool), typeof(Bubble), new PropertyMetadata(0,OnContentChanged));

    //private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    if((bool)e.NewValue==true)
    //    {
    //       Instance.Rectangle1.StopAnimation(Instance.rotationAnimation);

    //        Instance.rotationAnimation.Duration = TimeSpan.FromSeconds(8);

    //        Instance.Rectangle2.StopAnimation(Instance.rotationAnimation);
    //    }
    //}
}
