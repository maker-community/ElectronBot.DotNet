using ElectronBot.Braincase.ViewModels;

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
}
