using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Services;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Services
{
    public class PoseRecognitionService
    {

        private int _isSending;

        private PoseCpuSolution? _calculator;

        private bool _isProcessing = false;


        public async Task<PoseOutput?> PosePredictResultUnUseQueueAsync(
            PoseCpuSolution? calculator,
            SoftwareBitmap? softwareBitmap, Stream? stream1 = null, CancellationToken cancellationToken = default)
        {
            if (_isProcessing)
            {
                Debug.WriteLine("A frame already processing in the HandPredictResultUnUseQueueAsync");
                return null;
            }

            _calculator = calculator;

            try
            {
                if (softwareBitmap != null)
                {
                    _isProcessing = true;

                    using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                    // Set the software bitmap
                    encoder.SetSoftwareBitmap(softwareBitmap);

                    await encoder.FlushAsync();

                    using var image = new Bitmap(stream.AsStream());

                    var matData = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                    var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                    var dataMeta = mat2.Data;

                    var length = mat2.Width * mat2.Height * mat2.Channels();

                    var data = new byte[length];

                    Marshal.Copy(dataMeta, data, 0, length);

                    var widthStep = (int)mat2.Step();

                    using var imgFrame = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                    var handsOutput = _calculator!.Compute(imgFrame);

                    if (handsOutput.PoseLandmarks != null)
                    {
                        CameraFrameService.Current.NotifyPosePredictResult(handsOutput);
                    }
                    else
                    {
                        Debug.WriteLine("No hand landmarks");
                    }
                    _isProcessing = false;
                }
                else
                {
                    if (stream1 != null)
                    {
                        _isProcessing = true;
                        using var image = new Bitmap(stream1);

                        var matData = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                        var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                        var dataMeta = mat2.Data;

                        var length = mat2.Width * mat2.Height * mat2.Channels();

                        var data = new byte[length];

                        Marshal.Copy(dataMeta, data, 0, length);

                        var widthStep = (int)mat2.Step();

                        using var imgFrame = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                        var handsOutput = _calculator!.Compute(imgFrame);

                        if (handsOutput.PoseLandmarks != null)
                        {
                            CameraFrameService.Current.NotifyPosePredictResult(handsOutput);
                        }
                        else
                        {
                            Debug.WriteLine("No hand landmarks");
                        }
                        _isProcessing = false;
                    }
                }
            }
            catch (Exception)
            {
                _isProcessing = false;
            }

            return null;
        }

    }
}
