using Verdure.Braincase.ViewModels;
using ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.EbScreen.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MiniModePage : Page
{
    public MiniModeViewModel ViewModel => (MiniModeViewModel)DataContext;
    public MiniModePage()
    {
        this.InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<MiniModeViewModel>();
    }
}
