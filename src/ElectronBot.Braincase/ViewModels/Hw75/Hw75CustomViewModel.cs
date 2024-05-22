using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75CustomViewModel : ObservableRecipient
{
    private readonly DispatcherTimer _dispatcherTimer = new();

    private readonly ILocalSettingsService _localSettingsService;

    private string _todayWeek = DateTimeOffset.Now.ToString("ddd");

    private ClockDiagnosticInfo _clockDiagnosticInfo = new();

    private string _day = DateTimeOffset.Now.Day.ToString();

    private readonly ClockDiagnosticService _diagnosticService;

    private const string ViewName = "Hw75CustomView";

    [ObservableProperty] private Visibility _customContentVisibility = Visibility.Visible;

    [ObservableProperty] private Visibility _dateVisibility = Visibility.Visible;



    public Hw75CustomViewModel(ClockDiagnosticService clockDiagnosticService,
        ILocalSettingsService localSettingsService)
    {
        _diagnosticService = clockDiagnosticService;
        _localSettingsService = localSettingsService;

        _dispatcherTimer.Interval = new TimeSpan(0, 0, 60);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    public string TodayWeek
    {
        get => _todayWeek;
        set => SetProperty(ref _todayWeek, value);
    }

    public string Day
    {
        get => _day;
        set => SetProperty(ref _day, value);
    }

    private string _customClockTitle;

    private CustomClockTitleConfig _clockTitleConfig;

    public string CustomClockTitle
    {
        get => _customClockTitle;
        set => SetProperty(ref _customClockTitle, value);
    }

    public CustomClockTitleConfig ClockTitleConfig
    {
        get => _clockTitleConfig;
        set => SetProperty(ref _clockTitleConfig, value);
    }

    public ClockDiagnosticInfo ClockDiagnosticInfo
    {
        get => _clockDiagnosticInfo;
        set => SetProperty(ref _clockDiagnosticInfo, value);
    }

    private string _todayTime = DateTimeOffset.Now.ToString("t");

    public string TodayTime
    {
        get => _todayTime;
        set => SetProperty(ref _todayTime, value);
    }

    [RelayCommand]
    public async Task OnLoaded()
    {
        _dispatcherTimer.Start();

        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        CustomContentVisibility = ClockTitleConfig.Hw75CustomContentIsVisibility ? Visibility.Visible : Visibility.Collapsed;

        DateVisibility = ClockTitleConfig.Hw75TimeIsVisibility ? Visibility.Visible : Visibility.Collapsed;
        _diagnosticService.ClockDiagnosticInfoResult += DiagnosticService_ClockDiagnosticInfoResult;

        Hw75Helper.Instance.InvokeHandler();
    }

    [RelayCommand]
    public void OnUnLoaded()
    {
        _dispatcherTimer.Stop();
    }

    private void DiagnosticService_ClockDiagnosticInfoResult(object? sender, ClockDiagnosticInfo e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ClockDiagnosticInfo = e ?? new ClockDiagnosticInfo();
        });
    }

    private void DispatcherTimer_Tick(object? sender, object e)
    {
        TodayTime = DateTimeOffset.Now.ToString("t");
        TodayWeek = DateTimeOffset.Now.ToString("ddd");
        Day = DateTimeOffset.Now.Day.ToString();

        Hw75Helper.Instance.InvokeHandler();
    }

}
