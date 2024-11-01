using Verdure.Braincase.ClockViews;
using Verdure.Braincase.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Core;

namespace Verdure.Braincase.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

        InitializeComponent();
    }
}
