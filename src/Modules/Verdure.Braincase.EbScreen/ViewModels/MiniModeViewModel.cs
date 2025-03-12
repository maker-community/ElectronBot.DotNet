using System.Reflection.Metadata.Ecma335;
using Microsoft.UI.Xaml;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Services;
using Verdure.Braincase.ViewModels;

namespace ViewModels;
public partial class MiniModeViewModel : ObservableRecipient
{
    private readonly IElectronBotPlayer _electronBotPlayer;
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IClockViewProviderFactory _viewProviderFactory;
    private readonly DispatcherTimer _timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(200)
    };

    [ObservableProperty] private string _voiceResult = string.Empty;

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
    }

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
        await _electronBotPlayer.PlayImageAsync(Element);
    }

    [RelayCommand]
    public void PlayAction()
    {

    }


    [RelayCommand]
    public async Task OnLoadedAsync()
    {
        var saveClockView = await _localSettingsService
            .ReadSettingAsync<string>(Constants.CurrentClockViewKey);

        var clockView = string.IsNullOrEmpty(saveClockView) ? "DefautView" : saveClockView;
        var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockView);

        Element = viewProvider.CreateClockView(clockView);

        _timer.Start();
        if (Element is UserControl userControl)
        {
            var viewModel = userControl.DataContext as ClockViewModel;
            viewModel?.LoadedCommand.Execute(null);
        }
    }
    [RelayCommand]
    public void OnUnLoaded()
    {
        _timer.Tick -= Timer_Tick;
        _timer.Stop();

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
        else if (modeName == "ClockMode")
        {
         
        }
        else if (modeName == "NeedleMode")
        {
       
        }
        else
        {          
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
