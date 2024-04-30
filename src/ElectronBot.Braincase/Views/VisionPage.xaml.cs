using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Services.ElectronBot;
using Vedure.Braincsse.WinUI.Helpers;
using Verdure.ElectronBot.Core.Models;
using Windows.Media.Capture.Frames;

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
        var availableFrameSourceGroups = await CameraHelper.GetFrameSourceGroupsAsync();
        if (availableFrameSourceGroups != null)
        {
            MediaFrameSourceGroup? camera = null;
            var setting = App.GetService<ILocalSettingsService>();
            var saveCamera = await setting.ReadSettingAsync<ComboxItemModel>(Constants.DefaultCameraNameKey);
            if (saveCamera != null)
            {
                camera = availableFrameSourceGroups.FirstOrDefault(x => x.DisplayName == saveCamera.DataValue);
            }
            else
            {
                camera = availableFrameSourceGroups.FirstOrDefault();
            }

            CameraHelper cameraHelper = new CameraHelper() { FrameSourceGroup = camera };

            await CameraPreviewControl.StartAsync(cameraHelper);

            if (camera != null && camera.DisplayName.EndsWith("Cam"))
            {
                CameraPreviewControl.MediaPlayer.PlaybackSession.PlaybackRotation = Windows.Media.MediaProperties.MediaRotation.Clockwise270Degrees;
            }

            VisionService.Current.CameraHelper = CameraPreviewControl.CameraHelper;
            CameraPreviewControl.CameraHelper.FrameArrived += VisionService.Current.CameraHelper_FrameArrived;

            CameraPreviewControl.PreviewCameraChanged += CameraPreviewControl_PreviewCameraChanged;
        }


    }

    private void CameraPreviewControl_PreviewCameraChanged(object? sender, CommunityToolkit.WinUI.Controls.PreviewCameraChangedEventArgs e)
    {
        if (e.CameraName != null && e.CameraName.EndsWith("Cam"))
        {
            CameraPreviewControl.MediaPlayer.PlaybackSession.PlaybackRotation = Windows.Media.MediaProperties.MediaRotation.Clockwise270Degrees;
        }
        else
        {
            CameraPreviewControl.MediaPlayer.PlaybackSession.PlaybackRotation = Windows.Media.MediaProperties.MediaRotation.None;
        }
    }
}
