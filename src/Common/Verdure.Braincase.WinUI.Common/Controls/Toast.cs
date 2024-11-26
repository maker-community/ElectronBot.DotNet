using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using Verdure.Braincase.WinUI.Common.Contracts.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.Controls;
public partial class Toast : Control
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
        Width = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Width;
        Height = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Height;
        Transitions = new TransitionCollection
            {
                new EntranceThemeTransition()
            };
        Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().SizeChanged += Current_SizeChanged;
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
        Width = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Width;
        Height = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Height;
    }

    public async void Show()
    {
        var popup = new Popup
        {
            IsOpen = true,
            XamlRoot = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Content.XamlRoot
        };

        popup.Child = this;

        //popup.XamlRoot = ;

        await Task.Delay(Duration);

        popup.Child = null;
        popup.IsOpen = false;
        Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().SizeChanged -= Current_SizeChanged;
    }
}