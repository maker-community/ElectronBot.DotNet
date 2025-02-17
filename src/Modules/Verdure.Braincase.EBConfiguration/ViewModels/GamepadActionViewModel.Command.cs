using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.WinUI.Common.Contracts.Services;
using Windows.Gaming.Input;

namespace Verdure.Braincase.EBConfiguration.ViewModels;
public partial class GamepadActionViewModel : ObservableRecipient
{
    [RelayCommand]
    public void OnLoaded()
    {
        Gamepad.GamepadAdded += Gamepad_GamepadAdded;
        Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
        _dispatcherTimer.Start();
        _dispatcherTimer.Interval = new TimeSpan(100);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }
    [RelayCommand]
    public void OnUnLoaded()
    {
        _dispatcherTimer.Stop();

        Gamepad.GamepadAdded -= Gamepad_GamepadAdded;
        Gamepad.GamepadRemoved -= Gamepad_GamepadRemoved;
    }
    [RelayCommand]
    private void GamepadChanged(object? obj)
    {
        if (obj is ComboxItemModel model)
        {
            _selectItem = model;
        }
    }
}
