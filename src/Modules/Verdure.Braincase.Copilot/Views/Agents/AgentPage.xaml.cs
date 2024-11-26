using CommunityToolkit.Mvvm.DependencyInjection;
using Verdure.Braincase.Copilot.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Verdure.Braincase.Copilot.Views;

public sealed partial class AgentPage : Page
{
    public AgentPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AgentViewModel>();
    }

    public AgentViewModel ViewModel => (AgentViewModel)DataContext;
}
