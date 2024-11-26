using Microsoft.UI.Xaml.Controls;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.HelloWordKeyboard.Hw75Views;

public sealed partial class Hw75CustomView : UserControl
{
    public Hw75CustomViewModel ViewModel
    {
        get;
    }
    public Hw75CustomView()
    {
        this.InitializeComponent();

        ViewModel = Ioc.Default.GetRequiredService<Hw75CustomViewModel>();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnUnLoaded();
    }
}
