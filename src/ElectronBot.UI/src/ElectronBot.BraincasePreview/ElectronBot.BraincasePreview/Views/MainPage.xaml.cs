using ElectronBot.BraincasePreview.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ElectronBot.BraincasePreview.Views;

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
