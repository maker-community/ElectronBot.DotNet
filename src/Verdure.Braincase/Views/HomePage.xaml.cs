using Verdure.Braincase.ClockViews;
using Verdure.Braincase.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Core;
using Verdure.Braincase.Copilot.ViewModels;

namespace Verdure.Braincase.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public ChatViewModel ChatViewModel
    {
        get;
    }

    public LingxiSpaceViewModel LingxiSpaceViewModel
    {
        get;
    }
    public HomePage()
    {
        ViewModel = Ioc.Default.GetRequiredService<HomeViewModel>();
        ChatViewModel = Ioc.Default.GetRequiredService<ChatViewModel>();
        LingxiSpaceViewModel = Ioc.Default.GetRequiredService<LingxiSpaceViewModel>();
        InitializeComponent();
    }
}
