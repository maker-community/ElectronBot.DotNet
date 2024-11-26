using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.HelloWordKeyboard.Hw75Views;

public sealed partial class Hw75YellowCalendarView : UserControl
{
    public Hw75YellowCalendarViewModel ViewModel
    {
        get;
    }
    public Hw75YellowCalendarView()
    {
        this.InitializeComponent();
        ViewModel = Ioc.Default.GetRequiredService<Hw75YellowCalendarViewModel>();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnUnLoaded();
    }
}
