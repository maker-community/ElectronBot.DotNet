using System.Runtime.InteropServices.WindowsRuntime;
using HelloWordKeyboard.DotNet;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ElectronBot.Braincase.Helpers;

public class Hw75Helper
{
    public IHw75DynamicDevice? Hw75DynamicDevice
    {
        get; set;
    }

    public bool IsConnected
    {
        get; set;
    }

    private static Hw75Helper? _instance;
    public static Hw75Helper Instance => _instance ??= new Hw75Helper();

    private readonly SynchronizationContext? _context = SynchronizationContext.Current;

    public Hw75Helper()
    {
        try
        {
            Hw75DynamicDevice = new Hw75DynamicDevice();
        }
        catch
        {

        }

    }

    /// <summary>
    /// 同步数据到瀚文键盘
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public async Task SyncDataToDeviceAsync(UIElement? element)
    {
        if (Hw75Helper.Instance.IsConnected)
        {
            try
            {
                var renderTargetBitmap = new RenderTargetBitmap();

                await renderTargetBitmap.RenderAsync(element);

                var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                using var stream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight,
                    96,
                    96,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
                stream.Seek(0);

                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(stream.AsStream());

                image.Mutate(x =>
                {
                    x.Resize(128, 296);
                    //x.Grayscale();
                });

                var byteArray = image.EnCodeImageToBytes();


                _ = Hw75Helper.Instance.Hw75DynamicDevice?.SetEInkImage(byteArray, 0, 0, 128, 296, false);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
