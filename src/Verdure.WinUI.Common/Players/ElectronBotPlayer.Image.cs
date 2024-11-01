using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Verdure.Braincase.Core.Models;
using Windows.Storage;

namespace Verdure.Braincase.WinUI.Common.Players;
public partial class ElectronBotPlayer
{
    public async Task PlayImageAsync(string path)
    {
        using var image = await LoadImageAsync(path);

        image.Mutate(x =>
        {
            x.Resize(240, 240);
        });

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
    public Task PlayImageAsync(Stream stream) => throw new NotImplementedException();
    public async Task PlayImageAsync(byte[] bytes)
    {
        var frameData = new EmoticonActionFrame(bytes, false);

        _ = await _actionFrameService.SendToUsbDeviceAsync(frameData);
    }

    private async Task<Image<Rgba32>> LoadImageAsync(string imagePath)
    {
        if (imagePath.StartsWith("ms-appx"))
        {
            using var stream = await GetFileStreamAsync(imagePath);
            return Image.Load<Rgba32>(stream);
        }
        else
        {
            return Image.Load<Rgba32>(imagePath);
        }
    }

    private async Task<Stream> GetFileStreamAsync(string filePath)
    {
        var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(filePath));
        var randomAccessStream = await storageFile.OpenAsync(FileAccessMode.Read);
        return randomAccessStream.AsStreamForRead();
    }
}
