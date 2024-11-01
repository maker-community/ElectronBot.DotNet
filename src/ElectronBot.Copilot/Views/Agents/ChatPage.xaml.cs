using BotSharp.Abstraction.Conversations.Models;
using CommunityToolkit.Mvvm.DependencyInjection;
using Verdure.Braincase.Copilot.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.Copilot.Views.Agents;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ChatPage : Page
{
    public ChatPage()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<ChatViewModel>();
    }

    public ChatViewModel ViewModel => (ChatViewModel)DataContext;

    private void ConvList_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is Conversation conv)
        {
            ViewModel.ConvSelectCommand.Execute(conv);
        }

    }
}
