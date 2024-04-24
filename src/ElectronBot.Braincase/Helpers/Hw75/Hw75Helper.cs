using System.Runtime.InteropServices.WindowsRuntime;
using ElectronBot.Braincase.Contracts.Services;
using HelloWordKeyboard.DotNet;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Models;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Verdure.ElectronBot.Core.Models;
using Verdure.IoT.Net;
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

    public bool IsHaSwitchOn
    {
        get; set;
    }

    public string ViewName { get; set; } = "Hw75CustomView";

    public DispatcherTimer Timer { get; set; } = new DispatcherTimer();


    public event EventHandler? UpdateDataToDeviceHandler;

    private static Hw75Helper? _instance;
    public static Hw75Helper Instance => _instance ??= new Hw75Helper();

    private readonly SynchronizationContext? _context = SynchronizationContext.Current;

    public Hw75Helper()
    {
        try
        {
            Hw75DynamicDevice = new Hw75DynamicDevice();

            Timer.Interval = TimeSpan.FromMilliseconds(20);

            Timer.Tick += DispatcherTimer_Tick;
        }
        catch
        {

        }

    }


    /// <summary>
    /// 触发更新事件
    /// </summary>
    public void InvokeHandler()
    {
        UpdateDataToDeviceHandler?.Invoke(this, new EventArgs());
    }

    public void StartTimer()
    {
        Timer.Start();
    }

    public void StopTimer()
    {
        Timer.Stop();
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

                var dpi = App.MainWindow.GetDpiForWindow();

                using var stream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight,
                    dpi,
                    dpi,
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

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        try
        {
            var state = Hw75Helper.Instance.Hw75DynamicDevice?.GetMotorState();

            if (state != null)
            {
                var degrees = state.CurrentAngle * (180 / Math.PI);

                var targetDegrees = state.TargetAngle * (180 / Math.PI);

                var tmp = false;

                if (degrees > 190 && degrees < 225 && Math.Abs(targetDegrees - degrees) < 3)
                {
                    tmp = false;
                    if (tmp != IsHaSwitchOn)
                    {
                        IsHaSwitchOn = tmp;
                        await HaSwitchAsync(IsHaSwitchOn);
                    }
                }
                else if (degrees < 150 && degrees > 135 && Math.Abs(targetDegrees - degrees) < 3)
                {
                    tmp = true;
                    if (tmp != IsHaSwitchOn)
                    {
                        IsHaSwitchOn = tmp;
                        await HaSwitchAsync(IsHaSwitchOn);
                    }
                }
            }
        }
        catch { }
    }

    public async Task HaSwitchAsync(bool isOn)
    {
        var localSettingsService = App.GetService<ILocalSettingsService>();

        var haSwitchModel = await localSettingsService.ReadSettingAsync<ComboxItemModel>(Constants.DefaultHaSwitchNameKey);

        var haSetting = await localSettingsService.ReadSettingAsync<HaSetting>(Constants.HaSettingKey);

        if (haSwitchModel != null && haSetting != null)
        {
            try
            {
                var client = new HomeAssistantClient(haSetting.BaseUrl, haSetting.HaToken);

                if (isOn)
                {
                    await client.PostServiceAync(haSwitchModel?.DataKey?.Split('.')[0] ?? "", "turn_on", haSwitchModel?.DataKey ?? "");
                }
                else
                {
                    await client.PostServiceAync(haSwitchModel?.DataKey?.Split('.')[0] ?? "", "turn_off", haSwitchModel?.DataKey ?? "");
                }
            }
            catch { }
        }
    }

}
