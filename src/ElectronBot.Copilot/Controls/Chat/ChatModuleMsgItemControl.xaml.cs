// Copyright (c) Rodel. All rights reserved.

using BotSharp.Abstraction.Conversations.Models;
using Microsoft.UI.Xaml.Input;

namespace Verdure.Braincase.Copilot.Controls.Chat;

/// <summary>
/// 聊天消息.
/// </summary>
public sealed partial class ChatModuleMsgItemControl : UserControl
{
    public RoleDialogModel? ViewModel
    {
        get; set;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatMessageItemControl"/> class.
    /// </summary>
    public ChatModuleMsgItemControl() => InitializeComponent();

    private void OnEditorConfirmButtonClick(object sender, RoutedEventArgs e)
    {
        //var text = Editor.Text;
        ExitEditor();
        //ViewModel.EditCommand.Execute(default);
    }

    private void OnEditorCancelButtonClick(object sender, RoutedEventArgs e)
        => ExitEditor();

    private void ExitEditor()
    {
        //ViewModel.IsEditing = false;
        Editor.Text = string.Empty;
    }

    private void ShowTools()
    {
        //if (ViewModel.IsEditing || RootCard.ActualWidth < 90)
        //{
        //    return;
        //}

        //OptionsContainer.Visibility = Visibility.Visible;
        //var offset = MessageBackground.ActualWidth + Avatar.ActualWidth + 16;
        //var verticalOffset = TimeBlock.ActualHeight + 12;
        //if (RootCard.ActualWidth - offset < 90)
        //{
        //    offset = RootCard.ActualWidth - 90;
        //}

        //OptionsContainer.Margin = ViewModel.IsUser ? new Thickness(0, 0, offset, verticalOffset) : new Thickness(offset, 0, 0, verticalOffset);
    }

    private void HideTools()
        => OptionsContainer.Visibility = Visibility.Collapsed;

    private void OnCardPointerEntered(object sender, PointerRoutedEventArgs e)
        => ShowTools();

    private void OnCardPointerExited(object sender, PointerRoutedEventArgs e)
        => HideTools();

    private void OnEditButtonClick(object sender, RoutedEventArgs e)
    {
        //Editor.Text = ViewModel.Content;
        //ViewModel.IsEditing = true;
        HideTools();
    }

    private void OnCardPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        //if (OptionsContainer.Visibility == Visibility.Collapsed
        //    && !ViewModel.IsEditing)
        //{
        //    ShowTools();
        //}
    }

    private void UserControl_Loading(FrameworkElement sender, object args)
    {
        _ = ViewModel?.Role == "user"
             ? VisualStateManager.GoToState(this, nameof(MyState), false)
             : VisualStateManager.GoToState(this, nameof(AssistantState), false);
    }
}

