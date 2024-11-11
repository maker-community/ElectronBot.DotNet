// Copyright (c) Rodel. All rights reserved.

using BotSharp.Abstraction.Conversations.Models;
using Microsoft.UI.Xaml.Input;

namespace Verdure.Braincase.Copilot.Controls.Chat;

/// <summary>
/// 聊天消息.
/// </summary>
public sealed partial class ChatModuleMsgItemControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(RoleDialogModel), typeof(ChatModuleMsgItemControl), new PropertyMetadata(null));

    public RoleDialogModel? ViewModel
    {
        get => (RoleDialogModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ChatModuleMsgItemControl() => InitializeComponent();

    private void OnEditorConfirmButtonClick(object sender, RoutedEventArgs e)
    {
        ExitEditor();
    }

    private void OnEditorCancelButtonClick(object sender, RoutedEventArgs e)
        => ExitEditor();

    private void ExitEditor()
    {
        Editor.Text = string.Empty;
    }

    private void ShowTools()
    {
    }

    private void HideTools()
        => OptionsContainer.Visibility = Visibility.Collapsed;

    private void OnCardPointerEntered(object sender, PointerRoutedEventArgs e)
        => ShowTools();

    private void OnCardPointerExited(object sender, PointerRoutedEventArgs e)
        => HideTools();

    private void OnEditButtonClick(object sender, RoutedEventArgs e)
    {
        HideTools();
    }

    private void OnCardPointerMoved(object sender, PointerRoutedEventArgs e)
    {
    }

    private void UserControl_Loading(FrameworkElement sender, object args)
    {
        _ = ViewModel?.Role == "user"
             ? VisualStateManager.GoToState(this, nameof(MyState), false)
             : VisualStateManager.GoToState(this, nameof(AssistantState), false);
    }
}

