using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElectronBot.Braincase.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ImageCropperPage : Page
{
    public ImageCropperPickerViewModel ViewModel
    {
        get;
    }
    public ImageCropperPage()
    {
        ViewModel = App.GetService<ImageCropperPickerViewModel>();

        DataContext = ViewModel;

        this.InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        using IRandomAccessStream stream = new InMemoryRandomAccessStream();

        await ImageCropper.SaveAsync(stream, CommunityToolkit.WinUI.Controls.BitmapFileFormat.Png);

        var writeableBitmap = await BitmapTools.GetCroppedBitmapAsync(stream, new Point(0, 0),ViewModel.AspectRatio ==1? new Size(240, 240): new Size(12, 296), 1);

        ViewModel?.SetResult(writeableBitmap);
    }

    private void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.Exit();
    }
}
