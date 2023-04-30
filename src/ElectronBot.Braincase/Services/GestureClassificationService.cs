using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Services;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Services;
public class GestureClassificationService
{
    private readonly ConcurrentQueue<(SoftwareBitmap Data, TaskCompletionSource<string> Tcs, CancellationToken Ct)> _queue = new();

    private int _isSending;

    private HandsCpuSolution? _calculator;

    private string? _modelPath;

    private bool _isProcessing = false;

    public async Task<string> HandPredictResultAsync(HandsCpuSolution? calculator, string modelPath, SoftwareBitmap softwareBitmap, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _calculator = calculator;
        _modelPath = modelPath;

        var tcs = new TaskCompletionSource<string>();

        _queue.Enqueue((softwareBitmap, tcs, cancellationToken));

        if (Interlocked.CompareExchange(ref _isSending, 1, 0) == 0)
        {
            _ = Task.Run(SendDataAsync, CancellationToken.None);
        }

        return await tcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> HandPredictResultUnUseQueueAsync(HandsCpuSolution? calculator, string modelPath, SoftwareBitmap softwareBitmap, CancellationToken cancellationToken = default)
    {
        var ret = "";

        if (_isProcessing)
        {
            Debug.WriteLine("A frame already processing in the HandPredictResultUnUseQueueAsync");
            return ret;
        }
        

        _calculator = calculator;
        _modelPath = modelPath;

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

                var image = new Bitmap(stream.AsStream());

                var matData = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                var dataMeta = mat2.Data;

                var length = mat2.Width * mat2.Height * mat2.Channels();

                var data = new byte[length];

                Marshal.Copy(dataMeta, data, 0, length);

                var widthStep = (int)mat2.Step();

                var imgframe = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                var handsOutput = _calculator!.Compute(imgframe);

                if (handsOutput.MultiHandLandmarks != null)
                {
                    var landmarks = handsOutput.MultiHandLandmarks[0].Landmark;

                    Debug.WriteLine($"Got hands output with {landmarks.Count} landmarks");

                    var result = HandDataFormatHelper.PredictResult(landmarks.ToList(), _modelPath!);

                    Debug.WriteLine($"Hand Result: {result}");

                    CameraFrameService.Current.NotifyHandPredictResult(result);
                    
                }
                else
                {
                    Debug.WriteLine("No hand landmarks");
                }
                _isProcessing = false;
            }
        }
        catch (Exception e)
        {
            _isProcessing = false;
        }

        return ret;
    }

    public void ClearQueue()
    {
        _queue.Clear();
    }

    private async Task SendDataAsync()
    {
        while (_queue.TryDequeue(out var item))
        {
            var (softwareBitmap, tcs, cancellationToken) = item;

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.TrySetCanceled(cancellationToken);
                continue;
            }

            var ret = "";
            try
            {
                if (softwareBitmap != null)
                {
                    using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                    // Set the software bitmap
                    encoder.SetSoftwareBitmap(softwareBitmap);

                    await encoder.FlushAsync();

                    var image = new Bitmap(stream.AsStream());

                    var matData = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                    var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                    var dataMeta = mat2.Data;

                    var length = mat2.Width * mat2.Height * mat2.Channels();

                    var data = new byte[length];

                    Marshal.Copy(dataMeta, data, 0, length);

                    var widthStep = (int)mat2.Step();

                    var imgframe = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                    var handsOutput = _calculator!.Compute(imgframe);

                    if (handsOutput.MultiHandLandmarks != null)
                    {
                        var landmarks = handsOutput.MultiHandLandmarks[0].Landmark;

                        Debug.WriteLine($"Got hands output with {landmarks.Count} landmarks");

                        var result = HandDataFormatHelper.PredictResult(landmarks.ToList(), _modelPath!);

                        Debug.WriteLine($"Hand Result: {result}");

                        CameraFrameService.Current.NotifyHandPredictResult(result);

                    }
                    else
                    {
                        Debug.WriteLine("No hand landmarks");
                    }

                   
                    tcs.TrySetResult(ret);
                }
                //todo:
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        }

        Interlocked.Exchange(ref _isSending, 0);
    }
}