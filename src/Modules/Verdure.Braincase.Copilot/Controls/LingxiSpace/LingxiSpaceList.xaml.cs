// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.Copilot.Controls.LingxiSpace;
public sealed partial class LingxiSpaceList : LingxiSpaceControl
{
    public LingxiSpaceList()
    {
        this.InitializeComponent();
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc/>
    protected override void OnViewModelChanged(DependencyPropertyChangedEventArgs e)
    {
        //if (e.OldValue is LingxiSpaceViewModel oldVm)
        //{
        //    oldVm.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        //}

        //if (e.NewValue is LingxiSpaceViewModel newVm)
        //{
        //    newVm.RequestScrollToBottom += OnRequestScrollToBottomAsync;
        //}
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        //if (ViewModel is not null)
        //{
        //    ViewModel.RequestScrollToBottom -= OnRequestScrollToBottomAsync;
        //}
    }

    private async void OnRequestScrollToBottomAsync(object? sender, EventArgs e)
    {
        //if (MessageViewer is not null)
        //{
        //    await Task.Delay(500);
        //    MessageViewer.ChangeView(0, MessageViewer.ScrollableHeight + MessageViewer.ActualHeight + MessageViewer.VerticalOffset, default);
        //}
    }
}
