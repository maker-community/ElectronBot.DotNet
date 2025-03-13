using System.Diagnostics;
using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Verdure.Braincase.Contracts.Services;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.Services;
using Windows.Graphics.Imaging;
using Windows.Media.Playback;

namespace Verdure.Braincase.ViewModels;

public partial class HomeViewModel : ObservableRecipient, INavigationAware, IRecipient<ChangeClockView>, IRecipient<ChangeAppMode>
{
    private readonly DispatcherTimer _dispatcherTimer = new();

    private readonly IClockViewProviderFactory _viewProviderFactory;


    private readonly IActionExpressionProviderFactory _expressionProviderFactory;


    private readonly ILocalSettingsService _localSettingsService;

    private readonly IServiceProvider _services;


    private int modeNo = 0;


    private readonly MediaPlayer _mediaPlayer;

    SoftwareBitmap? frameServerDest = null;

    CanvasImageSource? canvasImageSource = null;

    private readonly ElementTheme _elementTheme;

    private GestureAppService _gestureAppService = new();

    private readonly IMemoryCache _memoryCache;

    private readonly IntPtr _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
    public HomeViewModel(
        ILocalSettingsService localSettingsService,
        IClockViewProviderFactory viewProviderFactory,
        ComboxDataService comboxDataService,
        ObjectPickerService objectPickerService,
        MediaPlayer mediaPlayer,
        IActionExpressionProviderFactory actionExpressionProviderFactory,
        IThemeSelectorService elementTheme,
        IMemoryCache memoryCache,
        IServiceProvider services)
    {
        _localSettingsService = localSettingsService;

        _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;

        _viewProviderFactory = viewProviderFactory;

        _expressionProviderFactory = actionExpressionProviderFactory;

        ClockComboxModels = comboxDataService.GetClockViewComboxList();

        _mediaPlayer = mediaPlayer;

        _mediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;

        _mediaPlayer.IsVideoFrameServerEnabled = true;

        var defaultProvider = _expressionProviderFactory.CreateActionExpressionProvider("Default");

        ElectronBotHelper.Instance.SerialPort.DataReceived += SerialPort_DataReceived;

        ElectronBotHelper.Instance.ClockCanvasStop += Instance_ClockCanvasStop;
        ElectronBotHelper.Instance.ClockCanvasStart += Instance_ClockCanvasStart;
        _elementTheme = elementTheme.Theme;

        WeakReferenceMessenger.Default.Register<ChangeClockView>(this);
        WeakReferenceMessenger.Default.Register<ChangeAppMode>(this);
        _memoryCache = memoryCache;
        _services = services;
    }

    private void Instance_ClockCanvasStart(object? sender, EventArgs e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            _dispatcherTimer.Start();
        });
    }

    private void Instance_ClockCanvasStop(object? sender, EventArgs e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            _dispatcherTimer.Stop();
        });
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        var indata = sp.ReadExisting();
        Debug.WriteLine("Data Received:");
        Debug.Write(indata);
    }

    /// <summary>
    /// 媒体播放帧处理事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void MediaPlayer_VideoFrameAvailable(MediaPlayer sender, object args)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                var canvasDevice = Ioc.Default.GetRequiredService<CanvasDevice>();

                if (frameServerDest == null)
                {
                    // FrameServerImage in this example is a XAML image control
                    frameServerDest =
                        new SoftwareBitmap(BitmapPixelFormat.Rgba8, 240, 240, BitmapAlphaMode.Ignore);

                }
                if (canvasImageSource == null)
                {
                    canvasImageSource = new CanvasImageSource(canvasDevice, 240, 240, 96);//96); 

                    EmojiImageSource = canvasImageSource;

                }

                using var inputBitmap = CanvasBitmap.CreateFromSoftwareBitmap(canvasDevice, frameServerDest);

                using var ds = canvasImageSource.CreateDrawingSession(Microsoft.UI.Colors.Black);

                _mediaPlayer.CopyFrameToVideoSurface(inputBitmap);

                ds.DrawImage(inputBitmap);
            }
            catch (Exception ex)
            {

            }
        });
    }

    /// <summary>
    /// 定时器处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        var clockName = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);

        if (clockName == "ClockMode")
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                await EbHelper.ShowClockCanvasToDeviceAsync(Element);
            }
        }
        else if (clockName == "NeedleMode")
        {
            var (x, y) = EbHelper.GetScreenCursorPos();

            var screenSize = EbHelper.GetScreenSize(_hwnd);
            var mdButton = EbHelper.IsVkMButtonEnabled();
            var str = $"height:{screenSize.height}width:{screenSize.width}x:{x}y:{y} mdButton:{mdButton}";
            Debug.WriteLine(str);

            if (mdButton)
            {
                var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                if (mdButton && !playEmojisLock)
                {
                    //随机播放表情
                    ElectronBotHelper.Instance.ToPlayEmojisRandom();
                }

                ElectronBotHelper.Instance.PlayEmojisLock = true;
            }
            else
            {
                if (!ElectronBotHelper.Instance.PlayEmojisLock)
                {
                    if (screenSize.height > screenSize.width)
                    {
                        await EbHelper.ShowClockCanvasAndPosToDeviceAsync(Element, screenSize.width, screenSize.height, x, y);
                    }
                    else
                    {
                        await EbHelper.ShowClockCanvasAndPosToDeviceAsync(Element, screenSize.height, screenSize.width, x, y);
                    }
                }
            }
        }
    }

    private async Task ResetActionAsync()
    {
        J1 = 0;
        J2 = 0;
        J3 = 0;
        J4 = 0;
        J5 = 0;
        J6 = 0;

        await Task.Run(() =>
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                if (ElectronBotHelper.Instance.EbConnected)
                {
                    var data = new byte[240 * 240 * 3];

                    var frame = new EmoticonActionFrame(data, true);

                    var service = Ioc.Default.GetRequiredService<EmoticonActionFrameService>();

                    service.ClearQueue();

                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                }
            }
        });
    }

    public async void OnNavigatedTo(object parameter)
    {
        var saveClockView = await _localSettingsService
            .ReadSettingAsync<string>(Constants.CurrentClockViewKey);

        var clockView = string.IsNullOrEmpty(saveClockView) ? "DefautView" : saveClockView;
        var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockView);

        Element = viewProvider.CreateClockView(clockView);

        var modeName = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);
        if (!string.IsNullOrWhiteSpace(modeName))
        {
            ModeIndex = ModeNameToIndex(modeName);
        }
        _dispatcherTimer.Start();
    }

    public void OnNavigatedFrom()
    {
        _dispatcherTimer.Tick += DispatcherTimer_Tick;
        _dispatcherTimer.Stop();
    }

    public void Receive(ChangeClockView view)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            var clockName = view.ClockViewName;
            if (!string.IsNullOrWhiteSpace(clockName))
            {
                var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

                service.ClearQueue();

                var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockName);

                if (clockName == "GooeyFooter" || clockName == "CustomView" || clockName == "GrooveView")
                {
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
                }
                else
                {
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                }

                Element = viewProvider.CreateClockView(clockName);

                await _localSettingsService.SaveSettingAsync(Constants.CurrentClockViewKey, clockName);

                if (!_dispatcherTimer.IsEnabled)
                {
                    _dispatcherTimer.Start();
                }
            }
        });
    }

    public void Receive(ChangeAppMode mode)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            var modeName = mode.ModeName;

            if (!string.IsNullOrWhiteSpace(modeName))
            {
                var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

                service.ClearQueue();

                await ChangeAppModeAsync(modeName);
            }
        });
    }
}
