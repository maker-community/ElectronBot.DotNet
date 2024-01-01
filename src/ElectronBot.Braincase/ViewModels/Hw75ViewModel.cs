using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Services;
using HelloWordKeyboard.DotNet;
using HelloWordKeyboard.DotNet.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Verdure.ElectronBot.Core.Models;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75ViewModel : ObservableRecipient, INavigationAware
{
    /// <summary>
    /// eink content
    /// </summary>
    [ObservableProperty]
    UIElement? _element;

    /// <summary>
    /// 时钟选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel? clockComBoxSelect;

    /// <summary>
    /// 瀚文设备信息
    /// </summary>
    [ObservableProperty]
    DeviceInfo? _deviceInfo;

    /// <summary>
    /// 固件版本
    /// </summary>
    [ObservableProperty]
    string? _firmwareVersion;

    /// <summary>
    /// ZMK版本
    /// </summary>
    [ObservableProperty]
    string? _zmkVersion;

    /// <summary>
    /// Zephyr版本
    /// </summary>
    [ObservableProperty]
    string? _zephyrVersion;

    /// <summary>
    /// 瀚文界面列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> clockComboxModels;

    private readonly IHw75DynamicViewProviderFactory _viewProviderFactory;

    private readonly DispatcherTimer _dispatcherTimer = new();

    public Hw75ViewModel(ComboxDataService comboxDataService, IHw75DynamicViewProviderFactory viewProviderFactory)
    {
        ClockComboxModels = comboxDataService.GetHw75ViewComboxList();
        _viewProviderFactory = viewProviderFactory;
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 50);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        if (Hw75Helper.Instance.IsConnected)
        {
            try
            {
                var renderTargetBitmap = new RenderTargetBitmap();

                await renderTargetBitmap.RenderAsync(Element);

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


    /// <summary>
    /// 表盘切换方法
    /// </summary>
    [RelayCommand]
    private void ClockChanged()
    {
        var clockName = ClockComBoxSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(clockName))
        {
            var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider(clockName);

            Element = viewProvider.CreateHw75DynamickView(clockName);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider("Hw75CustomView");

        Element = viewProvider.CreateHw75DynamickView("Hw75CustomView");
        try
        {
            DeviceInfo = Hw75Helper.Instance.Hw75DynamicDevice?.Open();

            Hw75Helper.Instance.IsConnected = true;

            var firmwareInfo = Hw75Helper.Instance.Hw75DynamicDevice?.GetVersion();

            ZmkVersion = firmwareInfo?.ZmkVersion;

            FirmwareVersion = firmwareInfo?.AppVersion;

            ZephyrVersion = firmwareInfo?.ZephyrVersion;
        }
        catch (Exception ex)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(2));
            });

            DeviceInfo = new DeviceInfo
            {
                DeviceName = ex.Message
            };

            Hw75Helper.Instance.IsConnected = false;
        }

        _dispatcherTimer.Start();
    }
    public void OnNavigatedFrom()
    {
        Hw75Helper.Instance.Hw75DynamicDevice?.Close();
        Hw75Helper.Instance.IsConnected = false;
        _dispatcherTimer.Stop();
    }
}
