using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.UI.Xaml;
using Verdure.ElectronBot.Core.Models;
using Windows.ApplicationModel;
using Windows.Gaming.Input;

namespace ElectronBot.Braincase.ViewModels;

public class GamepadViewModel : ObservableRecipient, INavigationAware
{
    private static Gamepad? _controller;

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

    float j1 = 0, j2 = 0, j3 = 0, j4 = 0, j5 = 0, j6 = 0;



    private int btnCount = 0;
    public GamepadViewModel()
    {
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
        _controller = null;
    }
    private void Gamepad_GamepadAdded(object? sender, Gamepad e)
    {
        _controller = Gamepad.Gamepads.FirstOrDefault();
    }

    public void OnNavigatedTo(object parameter)
    {
        Gamepad.GamepadAdded += Gamepad_GamepadAdded;
        Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;

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

            //左边的推杆 值范围 -1 0 1 控制底部舵机 12号 -90 0 90
            var leftX = reading.LeftThumbstickX;
            var leftY = reading.LeftThumbstickY;
            //右边的推杆 值范围 -1 0 1 展开 -30 0 30 小于0 4号 大于0 8号
            var rightX = reading.RightThumbstickX;
            //右边的推杆上下 -1 0 1 头部上下 -15 0 15 2号
            var rightY = reading.RightThumbstickY;
            //左边扳机 值范围 0 1 旋转 0 180 6号
            var pbLeft = reading.LeftTrigger;
            //右边扳机 值范围 0 1 旋转 0 180 8号
            var pbRight = reading.RightTrigger;

            //发送表情
            if (reading.Buttons.HasFlag(GamepadButtons.A) && isHoldRightThumbstick == false)
            {
                isHoldRightThumbstick = true;
                Debug.Write($"send emojis---{DateTime.Now.ToString()}");

                var list = (await App.GetService<ILocalSettingsService>()
                  .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

                if (list != null && list.Count > 0)
                {
                    var r = new Random().Next(list.Count);

                    var action = list[r];

                    string? videoPath;

                    if (action.EmojisType == EmojisType.Default)
                    {
                        videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{action.NameId}.mp4";
                    }
                    else
                    {
                        videoPath = action.EmojisVideoPath;
                    }
                    _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundAsync(videoPath);

                    await App.GetService<IActionExpressionProvider>().PlayActionExpressionAsync(action);
                }
            }
            else if (reading.Buttons.HasFlag(GamepadButtons.A) && isHoldRightThumbstick == true)
            {
                //摁下不做处理
            }
            else
            {
                try
                {
                    j1 = (float)(rightY * 15.0);


                    if (rightX < 0)
                    {
                        j4 = -(float)(rightX * 30.0);
                    }
                    else
                    {
                        j2 = (float)(rightX * 30.0);
                    }



                    j3 = (float)(pbRight * 180.0);

                    j5 = (float)(pbLeft * 180.0);

                    j6 = (float)(leftX * 90.0);


                    await Task.Run(() =>
                    {
                        if (ElectronBotHelper.Instance.EbConnected)
                        {
                            var data = new byte[240 * 240 * 3];

                            var frame = new EmoticonActionFrame(data, true, j1, j2, j3, j4, j5, j6);

                            ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                        }
                    });
                }
                catch (Exception)
                {

                }
                isHoldRightThumbstick = false;
            }
        }
    }

    public void OnNavigatedFrom()
    {
        _dispatcherTimer.Stop();

        Gamepad.GamepadAdded -= Gamepad_GamepadAdded;
        Gamepad.GamepadRemoved -= Gamepad_GamepadRemoved;
    }
}
