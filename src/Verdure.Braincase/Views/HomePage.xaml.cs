using Verdure.Braincase.ClockViews;
using Verdure.Braincase.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Core;

namespace Verdure.Braincase.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();

        InitializeComponent();
    }
}
