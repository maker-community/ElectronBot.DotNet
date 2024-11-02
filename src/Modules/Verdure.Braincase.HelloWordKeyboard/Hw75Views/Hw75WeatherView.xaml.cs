using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.HelloWordKeyboard.Hw75Views;

public sealed partial class Hw75WeatherView : UserControl
{
    public Hw75WeatherViewModel ViewModel
    {
        get;
    }
    public Hw75WeatherView()
    {
        this.InitializeComponent();

        ViewModel = Ioc.Default.GetRequiredService<Hw75WeatherViewModel>();
    }

    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OnUnLoaded();
    }
}
