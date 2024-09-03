using System.Runtime.InteropServices.WindowsRuntime;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Picker;
using ElectronBot.Braincase.Services;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ElectronBot.Braincase.Helpers;

public static class ImageHelper
{
    public static async Task<BitmapImage> ImageFromStringAsync(string data)
    {
        var byteArray = Convert.FromBase64String(data);
        var image = new BitmapImage();
        using (var stream = new InMemoryRandomAccessStream())
        {
            await stream.WriteAsync(byteArray.AsBuffer());
            stream.Seek(0);
            await image.SetSourceAsync(stream);
        }

        return image;
    }

    public static BitmapImage ImageFromAssetsFile(string fileName)
    {
        var image = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"));
        return image;
    }


    public static async Task<WriteableBitmap> CropImage(ImageCropperConfig config)
    {
        var startOption = new PickerOpenOption
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var ret = await App.GetService<ObjectPickerService>()
            .PickSingleObjectAsync<WriteableBitmap>
            (typeof(ImageCropperPickerViewModel).FullName!, config, startOption);

        if (!ret.Canceled)
        {
            return ret.Result;
        }
        return null;
    }

    public static async Task<bool> SaveWriteableBitmapImageFileAsync(WriteableBitmap image, StorageFile file)
    {
        var result = false;

        //BitmapEncoder 存放格式
        var bitmapEncoderGuid = BitmapEncoder.JpegEncoderId;

        var filename = file.Name;

        if (filename.EndsWith("jpg"))
        {
            bitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
        }
        else if (filename.EndsWith("png"))
        {
            bitmapEncoderGuid = BitmapEncoder.PngEncoderId;
        }
        else if (filename.EndsWith("bmp"))
        {
            bitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
        }
        else if (filename.EndsWith("tiff"))
        {
            bitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
        }
        else if (filename.EndsWith("gif"))
        {
            bitmapEncoderGuid = BitmapEncoder.GifEncoderId;
        }

        using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
        {
            var encoder = await BitmapEncoder.CreateAsync(bitmapEncoderGuid, stream);

            var pixelStream = image.PixelBuffer.AsStream();

            var pixels = new byte[pixelStream.Length];

            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)image.PixelWidth, (uint)image.PixelHeight, 96.0, 96.0, pixels);

            //Windows.Graphics.Imaging.BitmapDecoder decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(imgstream);

            //Windows.Graphics.Imaging.PixelDataProvider pxprd = await decoder.GetPixelDataAsync(Windows.Graphics.Imaging.BitmapPixelFormat.Bgra8, Windows.Graphics.Imaging.BitmapAlphaMode.Straight, new Windows.Graphics.Imaging.BitmapTransform(), Windows.Graphics.Imaging.ExifOrientationMode.RespectExifOrientation, Windows.Graphics.Imaging.ColorManagementMode.DoNotColorManage);

            await encoder.FlushAsync();

            result = true;
        }
        return result;
    }
}
