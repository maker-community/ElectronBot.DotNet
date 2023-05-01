using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ElectronBot.Braincase.ViewModels;
using Verdure.ElectronBot.Core.Models;
using ElectronBot.Braincase.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace ElectronBot.Braincase.Controls;

public sealed partial class EmojisInfoContentDialog : ContentDialog
{
    public EmojisInfoDialogViewModel ViewModel
    {
        get;
    
    }
    public EmojisInfoContentDialog()
    {
        ViewModel = App.GetService<EmojisInfoDialogViewModel>();

        DataContext = ViewModel;

        InitializeComponent();
    }

    public EmoticonAction EmoticonAction
    {
        get; set;
    }


    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

    }
}
