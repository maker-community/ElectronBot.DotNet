using ElectronBot.Braincase.ClockViews;
using ElectronBot.Braincase.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Core;

namespace ElectronBot.Braincase.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();

        InitializeComponent();
    }
}
