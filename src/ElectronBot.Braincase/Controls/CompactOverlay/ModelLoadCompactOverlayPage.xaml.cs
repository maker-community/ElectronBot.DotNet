using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Services;
using Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Controls.CompactOverlay;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ModelLoadCompactOverlayPage : Page
{
    public ModelLoadCompactOverlayViewModel ViewModel
    {
        get;
    }
    public ModelLoadCompactOverlayPage()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ModelLoadCompactOverlayViewModel>();
    }

    private void ModelLoadCompactOverlayPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        ViewModel.Loaded();
        ElectronBotHelper.Instance.IsEntityFirstEnabled = false;
    }

    private void ModelLoadCompactOverlayPage_OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.UnLoaded();
    }
}
