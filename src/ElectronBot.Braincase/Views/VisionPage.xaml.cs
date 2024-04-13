using ElectronBot.Braincase;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Services.ElectronBot;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class VisionPage : Page
{

    public VisionViewModel ViewModel
    {
        get;
    }
    public VisionPage()
    {
        ViewModel = App.GetService<VisionViewModel>();

        this.InitializeComponent();


    }

    private async void CameraPreviewControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await CameraPreviewControl.StartAsync();

        VisionService.Current.CameraHelper = CameraPreviewControl.CameraHelper;
        CameraPreviewControl.CameraHelper.FrameArrived += VisionService.Current.CameraHelper_FrameArrived;

        CameraPreviewControl.PreviewCameraChanged += CameraPreviewControl_PreviewCameraChanged;
    }

    private void CameraPreviewControl_PreviewCameraChanged(object? sender, CommunityToolkit.WinUI.Controls.PreviewCameraChangedEventArgs e)
    {
        CameraPreviewControl.MediaPlayer.PlaybackSession.PlaybackRotation = Windows.Media.MediaProperties.MediaRotation.Clockwise90Degrees;
    }
}
