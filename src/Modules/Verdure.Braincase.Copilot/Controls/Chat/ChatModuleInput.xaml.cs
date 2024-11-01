// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.UI.Input;
using Windows.System;
using Windows.UI.Core;

namespace Verdure.Braincase.Copilot.Controls.Chat;
public sealed partial class ChatModuleInput : ChatModuleControl
{
    public ChatModuleInput()
    {
        this.InitializeComponent();
    }

    private async void OnInputBoxPreviewKeyDownAsync(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift);
            var isShiftDown = shiftState == CoreVirtualKeyStates.Down || shiftState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);
            var ctrlState = InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control);
            var isCtrlDown = ctrlState == CoreVirtualKeyStates.Down || ctrlState == (CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked);

            if ((ViewModel.IsEnterSend && !isShiftDown)
                || (!ViewModel.IsEnterSend && isCtrlDown))
            {
                e.Handled = true;
                await ViewModel.SendChatCommand.ExecuteAsync(default);
            }
        }
    }

    private void OnEnterSendItemClick(object sender, RoutedEventArgs e)
    {

    }

    private void OnCtrlEnterSendItemClick(object sender, RoutedEventArgs e)
    {

    }
}
