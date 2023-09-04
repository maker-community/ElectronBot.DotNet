// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.Controls;
public sealed partial class GestureAppConfigItems : ItemsControl
{
    public static readonly DependencyProperty GestureLabelsProperty = DependencyProperty.Register(
        "GestureLabels",
        typeof(List<string>),
        typeof(GestureAppConfigItems),
        new PropertyMetadata(null, GestureLabelsPropertyChanged)
    );

    private static void GestureLabelsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
    }

    public List<string> GestureLabels
    {
        get => (List<string>)GetValue(GestureLabelsProperty);
        set => SetValue(GestureLabelsProperty, value);
    }

    #region
    public static readonly DependencyProperty DeleteItemCommandProperty = DependencyProperty.Register(
        "DeleteItemCommand",
        typeof(ICommand),
        typeof(GestureAppConfigItems),
        new PropertyMetadata(null, DeleteItemCommandPropertyChanged)
    );

    private static void DeleteItemCommandPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
    }

    public ICommand DeleteItemCommand
    {
        get => (ICommand)GetValue(DeleteItemCommandProperty);
        set => SetValue(DeleteItemCommandProperty, value);
    }

    /// <summary>
    /// fire event and command
    /// </summary>
    private void OnDeleteItem(string id)
    {
        if (DeleteItemCommand != null && DeleteItemCommand.CanExecute(id))
        {
            DeleteItemCommand.Execute(id);
        }
    }
    #endregion

    public GestureAppConfigItems()
    {
        InitializeComponent();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Button? button = sender as Button;

        if (button != null && button.Tag is string id)
        {
            OnDeleteItem(id);
        }
    }
}