// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using System.Threading.Tasks;
using Verdure.Braincase.Copilot.ViewModels;

namespace Verdure.Braincase.Copilot.Controls.Chat;
public sealed partial class ChatModuleMsgList : ChatModuleControl
{
    public ChatModuleMsgList()
    {
        this.InitializeComponent();
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is ChatViewModel oldVm)
        {
            oldVm.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }

        if (e.NewValue is ChatViewModel newVm)
        {
            newVm.RequestScrollToBottom += OnRequestScrollToBottomAsync;
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (ViewModel is not null)
        {
            ViewModel.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        }
    }

    private async void OnRequestScrollToBottomAsync(object? sender, EventArgs e)
    {
        if (MessageViewer is not null)
        {
            await Task.Delay(500);
            MessageViewer.ChangeView(0, MessageViewer.ScrollableHeight + MessageViewer.ActualHeight + MessageViewer.VerticalOffset, default);
        }
    }
}
