using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Helpers;
using Microsoft.UI.Xaml;
using Models.Hw75.YellowCalendar;
using Services.Hw75Services.YellowCalendar;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75YellowCalendarViewModel : ObservableRecipient
{
    [ObservableProperty]
    private YellowCalendarResult? _yellowCalendarResult;

    private readonly DispatcherTimer _dispatcherTimer = new();

    private const string ViewName = "Hw75YellowCalendarView";

    public Hw75YellowCalendarViewModel()
    {
        _dispatcherTimer.Interval = new TimeSpan(1, 0, 0);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        YellowCalendarResult = await GetYellowCalendarService.GetYellowCalendarAsync(true);

        Hw75Helper.Instance.InvokeHandler();
    }

    [RelayCommand]
    public async Task OnLoaded()
    {
        _dispatcherTimer.Start();

        YellowCalendarResult = await GetYellowCalendarService.GetYellowCalendarAsync();

        Hw75Helper.Instance.InvokeHandler();
    }

    [RelayCommand]
    public void OnUnLoaded()
    {
        _dispatcherTimer.Stop();
    }
}
