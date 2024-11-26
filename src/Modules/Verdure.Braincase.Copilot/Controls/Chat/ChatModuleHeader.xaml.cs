using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BotSharp.Abstraction.Conversations.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Verdure.Braincase.Copilot.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.Copilot.Controls.Chat;
public sealed partial class ChatModuleHeader : ChatModuleControl
{
    public ChatModuleHeader()
    {
        this.InitializeComponent();
    }

    private void ConvList_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is ConversationViewModel conv)
        {
            ViewModel.ConvViewModelSelectCommand.Execute(conv);
        }
    }
}
