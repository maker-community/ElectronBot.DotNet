using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using SixLabors.ImageSharp;
using Verdure.Braincase.Services;
using Verdure.Braincase.ViewModels;
using Verdure.Braincase.WinUI.Common.Players;

namespace ViewModels;
public partial class MiniModeViewModel : ObservableRecipient, IRecipient<LottieFrameRenderedEventArgs>
{
    private readonly IElectronBotPlayer _electronBotPlayer;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IClockViewProviderFactory _viewProviderFactory;
    private readonly DispatcherTimer _timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(200)
    };

    private readonly DispatcherQueue _dispatcherQueue;

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

    [ObservableProperty]
    string _voiceResult = string.Empty;

    [ObservableProperty]
    int modeIndex;

    /// <summary>
    /// 表盘内容
    /// </summary>
    [ObservableProperty]
    UIElement _element;

    /// <summary>
    /// 时钟选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel? _clockComBoxSelect;

    /// <summary>
    /// 表盘列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> clockComboxModels;

    private async void Timer_Tick(object sender, object e)
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

    [RelayCommand]
    public void PlayEmojis()
    {
        _ = _electronBotPlayer.PlayLottieByNameIdAsync("think", -1);
    }

    public void Receive(LottieFrameRenderedEventArgs message)
    {
        _dispatcherQueue.TryEnqueue(async () =>
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
        });
    }

    public async Task<Microsoft.UI.Xaml.Controls.Image> ConvertToWinUIImageAsync(SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgra32> imageSharpImage)
    {
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
            var bitmapImage = new BitmapImage();

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
        // 处理切换事件的逻辑
        var selectedRadioButton = parameter as RadioButton;
        if (selectedRadioButton != null)
        {
            await ChangeAppModeAsync(selectedRadioButton.Name);
        }
    }

    private async Task ChangeAppModeAsync(string modeName)
    {
        // 根据选中的RadioButton执行相应的逻辑
        var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

        service.ClearQueue();

        await _localSettingsService.SaveSettingAsync(Constants.CurrentModeKey, modeName);

        ModeIndex = ModeNameToIndex(modeName);

        if (modeName == "NaturalMode")
        {

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
        var provider = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow();
        provider.Show();
    }

    /// <summary>
    /// 表盘切换方法
    /// </summary>
    [RelayCommand]
    private async Task ClockChanged()
    {
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
}
