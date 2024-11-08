using Microsoft.UI.Xaml;
using Verdure.Braincase.ViewModels;

namespace ViewModels;
public partial class MiniModeViewModel : ObservableRecipient
{
    private readonly IElectronBotPlayer _electronBotPlayer;
    private readonly DispatcherTimer _timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(200)
    };

    [ObservableProperty] private string _voiceResult = string.Empty;

    public MiniModeViewModel(IElectronBotPlayer electronBotPlayer)
    {

        _timer.Tick += Timer_Tick;
        _electronBotPlayer = electronBotPlayer;
    }

    /// <summary>
    /// 表盘内容
    /// </summary>
    [ObservableProperty]
    UIElement _element;

    private async void Timer_Tick(object sender, object e)
    {
        await _electronBotPlayer.PlayImageAsync(Element);
    }

    [RelayCommand]
    public void PlayAction()
    {

    }


    [RelayCommand]
    public void OnLoaded()
    {
        var _viewProviderFactory = Ioc.Default.GetRequiredService<IClockViewProviderFactory>();
        var viewProvider = _viewProviderFactory.CreateClockViewProvider("DefautView");
        Element = viewProvider.CreateClockView("DefautView");
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
    public void CompactOverlay()
    {
        var provider = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow();
        provider.Show();
    }
}
