// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ElectronBot.Braincase;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Controls.CompactOverlay;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class DefaultCompactOverlayPage : Page
{

    public TodoCompactOverlayViewModel ViewModel
    {
        get;
    }
    public DefaultCompactOverlayPage()
    {
        this.InitializeComponent();

        ViewModel = App.GetService<TodoCompactOverlayViewModel>();
    }
}
