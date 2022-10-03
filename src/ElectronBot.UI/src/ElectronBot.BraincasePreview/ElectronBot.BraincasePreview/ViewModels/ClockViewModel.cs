using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Models;
using ElectronBot.BraincasePreview.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.BraincasePreview.ViewModels;

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
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;

        _dispatcherTimer.Start();

        //var ret = await _localSettingsService.ReadSettingAsync<string>(Constants.CustomClockTitleKey);

        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        //CustomClockTitle = ret ?? "";

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        await Task.CompletedTask;
    }

    private void DispatcherTimer_Tick(object? sender, object e)
    {
        TodayTime = DateTimeOffset.Now.ToString("T");
        TodayWeek = DateTimeOffset.Now.ToString("ddd");
        Day = DateTimeOffset.Now.Day.ToString();

        ClockDiagnosticInfo = _diagnosticService.GetClockDiagnosticInfoAsync();
    }

    public ClockViewModel(DispatcherTimer dispatcherTimer,
        ClockDiagnosticService clockDiagnosticService,
        ILocalSettingsService localSettingsService
        )
    {
        _dispatcherTimer = dispatcherTimer;
        _diagnosticService = clockDiagnosticService;
        _localSettingsService = localSettingsService;
    }

}
