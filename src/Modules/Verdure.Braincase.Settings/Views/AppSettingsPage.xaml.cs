using Microsoft.UI.Xaml.Media.Animation;
using Verdure.Braincase.Settings.ViewModels;
using Verdure.Braincase.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Verdure.Braincase.Settings.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AppSettingsPage : Page
{
    private int previousSelectedIndex;
    public AppSettingsViewModel ViewModel => (AppSettingsViewModel)DataContext;
    public AppSettingsPage()
    {
        this.InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AppSettingsViewModel>();
    }

    private void OnSettingSectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var selectedItem = sender.SelectedItem;
        var currentSelectedIndex = sender.Items.IndexOf(selectedItem);
        Type pageType = null;

        switch (currentSelectedIndex)
        {
            case 0:
                pageType = typeof(SettingsPage);
                break;
            case 1:
                pageType = typeof(DialogueSettingsPage);
                break;
            case 2:
                pageType = typeof(VoiceSettingsPage);
                break;
            case 3:
                pageType = typeof(DrawingSettingsPage);
                break;
            default:
                pageType = typeof(SettingsPage);
                break;
        }

        var slideNavigationTransitionEffect = currentSelectedIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

        SectionFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });

        previousSelectedIndex = currentSelectedIndex;

    }
}
