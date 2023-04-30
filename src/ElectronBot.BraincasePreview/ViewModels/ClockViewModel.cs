using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.Braincase.ViewModels;

public class ClockViewModel : ObservableRecipient
{
    private readonly DispatcherTimer _dispatcherTimer;

    private readonly ILocalSettingsService _localSettingsService;

    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

    private string _todayWeek = DateTimeOffset.Now.ToString("ddd");

    private ClockDiagnosticInfo _clockDiagnosticInfo = new();

    private string _day = DateTimeOffset.Now.Day.ToString();

    private readonly ClockDiagnosticService _diagnosticService;

    private bool isProcessing = false;


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

    private string _todayTime = DateTimeOffset.Now.ToString("T");

    public string TodayTime
    {
        get => _todayTime;
        set => SetProperty(ref _todayTime, value);
    }
    private async void OnLoaded()
    {
        _dispatcherTimer.Start();

        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        _diagnosticService.ClockDiagnosticInfoResult += DiagnosticService_ClockDiagnosticInfoResult;
    }

    private void DiagnosticService_ClockDiagnosticInfoResult(object? sender, ClockDiagnosticInfo e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ClockDiagnosticInfo = e ?? new ClockDiagnosticInfo();
        });    
    }

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        TodayTime = DateTimeOffset.Now.ToString("T");
        TodayWeek = DateTimeOffset.Now.ToString("ddd");
        Day = DateTimeOffset.Now.Day.ToString();

        _ = await _diagnosticService.InvokeClockViewAsync(sender!);
    }

    public ClockViewModel(DispatcherTimer dispatcherTimer,
        ClockDiagnosticService clockDiagnosticService,
        ILocalSettingsService localSettingsService
        )
    {
        _dispatcherTimer = dispatcherTimer;
        _diagnosticService = clockDiagnosticService;
        _localSettingsService = localSettingsService;

        _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

}
