using System.Diagnostics;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls;
using ElectronBot.BraincasePreview.Contracts.ViewModels;
using ElectronBot.BraincasePreview.Controls;
using ElectronBot.BraincasePreview.Helpers;
using ElectronBot.BraincasePreview.Services;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using OpenCvSharp.Extensions;
using Services;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Image = Microsoft.UI.Xaml.Controls.Image;

namespace ElectronBot.BraincasePreview.ViewModels;

public partial class GestureClassificationViewModel : ObservableRecipient, INavigationAware
{
    private bool _isInitialized = false;
    private static HandsCpuSolution? calculator;

    private readonly string modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\MLModel1.zip";
    public GestureClassificationViewModel()
    {
        calculator = new HandsCpuSolution();
    }


    [RelayCommand]
    private async Task TestGestureClassficationAsync()
    {
        var handResult = string.Empty;
        var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\hand.png");

        var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

        var dataMeta = mat2.Data;

        var length = mat2.Width * mat2.Height * mat2.Channels();

        var data = new byte[length];

        Marshal.Copy(dataMeta, data, 0, length);

        var widthStep = (int)mat2.Step();

        System.Drawing.Bitmap bitmap = BitmapConverter.ToBitmap(matData);

        var ret = await BitmapToBitmapImage(bitmap);

        if (ret.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                ret.BitmapAlphaMode == BitmapAlphaMode.Straight)
        {
            ret = SoftwareBitmap.Convert(ret, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        }

        var imgframe = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

        var handsOutput = calculator.Compute(imgframe);

        if (handsOutput.MultiHandLandmarks != null)
        {
            var landmarks = handsOutput.MultiHandLandmarks[0].Landmark;

            Debug.WriteLine($"Got hands output with {landmarks.Count} landmarks");

            handResult = HandDataFormatHelper.PredictResult(landmarks.ToList(), modelPath);
        }
        else
        {
            Debug.WriteLine("No hand landmarks");
        }

        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            var source = new SoftwareBitmapSource();

            await source.SetBitmapAsync(ret);

            // Set the source of the Image control
            ImgFileSource = source;

            ResultLabel = handResult;
        });
    }

    [ObservableProperty]
    public Image faceImage = new();

    [ObservableProperty]
    public SoftwareBitmapSource faceImageSource;

    [ObservableProperty]
    string resultLabel;

    [ObservableProperty]
    SoftwareBitmapSource imgFileSource;
    public async Task<SoftwareBitmap> BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
    {
        MemoryStream ms = new MemoryStream();

        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        ms.Seek(0, SeekOrigin.Begin);

        // Create the decoder from the stream
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ms.AsRandomAccessStream());

        // Get the SoftwareBitmap representation of the file
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        return softwareBitmap;
    }

    public async void OnNavigatedTo(object parameter)
    {
        await InitAsync();
    }


    [RelayCommand]
    public async void RandomContentEdit()
    {
        try
        {
            var randomContentEditDialog = new ContentDialog()
            {
                Title = "AddRandomContentTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Content = new RandomContentPage()
            };

            await randomContentEditDialog.ShowAsync();
        }
        catch (Exception)
        {

        }
    }


    private async Task InitAsync()
    {
        if (_isInitialized)
        {
            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        else
        {
            await InitializeScreenAsync();
        }
    }

    private async Task InitializeScreenAsync()
    {
        await CameraFrameService.Current.PickNextMediaSourceWorkerAsync(FaceImage);

        CameraFrameService.Current.SoftwareBitmapFrameCaptured += Current_SoftwareBitmapFrameCaptured;

        CameraFrameService.Current.SoftwareBitmapFrameHandPredictResult += Current_SoftwareBitmapFrameHandPredictResult;

        _isInitialized = true;
    }

    private void Current_SoftwareBitmapFrameHandPredictResult(object? sender, string e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ResultLabel = e;
        });
    }

    private async void Current_SoftwareBitmapFrameCaptured(object? sender, SoftwareBitmapEventArgs e)
    {
        if (e.SoftwareBitmap is not null)
        {

            if (e.SoftwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                  e.SoftwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                e.SoftwareBitmap = SoftwareBitmap.Convert(
                    e.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            var service = App.GetService<GestureClassificationService>();

            _ = await service.HandPredictResultUnUseQueueAsync(calculator, modelPath, e.SoftwareBitmap);
        }
    }
    public async void OnNavigatedFrom()
    {
        await CleanUpAsync();
    }

    private async Task CleanUpAsync()
    {
        try
        {
            _isInitialized = false;

            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        catch (Exception)
        {

        }
    }
}
