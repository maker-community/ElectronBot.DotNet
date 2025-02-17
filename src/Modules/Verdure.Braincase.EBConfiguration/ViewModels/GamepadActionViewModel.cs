using Windows.ApplicationModel;
using Windows.Gaming.Input;

namespace Verdure.Braincase.EBConfiguration.ViewModels;

public partial class GamepadActionViewModel : ObservableRecipient
{
    private static Gamepad? _controller;

    private ComboxItemModel? _selectItem;

    readonly DispatcherTimer _dispatcherTimer = new();

    float j1 = 0, j2 = 0, j3 = 0, j4 = 0, j5 = 0, j6 = 0;

    private int btnCount = 0;
    public GamepadActionViewModel()
    {
    }

    private void Gamepad_GamepadRemoved(object? sender, Gamepad e)
    {
        _controller = null;
    }
    private void Gamepad_GamepadAdded(object? sender, Gamepad e)
    {
        _controller = Gamepad.Gamepads.FirstOrDefault();
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

                var list = (await Ioc.Default.GetRequiredService<ILocalSettingsService>()
                  .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

                if (list != null && list.Count > 0)
                {
                    var r = new Random().Next(list.Count);

                    var action = list[r];

                    string? videoPath;

                    if (action.Type == EmojisFileType.Default)
                    {
                        videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{action.NameId}.mp4";
                    }
                    else
                    {
                        videoPath = action.EmojisVideoPath;
                    }
                    //_ = ElectronBotHelper.Instance.MediaPlayerPlaySoundAsync(videoPath);

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
                        //if (ElectronBotHelper.Instance.EbConnected)
                        //{
                        //    var data = new byte[240 * 240 * 3];

                        //    var frame = new EmoticonActionFrame(data, true, j1, j2, j3, j4, j5, j6);

                        //    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                        //}
                    });
                }
                catch (Exception)
                {

                }
                isHoldRightThumbstick = false;
            }
        }
    }
}
