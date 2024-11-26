using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.HelloWordKeyboard.Hw75Views;
public sealed partial class TodoView : UserControl
{

    public Hw75DynamicViewModel ViewModel
    {
        get;
    }
    public TodoView()
    {
        this.InitializeComponent();

        ViewModel = Ioc.Default.GetRequiredService<Hw75DynamicViewModel>();
    }

    private void UserControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}
