using System;
using System.Numerics;
using CommunityToolkit.WinUI;
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
using Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.ClockViews;
public sealed partial class GrooveClockView : UserControl
{
    public ClockViewModel ViewModel
    {
        get;
    }

    public GrooveClockView()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ClockViewModel>();
        var spectrumAnalyzer = new SpectrumAnalyzer(canvas);

        spectrumAnalyzer.Start();
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
