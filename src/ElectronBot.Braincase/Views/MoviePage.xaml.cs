using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ElectronBot.Braincase.Views;

public sealed partial class MoviePage : Page
{
    public MovieViewModel ViewModel
    {
        get;
    }

    public MoviePage()
    {
        ViewModel = App.GetService<MovieViewModel>();
        InitializeComponent();
    }


    private void ModelLoadCompactOverlayPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        ModelProgressRing.IsActive = true;
        ViewModel.Loaded();
        ModelProgressRing.IsActive = false;
    }

    private async void ModelLoadCompactOverlayPage_OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.UnLoaded();
    }
}
