using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using SixLabors.ImageSharp;
using Verdure.Braincase.Services;
using Verdure.Braincase.ViewModels;
using Verdure.Braincase.WinUI.Common.Players;

namespace ViewModels;
public partial class MiniModeViewModel : ObservableRecipient, IRecipient<LottieFrameRenderedEventArgs>, IDisposable
{
    private readonly IElectronBotPlayer _electronBotPlayer;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IClockViewProviderFactory _viewProviderFactory;
    private readonly DispatcherTimer _timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(200)
    };

    private readonly DispatcherQueue _dispatcherQueue;
    private bool _isDisposed;

    private readonly IntPtr _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow());

    public MiniModeViewModel(IElectronBotPlayer electronBotPlayer,
        ILocalSettingsService localSettingsService,
        IClockViewProviderFactory viewProviderFactory,
        ComboxDataService comboxDataService)
    {
        _timer.Tick += Timer_Tick;
        _electronBotPlayer = electronBotPlayer;
        _localSettingsService = localSettingsService;
        _viewProviderFactory = viewProviderFactory;
        ClockComboxModels = comboxDataService.GetClockViewComboxList();
        WeakReferenceMessenger.Default.Register<LottieFrameRenderedEventArgs>(this);
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    // 使用部分属性实现替代 ObservableProperty 装饰的字段
    private string _voiceResult = string.Empty;
    public string VoiceResult
    {
        get => _voiceResult;
        set => SetProperty(ref _voiceResult, value);
    }

    private int _modeIndex;
    public int ModeIndex
    {
        get => _modeIndex;
        set => SetProperty(ref _modeIndex, value);
    }

    /// <summary>
    /// 表盘内容
    /// </summary>
    private UIElement? _element;
    public UIElement? Element
    {
        get => _element;
        set => SetProperty(ref _element, value);
    }

    /// <summary>
    /// 时钟选中数据
    /// </summary>
    private ComboxItemModel? _clockComBoxSelect;
    public ComboxItemModel? ClockComBoxSelect
    {
        get => _clockComBoxSelect;
        set => SetProperty(ref _clockComBoxSelect, value);
    }

    /// <summary>
    /// 表盘列表
    /// </summary>
    private ObservableCollection<ComboxItemModel> _clockComboxModels = new();
    public ObservableCollection<ComboxItemModel> ClockComboxModels
    {
        get => _clockComboxModels;
        set => SetProperty(ref _clockComboxModels, value);
    }

    private async void Timer_Tick(object? sender, object? e)
    {
        if (_isDisposed || Element == null)
            return;

        try
        {
            var clockName = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);

            if (clockName == "ClockMode")
            {
                await _electronBotPlayer.PlayImageAsync(Element);
            }
            else if (clockName == "NeedleMode")
            {
                var (x, y) = EbHelper.GetScreenCursorPos();

                var screenSize = EbHelper.GetScreenSize(_hwnd);

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
        catch (ObjectDisposedException)
        {
            // 处理对象已释放异常
            _timer.Stop();
        }
        catch (Exception)
        {
            // 处理其他异常
        }
    }

    [RelayCommand]
    public void PlayEmojis()
    {
        if (_isDisposed) return;
        _ = _electronBotPlayer.PlayLottieByNameIdAsync("think", -1);
    }

    public void Receive(LottieFrameRenderedEventArgs message)
    {
        if (_isDisposed) return;

        _dispatcherQueue.TryEnqueue(async () =>
        {
            if (_isDisposed) return;

            try
            {
                if (message.Image != null)
                {
                    var clockName = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);

                    if (clockName == "NaturalMode")
                    {
                        var image = await ConvertToWinUIImageAsync(message.Image);

                        // 创建一个 ViewBox 来包裹图像，保持纵横比
                        var viewBox = new Viewbox
                        {
                            Width = 200,  // 设置固定宽度
                            Height = 200, // 设置固定高度
                            Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform,
                            Child = image
                        };

                        Element = viewBox;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // 处理对象已释放异常
            }
            catch (Exception)
            {
                // 处理其他异常
            }
        });
    }

    public async Task<Microsoft.UI.Xaml.Controls.Image> ConvertToWinUIImageAsync(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgra32> imageSharpImage)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(MiniModeViewModel));

        // Create a new WinUI Image control
        var winUIImage = new Microsoft.UI.Xaml.Controls.Image();

        // Convert the ImageSharp image to a memory stream
        using (var memoryStream = new MemoryStream())
        {
            // Save the image to the memory stream as PNG
            await imageSharpImage.SaveAsPngAsync(memoryStream);

            // Reset stream position
            memoryStream.Position = 0;

            // Create a BitmapImage
            var bitmapImage = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage();

            // Set the stream as the source
            await bitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());

            // Set the BitmapImage as the source of the WinUI Image
            winUIImage.Source = bitmapImage;
        }

        return winUIImage;
    }

    [RelayCommand]
    public async Task OnLoadedAsync()
    {
        if (_isDisposed) return;

        _timer.Start();

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
    }

    [RelayCommand]
    public void OnUnLoaded()
    {
        if (_isDisposed) return;

        _timer.Tick -= Timer_Tick;
        _timer.Stop();
        // 根据选中的RadioButton执行相应的逻辑
        var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();
        service.ClearQueue();
        if (Element is UserControl userControl)
        {
            var viewModel = userControl.DataContext as ClockViewModel;
            viewModel?.UnLoadedCommand.Execute(null);
        }
    }

    [RelayCommand]
    public async Task OnRadioButtonSelectionChangedAsync(object parameter)
    {
        if (_isDisposed) return;

        // 处理切换事件的逻辑
        var selectedRadioButton = parameter as RadioButton;
        if (selectedRadioButton != null)
        {
            await ChangeAppModeAsync(selectedRadioButton.Name);
        }
    }

    private async Task ChangeAppModeAsync(string modeName)
    {
        if (_isDisposed) return;

        // 根据选中的RadioButton执行相应的逻辑
        var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

        service.ClearQueue();

        await _localSettingsService.SaveSettingAsync(Constants.CurrentModeKey, modeName);

        ModeIndex = ModeNameToIndex(modeName);

        if (modeName == "NaturalMode")
        {
            // 什么也不做
        }
        else if (modeName == "ClockMode" || modeName == "NeedleMode")
        {
            _timer.Start();

            var saveClockView = await _localSettingsService
                .ReadSettingAsync<string>(Constants.CurrentClockViewKey);

            var clockView = string.IsNullOrEmpty(saveClockView) ? "DefautView" : saveClockView;
            var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockView);

            Element = viewProvider.CreateClockView(clockView);

            if (Element is UserControl userControl)
            {
                var viewModel = userControl.DataContext as ClockViewModel;
                viewModel?.LoadedCommand.Execute(null);
            }
        }
        else
        {
            _timer.Stop();
        }
    }

    [RelayCommand]
    public void CompactOverlay()
    {
        if (_isDisposed) return;

        var provider = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow();
        provider.Show();
    }

    /// <summary>
    /// 表盘切换方法
    /// </summary>
    [RelayCommand]
    private async Task ClockChanged()
    {
        if (_isDisposed) return;

        var clockName = ClockComBoxSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(clockName))
        {
            var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

            service.ClearQueue();

            var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockName);

            if (clockName == "GooeyFooter" || clockName == "CustomView" || clockName == "GrooveView")
            {
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            }
            else
            {
                _timer.Interval = new TimeSpan(0, 0, 1);
            }

            Element = viewProvider.CreateClockView(clockName);

            await _localSettingsService
                .SaveSettingAsync(Constants.CurrentClockViewKey, clockName);
        }
    }

    private int ModeNameToIndex(string modeName)
    {
        if (modeName == "NaturalMode")
        {
            return 0;
        }
        else if (modeName == "ClockMode")
        {
            return 1;
        }
        else if (modeName == "NeedleMode")
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            _timer.Stop();
            _timer.Tick -= Timer_Tick;

            WeakReferenceMessenger.Default.Unregister<LottieFrameRenderedEventArgs>(this);

            // 释放元素资源
            _element = null;

            GC.SuppressFinalize(this);
        }
    }
}
