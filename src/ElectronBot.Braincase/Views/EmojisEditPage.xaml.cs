using Verdure.Braincase.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Verdure.Braincase.Views;

public sealed partial class EmojisEditPage : Page
{
    public EmojisEditViewModel ViewModel
    {
        get;
    }

    public EmojisEditPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<EmojisEditViewModel>();
        InitializeComponent();
    }
}
