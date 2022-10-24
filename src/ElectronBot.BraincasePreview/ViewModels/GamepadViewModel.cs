using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.BraincasePreview.Contracts.ViewModels;
using Microsoft.UI.Xaml;
using Verdure.ElectronBot.Core.Models;
using Windows.Gaming.Input;

namespace ElectronBot.BraincasePreview.ViewModels;

public class GamepadViewModel : ObservableRecipient, INavigationAware
{
    private Gamepad? _controller;

    private ICommand _gamePadSelectCommand;
    public ICommand GamePadSelectCommand => _gamePadSelectCommand ??= new RelayCommand<object>(GamepadChanged);

    private ObservableCollection<ComboxItemModel> _gamePads = new();

    private ComboxItemModel? _selectItem;

    readonly DispatcherTimer _dispatcherTimer = new();

    private string _leftX;

    private string _leftY;

    private string _rightX;

    private string _rightY;

    private string _pbLeft;

    private string _pbRight;

    private double _leftThumbstickX;

    private double _leftThumbstickY;

    private double _rightThumbstickX;

    private double _rightThumbstickY;


    public GamepadViewModel()
    {
        Gamepad.GamepadAdded += Gamepad_GamepadAdded;
        Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;
    }

    public string LeftX
    {
        get => _leftX;
        set => SetProperty(ref _leftX, value);
    }

    public string LeftY
    {
        get => _leftY;
        set => SetProperty(ref _leftY, value);
    }

    public string RightX
    {
        get => _rightX;
        set => SetProperty(ref _rightX, value);
    }

    public string RightY
    {
        get => _rightY;
        set => SetProperty(ref _rightY, value);
    }

    public string PbLeft
    {
        get => _pbLeft;
        set => SetProperty(ref _pbLeft, value);
    }

    public string PbRight
    {
        get => _pbRight;
        set => SetProperty(ref _pbRight, value);
    }

    public double LeftThumbstickX
    {
        get => _leftThumbstickX;
        set => SetProperty(ref _leftThumbstickX, value);
    }

    public double LeftThumbstickY
    {
        get => _leftThumbstickY;
        set => SetProperty(ref _leftThumbstickY, value);
    }

    public double RightThumbstickX
    {
        get => _rightThumbstickX;
        set => SetProperty(ref _rightThumbstickX, value);
    }
    public double RightThumbstickY
    {
        get => _rightThumbstickY;
        set => SetProperty(ref _rightThumbstickY, value);
    }

    private void GamepadChanged(object? obj)
    {
        if (obj is ComboxItemModel model)
        {
            _selectItem = model;
        }
    }

    /// <summary>
    /// 设备列表
    /// </summary>
    public ObservableCollection<ComboxItemModel> GamePads
    {
        get => _gamePads;
        set => SetProperty(ref _gamePads, value);
    }

    private void Gamepad_GamepadRemoved(object? sender, Gamepad e)
    {

    }
    private void Gamepad_GamepadAdded(object? sender, Gamepad e)
    {
        _controller = Gamepad.Gamepads.FirstOrDefault();
    }

    public void OnNavigatedTo(object parameter)
    {
        _dispatcherTimer.Start();

        _dispatcherTimer.Interval = new TimeSpan(100);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;
    }

    private void DispatcherTimer_Tick(object? sender, object e)
    {

        if (_controller != null)
        {
            var reading = _controller.GetCurrentReading();

            LeftX = reading.LeftThumbstickX.ToString();
            LeftY = reading.LeftThumbstickY.ToString();
            RightX = reading.RightThumbstickX.ToString();
            RightY = reading.RightThumbstickY.ToString();
            PbLeft = reading.LeftTrigger.ToString();
            PbRight = reading.RightTrigger.ToString();


            LeftThumbstickX = reading.LeftThumbstickX;
            LeftThumbstickY = reading.LeftThumbstickY;

            RightThumbstickX = reading.RightThumbstickX;
            RightThumbstickY = reading.RightThumbstickY;
        }
    }

    public void OnNavigatedFrom()
    {
        _dispatcherTimer.Stop();
    }
}
