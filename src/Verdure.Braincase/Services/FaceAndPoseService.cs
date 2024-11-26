using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.Services;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Windows.Graphics.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.Media;
using Windows.Storage.Streams;
using Microsoft.Graphics.Canvas;
using Verdure.Braincase.Core.Models;
using Windows.Graphics;
using System.Text;
using System.IO;
using SixLabors.ImageSharp.Processing;
using NAudio.Wave;

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

                        byte[] pixelBytes;

                        using (DataReader dataReader = new DataReader(faceStream.GetInputStreamAt(0)))
                        {
                            var streamSize = (uint)faceStream.Size;
                            await dataReader.LoadAsync(streamSize);
                            pixelBytes = new byte[streamSize];
                            dataReader.ReadBytes(pixelBytes);
                        }

                        //使用ImageSharp处理帧数据
                        using var image2 = SixLabors.ImageSharp.Image
                            .LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(pixelBytes,
                            (int)face.PixelWidth, (int)face.PixelHeight);

                        image2.Mutate(x =>
                        {
                            x.Resize(240, 240);
                        });

                        // 获取转换后的数据
                        var rgbData = new byte[image2.Width * image2.Height * 3];

                        // 遍历每个像素，将Rgba32转换为Bgr24
                        for (var y = 0; y < image2.Height; y++)
                        {
                            for (var x = 0; x < image2.Width; x++)
                            {
                                var rgbaPixel = image2[x, y];
                                var rgbIndex = (y * image2.Width + x) * 3;
                                rgbData[rgbIndex] = rgbaPixel.B;
                                rgbData[rgbIndex + 1] = rgbaPixel.G;
                                rgbData[rgbIndex + 2] = rgbaPixel.R;
                            }
                        }
                        _faceData = rgbData;
                    }


                    byte[] pixelBytes2;

                    using (DataReader dataReader = new DataReader(softwareBitmap.GetInputStreamAt(0)))
                    {
                        var streamSize = (uint)softwareBitmap.Size;
                        await dataReader.LoadAsync(streamSize);
                        pixelBytes2 = new byte[streamSize];
                        dataReader.ReadBytes(pixelBytes2);
                    }

                    //使用ImageSharp处理帧数据
                    using var image3 = SixLabors.ImageSharp.Image
                        .LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(pixelBytes2,
                        (int)face.PixelWidth, (int)face.PixelHeight);

                    image3.Mutate(x =>
                    {
                        x.Resize(240, 240);
                    });

                    // 获取转换后的数据
                    var rgbData2 = new byte[image3.Width * image3.Height * 3];

                    // 遍历每个像素，将Rgba32转换为Bgr24
                    for (var y = 0; y < image3.Height; y++)
                    {
                        for (var x = 0; x < image3.Width; x++)
                        {
                            var rgbaPixel = image3[x, y];
                            var rgbIndex = (y * image3.Width + x) * 3;
                            rgbData2[rgbIndex] = rgbaPixel.B;
                            rgbData2[rgbIndex + 1] = rgbaPixel.G;
                            rgbData2[rgbIndex + 2] = rgbaPixel.R;
                        }
                    }

                    using var imgFrame = new ImageFrame(ImageFormat.Types.Format.Srgb, image3.Width, image3.Height, rgbData2.Length, rgbData2);

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