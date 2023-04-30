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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.Controls;
public sealed partial class ActionEdit : UserControl
{
    public ActionEdit()
    {
        InitializeComponent();
    }

    public event RangeBaseValueChangedEventHandler ValueChanged;
    public float J1
    {
        get => (float)GetValue(J1Property);
        set => SetValue(J1Property, value);
    }

    // Using a DependencyProperty as the backing store for J1.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty J1Property =
        DependencyProperty.Register("J1", typeof(float), typeof(ActionEdit), new PropertyMetadata(0));

    public float J2
    {
        get => (float)GetValue(J2Property);
        set => SetValue(J2Property, value);
    }

    // Using a DependencyProperty as the backing store for J2.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty J2Property =
        DependencyProperty.Register("J2", typeof(float), typeof(ActionEdit), new PropertyMetadata(0));


    public float J3
    {
        get => (float)GetValue(J3Property);
        set => SetValue(J3Property, value);
    }

    // Using a DependencyProperty as the backing store for J3.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty J3Property =
        DependencyProperty.Register("J3", typeof(float), typeof(ActionEdit), new PropertyMetadata(0));


    public float J4
    {
        get => (float)GetValue(J4Property);
        set => SetValue(J4Property, value);
    }

    // Using a DependencyProperty as the backing store for J4.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty J4Property =
        DependencyProperty.Register("J4", typeof(float), typeof(ActionEdit), new PropertyMetadata(0));



    public float J5
    {
        get => (float)GetValue(J5Property);
        set => SetValue(J5Property, value);
    }

    // Using a DependencyProperty as the backing store for J5.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty J5Property =
        DependencyProperty.Register("J5", typeof(float), typeof(ActionEdit), new PropertyMetadata(0));



    public float J6
    {
        get => (float)GetValue(J6Property);
        set => SetValue(J6Property, value);
    }

    // Using a DependencyProperty as the backing store for J6.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty J6Property =
        DependencyProperty.Register("J6", typeof(float), typeof(ActionEdit), new PropertyMetadata(0));

    private void Head_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }
}
