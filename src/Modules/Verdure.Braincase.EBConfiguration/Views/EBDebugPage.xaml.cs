using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.EBConfiguration.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.EBConfiguration.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class EBDebugPage : Page
{
    public EBDebugViewModel ViewModel => (EBDebugViewModel)DataContext;
    public EBDebugPage()
    {
        this.InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<EBDebugViewModel>();
    }
}
