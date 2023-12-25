using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75WeatherViewModel : ObservableRecipient
{
    [ObservableProperty]
    private Weather_Displayed? _gpsResult;

    private readonly DispatcherTimer _dispatcherTimer;

    private readonly ILocalSettingsService _localSettingsService;

    [ObservableProperty]
    private string _todayWeek = DateTimeOffset.Now.ToString("ddd");


    [ObservableProperty]
    private string? _day;

    [ObservableProperty]
    private string _todayTime = DateTimeOffset.Now.ToString("t");


    [ObservableProperty]
    private CustomClockTitleConfig? _clockTitleConfig;

    public Hw75WeatherViewModel(DispatcherTimer dispatcherTimer,
    ILocalSettingsService localSettingsService
    )
    {
        _dispatcherTimer = dispatcherTimer;
        _localSettingsService = localSettingsService;

        _dispatcherTimer.Interval = new TimeSpan(0, 0, 50);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private void DispatcherTimer_Tick(object? sender, object e)
    {
        TodayTime = DateTimeOffset.Now.ToString("t");
        TodayWeek = DateTimeOffset.Now.ToString("ddd");
        Day = DateTimeOffset.Now.Day.ToString();

    }

    [RelayCommand]
    private async Task OnLoaded()
    {
        _dispatcherTimer.Start();

        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        GpsResult = await GpsGetWeather.GetWeatherIdea();
    }
}
