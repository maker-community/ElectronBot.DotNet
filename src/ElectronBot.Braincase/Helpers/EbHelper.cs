using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using Verdure.ElectronBot.Core.Models;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ElectronBot.Braincase.Helpers;

public class EbHelper
{
    public static byte[] FaceData
    {
        get;
        set;
    } = new byte[240 * 240 * 3];

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
    public static async Task PlayActionListAsync(List<ElectronBotAction> actions, int interval = 500)
    {
        var service = App.GetService<EmoticonActionFrameService>();

        service.ClearQueue();

        try
        {
            if (actions != null && actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    var base64Text = action.ImageData;

                    var data = new byte[240 * 240 * 3];

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

                        Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);
                    }
                    var frame = new EmoticonActionFrame(
                             data, true, action.J1, action.J2, action.J3, action.J4, action.J5, action.J6);

                    _ = await service.SendToUsbDeviceAsync(frame);
                }
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 量子纠缠带动作
    /// </summary>
    /// <param name="softwareBitmap"></param>
    /// <param name="frameData"></param>
    /// <returns></returns>
    public static async Task ShowDataToDeviceAsync(SoftwareBitmap? softwareBitmap, EmoticonActionFrame frameData)
    {
        if (softwareBitmap != null)
        {
            using IRandomAccessStream stream = new InMemoryRandomAccessStream();

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            // Set the software bitmap
            encoder.SetSoftwareBitmap(softwareBitmap);

            await encoder.FlushAsync();

            using var image = new Bitmap(stream.AsStream());

            var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

            var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

            var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

            var dataMeta = mat2.Data;

            var data = new byte[240 * 240 * 3];

            Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

            var frame = new EmoticonActionFrame(data, frameData.Enable,
                frameData.J1, frameData.J2, frameData.J3, frameData.J4, frameData.J5, frameData.J6);

            var service = App.GetService<EmoticonActionFrameService>();

            ElectronBotHelper.Instance.ModelActionInvoke(
                new ModelActionFrame(mat2.ToMemoryStream(),
                    false, frameData.J1, frameData.J2, frameData.J3, frameData.J4, frameData.J5, frameData.J6));

            await service.SendToUsbDeviceAsync(frame);
        }
        else
        {
            ElectronBotHelper.Instance.ModelActionInvoke(new ModelActionFrame(new MemoryStream(), false,
                frameData.J1, frameData.J2, frameData.J3, frameData.J4, frameData.J5, frameData.J6));

            ElectronBotHelper.Instance.PlayEmoticonActionFrame(frameData);
        }

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

            var service = App.GetService<EmoticonActionFrameService>();

            ElectronBotHelper.Instance.ModelActionInvoke(new ModelActionFrame(mat2.ToMemoryStream()));

            await service.SendToUsbDeviceAsync(frame);
        }
    }


    /// <summary>
    /// 表盘内容同步到电子
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static async Task ShowClockCanvasToDeviceAsync(UIElement element)
    {
        var data = await SetClockUiToFrameAsync(element);
        var service = App.GetService<EmoticonActionFrameService>();
        _ = await service.SendToUsbDeviceAsync(data);
    }

    /// <summary>
    /// 表盘内容和指针数据同步到电子
    /// </summary>
    /// <param name="element">画面</param>
    /// <param name="height">屏幕高度</param>
    /// <param name="width">屏幕宽度</param>
    /// <param name="x">指针x轴</param>
    /// <param name="y">指针y轴</param>
    /// <returns></returns>
    public static async Task ShowClockCanvasAndPosToDeviceAsync(UIElement? element, int height, int width, int x, int y)
    {
        //var data = await SetClockUiToFrameAsync(element);
        //var data1 = new byte[240 * 240 * 3];
        var data = new EmoticonActionFrame(FaceData);

        var centerX = width / 2;
        var centerY = height / 2;
        if (y == 0)
        {
            data.J1 = 15;
        }

        if (x == 0)
        {
            data.J6 = -90;
        }
        if (x < centerX && x > 0)
        {
            var j6 = -(float)((centerX - x) / (centerX * 1.0) * 90);
            data.J6 = j6;
        }
        else if (x >= centerX)
        {
            var j6 = (float)((x - centerX) / (centerX * 1.0) * 90);
            data.J6 = j6;
        }

        if (y < centerY && y > 0)
        {
            var j1 = (float)((centerY - y) / (centerY * 1.0) * 15);
            data.J1 = j1;
        }
        else if (y >= centerY)
        {
            var j1 = -(float)((y - centerY) / (centerY * 1.0) * 15);
            data.J1 = j1;
        }

        data.Enable = true;

        var str = $"j1:{data.J1} j6:{data.J6} centerX:{centerX}centerY:{centerY}";
        Debug.WriteLine(str);

        //Task.Run(() =>
        //{
        //    if (ElectronBotHelper.Instance.EbConnected)
        //    {
        //        ElectronBotHelper.Instance.PlayEmoticonActionFrame(data);
        //    }
        //});
        //return Task.CompletedTask;

        var service = App.GetService<EmoticonActionFrameService>();
        _ = await service.SendToUsbDeviceAsync(data);
    }

    /// <summary>
    /// 组装表盘数据
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static async Task<EmoticonActionFrame> SetClockUiToFrameAsync(UIElement element)
    {
        var data = new byte[240 * 240 * 3];
        try
        {
            var bitmap = new RenderTargetBitmap();

            await bitmap.RenderAsync(element);

            var pixels = await bitmap.GetPixelsAsync();

            //var canvasDevice = App.GetService<CanvasDevice>();
            // Transfer the pixel data from XAML to Win2D for further processing.
            using var canvasDevice = new CanvasDevice();

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

            Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

        }
        catch (Exception)
        {

        }
        return new EmoticonActionFrame(data);
    }

    /// <summary>
    /// 获取屏幕鼠标坐标
    /// </summary>
    /// <returns></returns>
    public static (int x, int y) GetScreenCursorPos()
    {
        var myPoint = new CursorPoint();
        GetCursorPos(ref myPoint);
        return (myPoint.X, myPoint.Y);
    }

    /// <summary>
    /// 获取屏幕尺寸
    /// </summary>
    /// <param name="hwnd"></param>
    /// <returns></returns>
    public static (int height, int width) GetScreenSize(IntPtr hwnd)
    {
        var monitor = MonitorFromWindow(hwnd, 2);

        var monitorInfo = new MONITORINFO();
        monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
        GetMonitorInfo(monitor, ref monitorInfo);
        var width = monitorInfo.rcMonitor.Width - monitorInfo.rcMonitor.Left;
        var height = monitorInfo.rcMonitor.Height - monitorInfo.rcMonitor.Top;
        return (height, width);
    }

    /// <summary>
    /// 鼠标中键是否被按住
    /// </summary>
    /// <returns></returns>
    public static bool IsVkMButtonEnabled()
    {
        if ((GetKeyState(4) & 0x8000) != 0)
        {
            // 鼠标中键被按下
            return true;
        }
        return false;
    }

    /// <summary>
    /// 语音唤醒组合键是否被按住
    /// </summary>
    /// <returns></returns>
    public static bool IsVoiceEnabled()
    {
          // 检测Space + E是否被按下
         if ((GetAsyncKeyState(0x20) & 0x8000) != 0 && (GetAsyncKeyState((int)'E') & 0x8000) != 0)
         {
             return true;
         }
         return false;
    }
    /*
    GitHub Copilot: 键盘虚拟键码是一个0到255之间的整数，用于表示键盘上的每个按键。以下是一些常见按键的虚拟键码：

    - `VK_BACK`：0x08
    - `VK_TAB`：0x09
    - `VK_RETURN`：0x0D
    - `VK_SHIFT`：0x10
    - `VK_CONTROL`：0x11
    - `VK_MENU`：0x12
    - `VK_PAUSE`：0x13
    - `VK_CAPITAL`：0x14
    - `VK_ESCAPE`：0x1B
    - `VK_SPACE`：0x20
    - `VK_PRIOR`：0x21
    - `VK_NEXT`：0x22
    - `VK_END`：0x23
    - `VK_HOME`：0x24
    - `VK_LEFT`：0x25
    - `VK_UP`：0x26
    - `VK_RIGHT`：0x27
    - `VK_DOWN`：0x28
    - `VK_SELECT`：0x29
    - `VK_PRINT`：0x2A
    - `VK_EXECUTE`：0x2B
    - `VK_SNAPSHOT`：0x2C
    - `VK_INSERT`：0x2D
    - `VK_DELETE`：0x2E
    - `VK_HELP`：0x2F
    - `VK_NUMPAD0`：0x60
    - `VK_NUMPAD1`：0x61
    - `VK_NUMPAD2`：0x62
    - `VK_NUMPAD3`：0x63
    - `VK_NUMPAD4`：0x64
    - `VK_NUMPAD5`：0x65
    - `VK_NUMPAD6`：0x66
    - `VK_NUMPAD7`：0x67
    - `VK_NUMPAD8`：0x68
    - `VK_NUMPAD9`：0x69
    - `VK_MULTIPLY`：0x6A
    - `VK_ADD`：0x6B
    - `VK_SEPARATOR`：0x6C
    - `VK_SUBTRACT`：0x6D
    - `VK_DECIMAL`：0x6E
    - `VK_DIVIDE`：0x6F
    - `VK_F1`：0x70
    - `VK_F2`：0x71
    - `VK_F3`：0x72
    - `VK_F4`：0x73
    - `VK_F5`：0x74
    - `VK_F6`：0x75
    - `VK_F7`：0x76
    - `VK_F8`：0x77
    - `VK_F9`：0x78
    - `VK_F10`：0x79
    - `VK_F11`：0x7A
    - `VK_F12`：0x7B

    注意，这只是一些常见按键的虚拟键码，键盘上还有很多其他按键，每个按键都有自己的虚拟键码。你可以使用`MapVirtualKey`函数将字符值转换为虚拟键码，或者使用`GetAsyncKeyState`函数检测虚拟键码对应的按键是否被按下。

    */
    ///<summary>
    ///返回当前光标的位置(用来更换鼠标位置）
    ///lpPoint-POINTAPI，随同指针在屏幕像素坐标中的位置载入的一个结构
    ///</summary>
    [DllImport("user32.dll", EntryPoint = "GetCursorPos")]

    private static extern int GetCursorPos(ref CursorPoint lpPoint);

    private struct CursorPoint
    {
        public int X;
        public int Y;
        public CursorPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }


    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Height;
        public int Width;
    }

    [DllImport("user32.dll")]
    static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll")]
    public static extern short GetAsyncKeyState(int vKey);
}
