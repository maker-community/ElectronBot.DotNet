using System;
using System.Numerics;
using CommunityToolkit.WinUI;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.ClockViews;
public sealed partial class HiddenTextView : UserControl
{

    private PointLight _redLight;
    private PointLight _blueLight;
    private AmbientLight _backgroundLight;

    private Color _redColor = Color.FromArgb(255, 217, 17, 83);
    private Color _blueColor = Color.FromArgb(255, 0, 27, 171);

    private Color _lightRedColor = Color.FromArgb(255, 247, 97, 163);
    private Color _lightBlueColor = Color.FromArgb(255, 80, 107, 251);

    public ClockViewModel ViewModel
    {
        get;
    }

    public HiddenTextView()
    {
        this.InitializeComponent();
        //this.Loaded += HiddenTextView_Loaded;

        ViewModel = App.GetService<ClockViewModel>();

        ShowTextShimmingAsync();
        CreateBackgroundLight();
        ShowBackgroundLight();
    }

    //private void HiddenTextView_Loaded(object sender, RoutedEventArgs e)
    //{
    //    ShowTextShimmingAsync();
    //    CreateBackgroundLight();
    //    ShowBackgroundLight();
    //}

    private void ShowTextShimmingAsync()
    {
        _redLight = CreatePointLightAndStartAnimation(_redColor, TimeSpan.Zero);
        _blueLight = CreatePointLightAndStartAnimation(_blueColor, TimeSpan.FromSeconds(0.25));
        var focusVisual = VisualExtensions.GetVisual(FocusPanel);

        _redLight.Targets.Add(focusVisual);
        _blueLight.Targets.Add(focusVisual);
    }

    private void CreateBackgroundLight()
    {
        Visual hostVisual = ElementCompositionPreview.GetElementVisual(this);

        var compositor = hostVisual.Compositor;
        var focusVisual = VisualExtensions.GetVisual(FocusPanel);

        _backgroundLight = compositor.CreateAmbientLight();
        _backgroundLight.Color = _lightRedColor;
        _backgroundLight.Intensity = 0;
        _backgroundLight.Targets.Add(focusVisual);
    }

    private PointLight CreatePointLightAndStartAnimation(Color color, TimeSpan delay)
    {
        var width = 960;
        var height = 461;

        Visual hostVisual = ElementCompositionPreview.GetElementVisual(this);

        var compositor = hostVisual.Compositor;

        var rootVisual = VisualExtensions.GetVisual(Root);
        var pointLight = compositor.CreatePointLight();

        pointLight.Color = color;
        pointLight.CoordinateSpace = rootVisual;
        pointLight.Offset = new Vector3(-width * 4, height / 2, 75.0f);

        var offsetAnimation = compositor.CreateScalarKeyFrameAnimation();
        offsetAnimation.InsertKeyFrame(1.0f, width * 5, compositor.CreateLinearEasingFunction());
        offsetAnimation.Duration = TimeSpan.FromSeconds(10);
        offsetAnimation.DelayTime = delay;
        offsetAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

        pointLight.StartAnimation("Offset.X", offsetAnimation);
        return pointLight;
    }

    private void SwitchBackgroundLightColor()
    {
        if (_backgroundLight == null)
            return;

        Visual hostVisual = ElementCompositionPreview.GetElementVisual(this);

        var compositor = hostVisual.Compositor;

        var colorAnimation = compositor.CreateColorKeyFrameAnimation();
        colorAnimation.InsertKeyFrame(1.0f, Colors.Purple, compositor.CreateLinearEasingFunction());
        colorAnimation.Duration = TimeSpan.FromSeconds(1);
        _backgroundLight.StartAnimation(nameof(AmbientLight.Color), colorAnimation);
    }

    private void ShowBackgroundLight()
    {
        if (_backgroundLight == null)
            return;

        Visual hostVisual = ElementCompositionPreview.GetElementVisual(this);

        var compositor = hostVisual.Compositor;

        var scalarAnimation = compositor.CreateScalarKeyFrameAnimation();
        scalarAnimation.InsertKeyFrame(1.0f, 0.5f, compositor.CreateLinearEasingFunction());
        scalarAnimation.Duration = TimeSpan.FromSeconds(1);
        _backgroundLight.StartAnimation(nameof(AmbientLight.Intensity), scalarAnimation);
    }
}
