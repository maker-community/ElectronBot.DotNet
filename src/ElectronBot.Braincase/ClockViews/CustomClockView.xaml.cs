using System;
using System.Numerics;
using CommunityToolkit.WinUI.UI;
using ElectronBot.Braincase.ViewModels;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Windows.UI;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.ClockViews;
public sealed partial class CustomClockView : UserControl
{

    const float defaultDpi = 96;
    CanvasRenderTarget glassSurface;
    CanvasBitmap imgbackground;
    GaussianBlurEffect blurEffect;
    RainyDay rainday;
    float scalefactor;
    float imgW;
    float imgH;
    float imgX;
    float imgY;

    public ClockViewModel ViewModel
    {
        get;
    }

    public CustomClockView()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ClockViewModel>();
    }

    private async void Canvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        await PrepareRaindayAsync(sender);
    }

    private async Task PrepareRaindayAsync(CanvasControl sender)
    {
        var imgPath = Path.Combine(AppContext.BaseDirectory, $"Assets/Images/CustomViewDefault.jpg");

        var blurAmount = 4.0f;

        if (ViewModel.ClockTitleConfig != null)
        {
            if (!string.IsNullOrEmpty(ViewModel.ClockTitleConfig.CustomViewPicturePath))
            {
                imgPath = ViewModel.ClockTitleConfig.CustomViewPicturePath;
            }

            blurAmount = ViewModel.ClockTitleConfig.GaussianBlurValue;

            if (!ViewModel.ClockTitleConfig.CustomViewContentIsVisibility)
            {
                PomodoroPanel.Visibility = Visibility.Collapsed;
            }
        }

        imgbackground = await CanvasBitmap.LoadAsync(sender, imgPath);

        blurEffect = new GaussianBlurEffect()
        {
            Source = imgbackground,
            BlurAmount = blurAmount,
            BorderMode = EffectBorderMode.Soft
        };
        scalefactor =  (float)Math.Min(sender.Size.Width / imgbackground.Size.Width, sender.Size.Height / imgbackground.Size.Height);
        imgW = (float)imgbackground.Size.Width * scalefactor;
        imgH = (float)imgbackground.Size.Height * scalefactor;
        imgX = (float)(sender.Size.Width - imgW) / 2;
        imgY = (float)(sender.Size.Height - imgH) / 2;
        glassSurface = new CanvasRenderTarget(sender, imgW, imgH, defaultDpi);

        List<List<float>> pesets;


        rainday = new RainyDay(sender, imgW, imgH, imgbackground)
        {
            ImgSclaeFactor = scalefactor,
            GravityAngle = (float)Math.PI / 2
        };
        pesets = new List<List<float>>() {

            new List<float> { 3, 3, 0.88f },
            new List<float> { 5, 5, 0.9f },
            new List<float> { 6, 2, 1 }
        };

        rainday.Rain(pesets, 100);
    }

    private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (imgbackground != null)
        {
            args.DrawingSession.DrawImage(blurEffect, new Rect(imgX, imgY, imgW, imgH), new Rect(0, 0, imgbackground.Size.Width, imgbackground.Size.Height));
            args.DrawingSession.DrawImage(glassSurface, imgX, imgY);

            using var ds = glassSurface.CreateDrawingSession();
            rainday.UpdateDrops(ds);

        }
        canvas.Invalidate();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        canvas.RemoveFromVisualTree();
        canvas = null;
    }



    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        InitDemoData();
    }

    void InitDemoData()
    {
        this.DataContext = this;
    }
}
