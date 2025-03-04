using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using SkiaSharp.Skottie;
using Verdure.Braincase.Core.Models;
using Windows.ApplicationModel;

namespace Verdure.Braincase.WinUI.Common.Players;

public partial class ElectronBotPlayer
{
    public async Task PlayLottieByNameIdAsync(string nameId, int times)
    {
        await _emojiSemaphore.WaitAsync();
        try
        {
            var path = Package.Current.InstalledLocation.Path + $"\\Assets\\LottieFiles\\{nameId}.json";
            // 读取Lottie JSON文件
            var animation = Animation.Create(path);
            if (animation != null)
            {
                for (var t = 0; t < times; t++)
                {
                    animation.Seek(0);
                    //帧数
                    var frameCount = animation.OutPoint;
                    Console.WriteLine($"frame count :{frameCount}");
                    Console.WriteLine($"fps :{animation.Fps}");
                    Console.WriteLine($"Duration :{animation.Duration.TotalSeconds}");
                    for (var i = 0; i < frameCount; i++)
                    {
                        var progress = animation.Duration.TotalSeconds / (frameCount - i);
                        var image = RenderLottieFrame(animation, progress, 240, 240);
                        // 获取转换后的数据
                        var rgbData = new byte[image.Width * image.Height * 3];

                        // 遍历每个像素，将Rgba32转换为Bgr24
                        for (var y = 0; y < image.Height; y++)
                        {
                            for (var x = 0; x < image.Width; x++)
                            {
                                var rgbaPixel = image[x, y];
                                var rgbIndex = (y * image.Width + x) * 3;
                                rgbData[rgbIndex] = rgbaPixel.B;
                                rgbData[rgbIndex + 1] = rgbaPixel.G;
                                rgbData[rgbIndex + 2] = rgbaPixel.R;
                            }
                        }

                        var frameData = new EmoticonActionFrame(rgbData, false);

                        _ = await _actionFrameService.SendToUsbDeviceAsync(frameData);
                    }
                }

            }
        }
        finally
        {
            _emojiSemaphore.Release();
        }
    }

    private static Image<Bgra32> RenderLottieFrame(Animation animation, double progress, int width, int height)
    {
        // 创建SKSurface用于渲染
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        // 清除背景
        canvas.Clear(SKColors.Transparent);

        animation.SeekFrameTime(progress);
        animation.Render(canvas, new SKRect(0, 0, width, height));

        // 将SKBitmap转换为byte数组
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var bytes = data.ToArray();

        // 转换为ImageSharp格式
        using var memStream = new MemoryStream(bytes);
        return Image.Load<Bgra32>(memStream);
    }

}
