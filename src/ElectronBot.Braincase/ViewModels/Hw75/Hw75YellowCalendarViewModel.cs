﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Microsoft.UI.Xaml;
using Models.Hw75.YellowCalendar;
using Services.Hw75Services.YellowCalendar;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75YellowCalendarViewModel : ObservableRecipient
{
    [ObservableProperty]
    private YellowCalendarResult? _yellowCalendarResult;
        
    public Hw75YellowCalendarViewModel()
    {
    }

    [RelayCommand]
    private async Task OnLoaded()
    {
        YellowCalendarResult = await GetYellowCalendarService.GetYellowCalendarAsync();
    }
}
