using ElectronBot.Braincase;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class VisionPage : Page
{

    public VisionViewModel ViewModel
    {
        get;
    }
    public VisionPage()
    {
        ViewModel = App.GetService<VisionViewModel>();

        this.InitializeComponent();
    }
}
