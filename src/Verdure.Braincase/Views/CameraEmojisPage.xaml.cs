using Verdure.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CameraEmojisPage : Page
{
    public CameraEmojisViewModel ViewModel
    {
        get;
    }
    public CameraEmojisPage()
    {
        ViewModel = Ioc.Default.GetRequiredService<CameraEmojisViewModel>();

        InitializeComponent();
    }
}
