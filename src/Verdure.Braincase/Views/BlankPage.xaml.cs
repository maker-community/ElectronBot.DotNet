using Verdure.Braincase.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Verdure.Braincase.Views;

public sealed partial class BlankPage : Page
{
    public BlankViewModel ViewModel
    {
        get;
    }

    public BlankPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<BlankViewModel>();
        InitializeComponent();
    }
}
