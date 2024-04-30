using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Microsoft.UI.Xaml;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75WeatherViewModel : ObservableRecipient
{
    [ObservableProperty]
    private Weather_Displayed? _gpsResult;

    private readonly DispatcherTimer _dispatcherTimer =new ();

    private readonly ILocalSettingsService _localSettingsService;

    [ObservableProperty]
    private string _todayWeek = DateTimeOffset.Now.ToString("ddd");


    [ObservableProperty]
    private string? _day;

    [ObservableProperty]
    private string _todayTime = DateTimeOffset.Now.ToString("t");


    [ObservableProperty]
    private CustomClockTitleConfig? _clockTitleConfig;

    private const string ViewName = "Hw75WeatherView";

    public Hw75WeatherViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;

        _dispatcherTimer.Interval = new TimeSpan(0, 30, 0);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        GpsResult = await GpsGetWeather.GetWeatherIdea();

        Hw75Helper.Instance.InvokeHandler();
    }

    [RelayCommand]
    public async Task OnLoaded()
    {
        _dispatcherTimer.Start();

        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        GpsResult = await GpsGetWeather.GetWeatherIdea();

        Hw75Helper.Instance.InvokeHandler();
    }

    [RelayCommand]
    public void OnUnLoaded()
    {
        _dispatcherTimer.Stop();
    }
}
