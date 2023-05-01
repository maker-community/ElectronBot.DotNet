using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ElectronBot.Braincase.Helpers;
public class BitmapTools
{
    /// <summary>
    /// Gets the cropped bitmap asynchronously.
    /// </summary>
    /// <param name="originalImage">The original image.</param>
    /// <param name="startPoint">The start point.</param>
    /// <param name="cropSize">Size of the corp.</param>
    /// <param name="scale">The scale.</param>
    /// <returns>The cropped image.</returns>
    public static async Task<WriteableBitmap> GetCroppedBitmapAsync(IRandomAccessStream originalImage,
        Point startPoint, Size cropSize, double scale)
    {
        if (double.IsNaN(scale) || double.IsInfinity(scale))
        {
            scale = 1;
        }

        // Convert start point and size to integer.
        var startPointX = (uint)Math.Floor(startPoint.X * scale);
        var startPointY = (uint)Math.Floor(startPoint.Y * scale);
        var height = (uint)Math.Floor(cropSize.Height * scale);
        var width = (uint)Math.Floor(cropSize.Width * scale);

        // Create a decoder from the stream. With the decoder, we can get 
        // the properties of the image.
        var decoder = await BitmapDecoder.CreateAsync(originalImage);

        // The scaledSize of original image.
        var scaledWidth = (uint)Math.Floor(decoder.PixelWidth * scale);
        var scaledHeight = (uint)Math.Floor(decoder.PixelHeight * scale);

        // Refine the start point and the size. 
        if (startPointX + width > scaledWidth)
        {
            startPointX = scaledWidth - width;
        }

        if (startPointY + height > scaledHeight)
        {
            startPointY = scaledHeight - height;
        }

        // Get the cropped pixels.
        var pixels = await GetPixelData(decoder, startPointX, startPointY, width, height,
            scaledWidth, scaledHeight);

        // Stream the bytes into a WriteableBitmap
        var cropBmp = new WriteableBitmap((int)width, (int)height);
        var pixStream = cropBmp.PixelBuffer.AsStream();
        pixStream.Write(pixels, 0, (int)(width * height * 4));

        return cropBmp;
    }

    /// <summary>
    /// Gets the pixel data.
    /// </summary>
    /// <remarks>
    /// If you want to get the pixel data of a scaled image, set the scaledWidth and scaledHeight
    /// of the scaled image.
    /// </remarks>
    /// <param name="decoder">The bitmap decoder.</param>
    /// <param name="startPointX">The X coordinate of the start point.</param>
    /// <param name="startPointY">The Y coordinate of the start point.</param>
    /// <param name="width">The width of the source rect.</param>
    /// <param name="height">The height of the source rect.</param>
    /// <param name="scaledWidth">The desired width.</param>
    /// <param name="scaledHeight">The desired height.</param>
    /// <returns>The image data.</returns>
    private static async Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY,
        uint width, uint height, uint scaledWidth, uint scaledHeight)
    {
        var transform = new BitmapTransform();
        var bounds = new BitmapBounds();
        bounds.X = startPointX;
        bounds.Y = startPointY;
        bounds.Height = height;
        bounds.Width = width;
        //transform.Bounds = bounds;

        transform.ScaledWidth = 240;
        transform.ScaledHeight = 240;

        // Get the cropped pixels within the bounds of transform.
        var pix = await decoder.GetPixelDataAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Straight,
            transform,
            ExifOrientationMode.IgnoreExifOrientation,
            ColorManagementMode.ColorManageToSRgb);
        var pixels = pix.DetachPixelData();
        return pixels;
    }

    /// <summary>
    /// Resizes the specified stream.
    /// </summary>
    /// <param name="sourceStream">The source stream to resize.</param>
    /// <param name="newWidth">The width of the resized image.</param>
    /// <param name="newHeight">The height of the resized image.</param>
    /// <returns>The resized image stream.</returns>
    public static async Task<InMemoryRandomAccessStream> Resize(IRandomAccessStream sourceStream, uint requestedMinSide)
    {
        var decoder = await BitmapDecoder.CreateAsync(sourceStream);
        uint originalPixelWidth = decoder.OrientedPixelWidth;
        uint originalPixelHeight = decoder.OrientedPixelHeight;

        double widthRatio = (double)requestedMinSide / originalPixelWidth;
        double heightRatio = (double)requestedMinSide / originalPixelHeight;
        uint aspectHeight = (uint)requestedMinSide;
        uint aspectWidth = (uint)requestedMinSide;
        var scaledSize = (uint)requestedMinSide;
        if (originalPixelWidth < originalPixelHeight)
        {
            aspectWidth = (uint)(heightRatio * originalPixelWidth);
        }
        else
        {
            aspectHeight = (uint)(widthRatio * originalPixelHeight);
        }

        var destinationStream = new InMemoryRandomAccessStream();


        var transform = new BitmapTransform { ScaledWidth = aspectWidth, ScaledHeight = aspectHeight };

        var pixelData = await decoder.GetPixelDataAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Straight,
            transform,
            ExifOrientationMode.RespectExifOrientation,
            ColorManagementMode.DoNotColorManage);

        var encoder =
            await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);

        if (decoder.OrientedPixelHeight != decoder.PixelHeight && decoder.OrientedPixelWidth != decoder.PixelWidth)
            encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees;

        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, aspectWidth, aspectHeight, 96, 96,
            pixelData.DetachPixelData());
        await encoder.FlushAsync();


        return destinationStream;
    }

    /// <summary>
    /// Rotates the given stream.
    /// </summary>
    /// <param name="randomAccessStream">The random access stream.</param>
    /// <param name="rotation">The rotation.</param>
    /// <returns>The stream.</returns>
    public static async Task<InMemoryRandomAccessStream> Rotate(IRandomAccessStream randomAccessStream,
        BitmapRotation rotation)
    {
        var decoder = await BitmapDecoder.CreateAsync(randomAccessStream);

        var rotatedStream = new InMemoryRandomAccessStream();

        var encoder = await BitmapEncoder.CreateForTranscodingAsync(rotatedStream, decoder);

        encoder.BitmapTransform.Rotation = rotation;
        encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;

        await encoder.FlushAsync();

        return rotatedStream;
    }


    /// <summary>
    /// Resizes and crops source file image so that resized image width/height are not larger than <param name="requestedMinSide"></param>
    /// </summary>
    /// <param name="sourceFile">Source StorageFile</param>
    /// <param name="requestedMinSide">Width/Height of the output image</param>
    /// <param name="resizedImageFile">Target StorageFile</param>
    /// <returns></returns>
    public static async Task<IStorageFile> CreateThumbnaiImage(StorageFile sourceFile, int requestedMinSide, StorageFile resizedImageFile)
    {
        var imageStream = await sourceFile.OpenReadAsync();
        var decoder = await BitmapDecoder.CreateAsync(imageStream);
        var originalPixelWidth = decoder.PixelWidth;
        var originalPixelHeight = decoder.PixelHeight;
        using (imageStream)
        {
            //do resize only if needed
            if (originalPixelHeight > requestedMinSide && originalPixelWidth > requestedMinSide)
            {
                using (var resizedStream = await resizedImageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    //create encoder based on decoder of the source file
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
                    double widthRatio = (double)requestedMinSide / originalPixelWidth;
                    double heightRatio = (double)requestedMinSide / originalPixelHeight;
                    uint aspectHeight = (uint)requestedMinSide;
                    uint aspectWidth = (uint)requestedMinSide;
                    uint cropX = 0, cropY = 0;
                    var scaledSize = (uint)requestedMinSide;
                    if (originalPixelWidth > originalPixelHeight)
                    {
                        aspectWidth = (uint)(heightRatio * originalPixelWidth);
                        cropX = (aspectWidth - aspectHeight) / 2;
                    }
                    else
                    {
                        aspectHeight = (uint)(widthRatio * originalPixelHeight);
                        cropY = (aspectHeight - aspectWidth) / 2;
                    }
                    //you can adjust interpolation and other options here, so far linear is fine for thumbnails
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                    encoder.BitmapTransform.ScaledHeight = aspectHeight;
                    encoder.BitmapTransform.ScaledWidth = aspectWidth;
                    encoder.BitmapTransform.Bounds = new BitmapBounds()
                    {
                        Width = scaledSize,
                        Height = scaledSize,
                        X = cropX,
                        Y = cropY,
                    };
                    await encoder.FlushAsync();
                }
            }
            else
            {
                //otherwise just use source file as thumbnail
                await sourceFile.CopyAndReplaceAsync(resizedImageFile);
            }
        }
        return resizedImageFile;
    }

}