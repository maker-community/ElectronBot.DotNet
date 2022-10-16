using ElectronBot.BraincasePreview.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ElectronBot.BraincasePreview.Views;

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
