using ElectronBot.Braincase;
using Microsoft.UI.Xaml.Controls;
using ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hw75Views;
public sealed partial class TodoView : UserControl
{

    public Hw75DynamicViewModel ViewModel
    {
        get;
    }
    public TodoView()
    {
        this.InitializeComponent();

        ViewModel = App.GetService<Hw75DynamicViewModel>();
    }

    private void UserControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}
