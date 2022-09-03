using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Threading.Tasks;
using ElectronBot.BraincasePreview.Core.Models;
using ElectronBot.BraincasePreview.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Devices;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ElectronBot.BraincasePreview.Helpers;

public class EbHelper
{
    public static async Task<string> ToBase64Async(byte[] image, uint height, uint width, double dpiX = 96, double dpiY = 96)
    {
        // encode image
        var encoded = new InMemoryRandomAccessStream();

        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, encoded);

        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, height, width, dpiX, dpiY, image);

        await encoder.FlushAsync();

        encoded.Seek(0);

        // read bytes
        var bytes = new byte[encoded.Size];

        await encoded.AsStream().ReadAsync(bytes, 0, bytes.Length);

        // create base64
        return Convert.ToBase64String(bytes);
    }

    public static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Panel desiredPanel = Panel.Unknown)
    {
        // Get available devices for capturing pictures
        var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        // Get the desired camera by panel
        DeviceInformation desiredDevice = allVideoDevices
            .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

        // If there is no device mounted on the desired panel, return the first device found
        return desiredDevice ?? allVideoDevices.FirstOrDefault();
    }


    /// <summary>
    /// 获取相机设备列表
    /// </summary>
    /// <returns></returns>
    public static async Task<List<ComboxItemModel>> FindCameraDeviceListAsync()
    {
        List<ComboxItemModel> ret = new();
        // Get available devices for capturing pictures
        var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

        if (allVideoDevices != null && allVideoDevices.Count > 0)
        {
            var devList = allVideoDevices.ToList();

            foreach (var dev in devList)
            {
                ComboxItemModel combox = new()
                {
                    DataKey = devList.IndexOf(dev).ToString(),
                    DataValue = dev.Name,
                    Tag = dev
                };

                ret.Add(combox);
            }
        }

        return ret;
    }

    /// <summary>
    /// 获取音频设备列表
    /// </summary>
    /// <returns></returns>
    public static async Task<List<ComboxItemModel>> FindAudioDeviceListAsync()
    {
        List<ComboxItemModel> ret = new();

        var audioSelector = MediaDevice.GetAudioRenderSelector();

        var allVideoDevices = await DeviceInformation.FindAllAsync(audioSelector);

        if (allVideoDevices != null && allVideoDevices.Count > 0)
        {
            var devList = allVideoDevices.ToList();

            foreach (var dev in devList)
            {
                ComboxItemModel combox = new()
                {
                    Tag = dev,
                    DataKey = devList.IndexOf(dev).ToString(),
                    DataValue = dev.Name
                };

                ret.Add(combox);
            }
        }

        return ret;
    }

    /// <summary>
    /// 导入动作列表
    /// </summary>
    /// <param name="hwnd"></param>
    /// <returns></returns>
    public static async Task<List<ElectronBotAction>> ImportActionListAsync(IntPtr hwnd)
    {
        var list = new List<ElectronBotAction>();
        try
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
            };

            picker.FileTypeFilter.Add(".json");

            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var text = await FileIO.ReadTextAsync(file);

                var actionList = JsonSerializer.Deserialize<List<ElectronBotAction>>(text);

                if (actionList != null && actionList.Count > 0)
                {
                    foreach (var action in actionList)
                    {
                        action.Id = Guid.NewGuid().ToString();

                        if (action.ImageData != null && action.ImageData.StartsWith(("data:image/png;base64")))
                        {
                            var base64Text = action.ImageData;

                            base64Text = base64Text
                                .Replace("data:image/png;base64,", "")
                                .Replace("data:image/jgp;base64,", "")
                                .Replace("data:image/jpg;base64,", "")
                                .Replace("data:image/jpeg;base64,", "");//将base64头部信息替换

                            // read stream
                            var bytes = Convert.FromBase64String(base64Text);

                            var image = bytes.AsBuffer().AsStream().AsRandomAccessStream();

                            // decode image
                            var decoder = await BitmapDecoder.CreateAsync(image);

                            image.Seek(0);

                            // create bitmap
                            var output = new WriteableBitmap((int)decoder.PixelHeight, (int)decoder.PixelWidth);

                            await output.SetSourceAsync(image);

                            action.BitmapImageData = output;
                        }

                        list.Add(action);
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }

        return list;
    }

    /// <summary>
    /// 播放动作列表
    /// </summary>
    /// <param name="actions">动作列表</param>
    /// <param name="interval">动作时间间隔默认500毫秒</param>
    /// <returns></returns>
    public static Task PlayActionListAsync(List<ElectronBotAction> actions, int interval = 500)
    {
        try
        {
            if (actions != null && actions.Count > 0)
            {
                EmojiPlayHelper.Current.Interval = interval;

                foreach (var action in actions)
                {
                    var base64Text = action.ImageData;

                    if (base64Text != null)
                    {
                        base64Text = base64Text
                                .Replace("data:image/png;base64,", "")
                                .Replace("data:image/jgp;base64,", "")
                                .Replace("data:image/jpg;base64,", "")
                                .Replace("data:image/jpeg;base64,", "");//将base64头部信息替换

                        var bytes = Convert.FromBase64String(base64Text);

                        var imageStream = bytes.AsBuffer().AsStream();

                        var image = new Bitmap(imageStream);

                        var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                        var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

                        var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                        var dataMeta = mat2.Data;

                        var data = new byte[240 * 240 * 3];

                        Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                        var frame = new EmoticonActionFrame(
                                   data, true, action.J1, action.J2, action.J3, action.J4, action.J5, action.J6);

                        EmojiPlayHelper.Current.Enqueue(frame);
                    }
                }
            }

            ///EmojiPlayHelper.Current.Interval = 0;

        }
        catch (Exception ex)
        {

        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 量子纠缠
    /// </summary>
    /// <param name="softwareBitmap"></param>
    /// <returns></returns>
    public static async Task ShowFaceToDeviceAsync(SoftwareBitmap softwareBitmap)
    {
        if (softwareBitmap != null)
        {
            using IRandomAccessStream stream = new InMemoryRandomAccessStream();

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            // Set the software bitmap
            encoder.SetSoftwareBitmap(softwareBitmap);

            await encoder.FlushAsync();

            var image = new Bitmap(stream.AsStream());

            var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

            var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

            var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

            var dataMeta = mat2.Data;

            var data = new byte[240 * 240 * 3];

            Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

            var frame = new EmoticonActionFrame(data);

            EmojiPlayHelper.Current.Enqueue(frame);
        }
    }


    /// <summary>
    /// 表盘内容同步到电子
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static async Task ShowClockCanvasToDeviceAsync(UIElement element)
    {
        try
        {
            var bitmap = new RenderTargetBitmap();

            await bitmap.RenderAsync(element);

            var pixels = await bitmap.GetPixelsAsync();

            var canvasDevice = App.GetService<CanvasDevice>();
            // Transfer the pixel data from XAML to Win2D for further processing.
            //using var canvasDevice = CanvasDevice.GetSharedDevice();

            using var canvasBitmap = CanvasBitmap.CreateFromBytes(
                canvasDevice, pixels.ToArray(), bitmap.PixelWidth, bitmap.PixelHeight,
                Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized);

            using IRandomAccessStream stream = new InMemoryRandomAccessStream();

            await canvasBitmap.SaveAsync(stream, CanvasBitmapFileFormat.Png);

            var image = new Bitmap(stream.AsStream());

            var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

            var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

            var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

            var dataMeta = mat2.Data;

            var data = new byte[240 * 240 * 3];

            Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

            EmojiPlayHelper.Current.Enqueue(new EmoticonActionFrame(data));
        }
        catch (Exception ex)
        {

        }
    }
}
