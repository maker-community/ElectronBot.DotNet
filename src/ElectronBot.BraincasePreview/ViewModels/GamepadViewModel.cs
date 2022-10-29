using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.BraincasePreview.Contracts.ViewModels;
using ElectronBot.BraincasePreview.Core.Models;
using ElectronBot.BraincasePreview.Services.EbotGrpcService;
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

    private bool isHoldRightThumbstick = false;

    private bool isReleaseRightThumbstick = false;


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

    private async void DispatcherTimer_Tick(object? sender, object e)
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


            if (reading.Buttons.HasFlag(GamepadButtons.RightThumbstick))
            {
                isHoldRightThumbstick = true;
            }


            if(isHoldRightThumbstick == true && !reading.Buttons.HasFlag(GamepadButtons.RightThumbstick))
            {
                isReleaseRightThumbstick = true;

                isHoldRightThumbstick = false;
            }

            //发送表情
            if (isHoldRightThumbstick == false && isReleaseRightThumbstick == true)
            {
                Debug.Write($"send emojis---{DateTime.Now.ToString()}");

                isReleaseRightThumbstick = false;

                isHoldRightThumbstick = false;
            }

            var init1 = 0;

            var init2 = 0;

            var init3 = 0;

            var init4 = 0;

            var enableA = 0;

            var enableB = 0;

            //左转
            if (reading.LeftThumbstickX < 0)
            {
                init1 = 1;

                init2 = 0;

                init3 = 1;

                init4 = 0;
            }

            //右转

            if (reading.LeftThumbstickX > 0)
            {
                init1 = 0;

                init2 = 1;

                init3 = 0;

                init4 = 1;
            }

            //后退

            if (reading.LeftTrigger > 0)
            {
                init1 = 1;

                init2 = 0;

                init3 = 0;

                init4 = 1;
            }

            //前进

            if (reading.RightTrigger > 0)
            {
                init1 = 0;

                init2 = 1;

                init3 = 1;

                init4 = 0;
            }


            if ((int)reading.LeftThumbstickX == 0 && (int)reading.RightTrigger == 0 && (int)reading.LeftTrigger == 0)
            {
                init1 = 0;

                init2 = 0;

                init3 = 0;

                init4 = 0;
            }

            var data = new MotorControlRequestModel
            {
                Init1 = init1,
                Init2 = init2,
                Init3 = init3,
                Init4 = init4,
                EnableA = enableA,
                EnableB = enableB
            };

            try
            {

                //通过grpc通讯和树莓派传输数据 
                var grpcClient = App.GetService<EbGrpcService>();

                _ = await grpcClient.MotorControlAsync(data);
            }
            catch (Exception)
            {

            }
        }
    }

    public void OnNavigatedFrom()
    {
        _dispatcherTimer.Stop();
    }
}
