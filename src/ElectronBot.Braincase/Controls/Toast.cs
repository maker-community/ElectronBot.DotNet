using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.Controls;
public class Toast : Control
{
    // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(string), typeof(Toast), new PropertyMetadata(0));

    // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(Toast),
            new PropertyMetadata(TimeSpan.FromSeconds(2.0)));

    public Toast(string content)
    {
        DefaultStyleKey = typeof(Toast);
        Content = content;
        Width = App.MainWindow.Bounds.Width;
        Height = App.MainWindow.Bounds.Height;
        Transitions = new TransitionCollection
            {
                new EntranceThemeTransition()
            };
        App.MainWindow.SizeChanged += Current_SizeChanged;
    }

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public string Content
    {
        get => (string)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
    {
        Width = App.MainWindow.Bounds.Width;
        Height = App.MainWindow.Bounds.Height;
    }

    public async void Show()
    {
        var popup = new Popup
        {
            IsOpen = true,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        popup.Child = this;

        //popup.XamlRoot = ;

        await Task.Delay(Duration);

        popup.Child = null;
        popup.IsOpen = false;
        App.MainWindow.SizeChanged -= Current_SizeChanged;
    }
}