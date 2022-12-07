using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using ElectronBot.BraincasePreview.Helpers;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Microsoft.UI.Xaml.Media.Imaging;
using OpenCvSharp.Extensions;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;

namespace ElectronBot.BraincasePreview.ViewModels;

public partial class GestureClassificationViewModel : ObservableRecipient
{

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

        Bitmap bitmap = BitmapConverter.ToBitmap(matData);

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
}
