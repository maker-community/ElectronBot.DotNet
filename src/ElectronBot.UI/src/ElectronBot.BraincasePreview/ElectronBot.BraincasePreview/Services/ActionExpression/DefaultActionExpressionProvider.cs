using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Core.Models;
using ElectronBot.BraincasePreview.Helpers;
using OpenCvSharp;
using Windows.ApplicationModel;

namespace ElectronBot.BraincasePreview.Services;
public class DefaultActionExpressionProvider : IActionExpressionProvider
{
    public string Name => "Default";

    public Task PlayActionExpressionAsync(string actionName)
    {
        Mat image = new();

        var capture = new VideoCapture(Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{actionName}.mp4");

        while (true)
        {
            capture.Read(image);

            capture.Set(OpenCvSharp.VideoCaptureProperties.PosFrames,
                capture.Get(OpenCvSharp.VideoCaptureProperties.PosFrames) + 1);

            if (image.Empty())
            {
                break;
            }
            else
            {
                //var mat1 = image.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Lanczos4);

                //var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                var dataMeta = image.Data;

                var data = new byte[240 * 240 * 3];

                Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                EmojiPlayHelper.Current.Enqueue(new EmoticonActionFrame(data));
            }
        }

        return Task.CompletedTask;
    }
}
