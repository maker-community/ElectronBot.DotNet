using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Windows.Graphics.Imaging;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CameraEmojisPage : Page
{
    public CameraEmojisViewModel ViewModel
    {
        get;
    }
    public CameraEmojisPage()
    {
        ViewModel = App.GetService<CameraEmojisViewModel>();

        InitializeComponent();
    }
}
