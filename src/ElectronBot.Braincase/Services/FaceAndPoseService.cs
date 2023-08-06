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
using Windows.Media.FaceAnalysis;
using Windows.Media;
using Windows.Storage.Streams;
using Microsoft.Graphics.Canvas;
using Verdure.ElectronBot.Core.Models;
using Windows.Graphics;
using System.Text;
using System.IO;

namespace Services;
public class FaceAndPoseService
{
    private readonly ConcurrentQueue<(IRandomAccessStream? Data, TaskCompletionSource Tcs, CancellationToken Ct)> _queue = new();

    private int _isSending;

    private PoseCpuSolution? _calculator;

    private bool _isProcessing = false;

    private byte[] _faceData = new byte[240 * 240 * 3];


    private FaceDetector? _faceDetector;
    private SizeInt32 _lastSize;
    public async Task SetSolutionAsync(PoseCpuSolution? calculator, SizeInt32 lastSize)
    {
        _calculator = calculator;

        _lastSize = lastSize;
        _faceDetector = await FaceDetector.CreateAsync();
    }

    public async Task FaceAndPoseResultAsync(IRandomAccessStream? streamData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tcs = new TaskCompletionSource();

        _queue.Enqueue((streamData, tcs, cancellationToken));

        if (Interlocked.CompareExchange(ref _isSending, 1, 0) == 0)
        {
            _ = Task.Run(SendDataAsync, CancellationToken.None);
        }
        await tcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
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
            try
            {
                if (softwareBitmap != null)
                {
                    _isProcessing = true;

                    using var face = await FaceDetectionAsync(softwareBitmap);

                    if (face is not null)
                    {
                        using IRandomAccessStream faceStream = new InMemoryRandomAccessStream();

                        var encoderFace = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, faceStream);

                        // Set the software bitmap
                        encoderFace.SetSoftwareBitmap(face);

                        await encoderFace.FlushAsync();

                        using var imageFace = new System.Drawing.Bitmap(faceStream.AsStream());

                        var matFace = OpenCvSharp.Extensions.BitmapConverter.ToMat(imageFace);

                        var matFace1 = matFace.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

                        var matFace2 = matFace1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                        var dataMetaFace = matFace2.Data;

                        var faceData = new byte[240 * 240 * 3];

                        Marshal.Copy(dataMetaFace, faceData, 0, 240 * 240 * 3);

                        _faceData = faceData;
                    }


                    

                    //using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                    //var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                    //// Set the software bitmap
                    //encoder.SetSoftwareBitmap(softwareBitmap);

                    //await encoder.FlushAsync();

                    using var image = new Bitmap(softwareBitmap.AsStream());

                    var matData = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                    var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                    var dataMeta = mat2.Data;

                    var length = mat2.Width * mat2.Height * mat2.Channels();

                    var data = new byte[length];

                    Marshal.Copy(dataMeta, data, 0, length);

                    var widthStep = (int)mat2.Step();

                    using var imgFrame = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

                    var poseOutput = _calculator!.Compute(imgFrame);

                    if (poseOutput.PoseLandmarks != null)
                    {
                        await SendFaceAndPoseDataAsync(poseOutput);
                        //CameraFrameService.Current.NotifyPosePredictResult(poseOutput);
                    }
                    else
                    {
                        Debug.WriteLine("No hand landmarks");
                    }
                    _isProcessing = false;


                    softwareBitmap.Dispose();
                    softwareBitmap = null;
                    tcs.TrySetResult();
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

    private async Task SendFaceAndPoseDataAsync(PoseOutput e)
    {
        try
        {
            var leftUpAngle = AngleHelper.GetPointAngle(
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[24].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[24].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[14].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[12].Y * _lastSize.Height));

            var rightUpAngle = AngleHelper.GetPointAngle(
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[13].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[23].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[23].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[11].Y * _lastSize.Height));


            var leftWaveAngle = AngleHelper.GetPointAngle(
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[16].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[16].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[12].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[14].Y * _lastSize.Height));

            var rightWaveAngle = AngleHelper.GetPointAngle(
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[15].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[15].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[11].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[13].Y * _lastSize.Height));

            var headAngle = AngleHelper.GetPointAngle(
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[11].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[12].Y * _lastSize.Height),
                new System.Numerics.Vector2(e.PoseLandmarks.Landmark[0].X * _lastSize.Width,
                    e.PoseLandmarks.Landmark[0].Y * _lastSize.Height));

            float j1 = 0;
            if (headAngle < 90)
            {
                headAngle = 180 - headAngle;
                j1 = (headAngle / 180) * 20;
            }
            else if (headAngle > 90)
            {
                j1 = (headAngle / 180) * 15 * (-1);
            }


            var frame = new EmoticonActionFrame(_faceData, true, j1, (rightWaveAngle / 180) * 30, rightUpAngle,
                (leftWaveAngle / 180) * 30, leftUpAngle, 0);

            //待处理面部数据
            await EbHelper.ShowDataToDeviceAsync(null, frame);
        }
        catch(Exception)
        {
        }
    }

    private async Task<SoftwareBitmap?> FaceDetectionAsync(IRandomAccessStream streamData)
    {

        var decoder = await BitmapDecoder.CreateAsync(streamData);

        var transform = new BitmapTransform();

        const float sourceImageHeightLimit = 1280;

        if (decoder.PixelHeight > sourceImageHeightLimit)
        {
            float scalingFactor = (float)sourceImageHeightLimit / (float)decoder.PixelHeight;
            transform.ScaledWidth = (uint)Math.Floor(decoder.PixelWidth * scalingFactor);
            transform.ScaledHeight = (uint)Math.Floor(decoder.PixelHeight * scalingFactor);
        }

       using var sourceBitmap = await decoder.GetSoftwareBitmapAsync(decoder.BitmapPixelFormat, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);


        // Use FaceDetector.GetSupportedBitmapPixelFormats and IsBitmapPixelFormatSupported to dynamically
        // determine supported formats
        const BitmapPixelFormat faceDetectionPixelFormat = BitmapPixelFormat.Gray8;

        using var convertedBitmap= SoftwareBitmap.Convert(sourceBitmap, faceDetectionPixelFormat);


        var faces = await _faceDetector?.DetectFacesAsync(convertedBitmap);

        if (faces.Count > 0)
        {
            //crop image to focus on face portion
            var faceBox = faces[0].FaceBox;

            var inputFrame = VideoFrame.CreateWithSoftwareBitmap(sourceBitmap);

            var tmp = new VideoFrame(sourceBitmap.BitmapPixelFormat, (int)(faceBox.Width + faceBox.Width % 2) - 2,
                (int)(faceBox.Height + faceBox.Height % 2) - 2);

            await inputFrame.CopyToAsync(tmp, faceBox, null);

            if (tmp.SoftwareBitmap is not null)
            {
                return tmp.SoftwareBitmap;
            }
        }
        return null;
    }
}