using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Services;
using HelloWordKeyboard.DotNet;
using HidApi;
using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Verdure.ElectronBot.Core.Models;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage.Streams;
using static Mediapipe.Net.Framework.Protobuf.Rasterization.Types;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75ViewModel : ObservableRecipient,INavigationAware
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
    /// 表盘列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> clockComboxModels;

    private readonly IClockViewProviderFactory _viewProviderFactory;

    private readonly DispatcherTimer _dispatcherTimer;

    private readonly IHw75DynamicDevice _hw75DynamicDevice;

    public Hw75ViewModel(ComboxDataService comboxDataService, IClockViewProviderFactory viewProviderFactory
        , DispatcherTimer dispatcherTimer, IHw75DynamicDevice hw75DynamicDevice)
    {
        ClockComboxModels = comboxDataService.GetClockViewComboxList();
        _viewProviderFactory = viewProviderFactory;
        _dispatcherTimer = dispatcherTimer;
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 20);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
        _hw75DynamicDevice = hw75DynamicDevice;
    }

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        try
        {
            var bitmap = new RenderTargetBitmap();

        await bitmap.RenderAsync(Element);

        var pixels = await bitmap.GetPixelsAsync();

        using var canvasDevice = new CanvasDevice();

        using var canvasBitmap = CanvasBitmap.CreateFromBytes(
            canvasDevice, pixels.ToArray(), bitmap.PixelWidth, bitmap.PixelHeight,
            Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized);

        using IRandomAccessStream stream = new InMemoryRandomAccessStream();

        await canvasBitmap.SaveAsync(stream, CanvasBitmapFileFormat.Png);

        using var image =  SixLabors.ImageSharp.Image.Load<Rgba32>(stream.AsStream());

        image.Mutate(x => 
        {
            x.Resize(128, 296);
            x.Grayscale();
        });

        var byteArray = image.EnCodeImageToBytes();

   
            _ = _hw75DynamicDevice.SetEInkImage(byteArray, 0, 0, 128, 296, false);
        }
        catch(Exception ex)
        { 
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
            var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockName);

            Element = viewProvider.CreateClockView(clockName);
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        var viewProvider = _viewProviderFactory.CreateClockViewProvider("LongShadowView");

        Element = viewProvider.CreateClockView("LongShadowView");

        //_dispatcherTimer.Start();

        try
        {
            _hw75DynamicDevice.Open();
        }
        catch(Exception ex)
        {
        }
       
    }
    public void OnNavigatedFrom()
    {
        _dispatcherTimer.Stop();
        Hid.Exit();
    }
}
