using ElectronBot.Braincase.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ElectronBot.Braincase.Views;

public sealed partial class EmojisEditPage : Page
{
    public EmojisEditViewModel ViewModel
    {
        get;
    }

    public EmojisEditPage()
    {
        ViewModel = App.GetService<EmojisEditViewModel>();
        InitializeComponent();
    }
}
