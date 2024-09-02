using System.IO.Ports;
using System.Text.Json;
using System.Text.RegularExpressions;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using ElectronBot.DotNet;
using ElectronBot.DotNet.LibUsb;
using ElectronBot.DotNet.WinUsb;
using Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Models;
using Services;
using Verdure.ElectronBot.Core.Models;
using Verdure.IoT.Net;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;

namespace ElectronBot.Braincase.Helpers;

public class ElectronBotHelper
{
    public IElectronLowLevel? ElectronBot
    {
        get; set;
    }

    public bool PlayEmojisLock
    {
        get; set;
    } = false;

    public bool VoiceLock
    {
        get; set;
    } = false;

    public bool UpLock
    {
        get;
        set;
    }
    public bool DownLock
    {
        get;
        set;
    }

    public bool BackLock
    {
        get;
        set;
    }

    public bool ForwardLock
    {
        get;
        set;
    }

    public bool LandLock
    {
        get;
        set;
    }
    public bool StopLock
    {
        get;
        set;
    }
    public bool LeftLock
    {
        get;
        set;
    }

    public bool RightLock
    {
        get;
        set;
    }

    public bool FingerHeartLock
    {
        get;
        set;
    }


    public bool ThirdFingerLock
    {
        get;
        set;
    }

    public bool IsLibUsbFW
    {
        get;
        set;
    } = true;

    private static ElectronBotHelper? _instance;
    public static ElectronBotHelper Instance => _instance ??= new ElectronBotHelper();

    private readonly SynchronizationContext? _context = SynchronizationContext.Current;

    private readonly Dictionary<string, string> _electronDic = new();

    private DeviceWatcher? deviceWatcher;

    public event EventHandler? ClockCanvasStop;
    public event EventHandler? ClockCanvasStart;

    public event EventHandler? PlayEmojisRandom;

    public event EventHandler<ModelActionFrame>? ModelActionFrame;

    public event EventHandler<string>? PlayEmojisByNameId;

    private MediaPlayer mediaPlayer = new();

    private bool isTTS = false;

    private bool _isOpenMediaEnded = false;

    private readonly int _playEmojisCount = 3;

    private List<float>? _angleList;
    public bool EbConnected
    {
        get; set;
    }

    public bool StartupTask
    {
        get; set;
    }

    public bool IsEntityFirstEnabled
    {
        get;
        set;
    } = false;


    public void InvokeClockCanvasStop()
    {
        ClockCanvasStop?.Invoke(this, new EventArgs());
    }

    public void InvokeClockCanvasStart()
    {
        ClockCanvasStart?.Invoke(this, new EventArgs());
    }


    public SerialPort SerialPort { get; set; } = new SerialPort();

    public List<Package> AppPackages = new();

    private PackageManager PackageManager { get; } = new PackageManager();

    private DeviceWatcher? _hidDeviceWatcher;

    private DeviceWatcher? _usbDeviceWatcher;

    public async Task InitAsync()
    {
        // Target all Serial Devices present on the system
        var deviceSelector = SerialDevice.GetDeviceSelector();

        var myDevices = await DeviceInformation.FindAllAsync(deviceSelector);

        deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);

        deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(OnDeviceAdded);
        deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(OnDeviceRemoved);

        deviceWatcher.Start();


        mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

        PlayEmojisRandom += ElectronBotHelper_PlayEmojisRandom;

        PlayEmojisByNameId += ElectronBotHelper_PlayEmojisByNameId;

        SystemEvents.InvokeOnEventsThread(() =>
        {
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
        });

        // Create the device watcher
        // Replace VID and PID with your device's Vendor ID and Product ID
        var aqs = UsbDevice.GetDeviceSelector(0x5241, 0x5241);
        _usbDeviceWatcher = DeviceInformation.CreateWatcher(aqs);

        //// Register the device added event
        _usbDeviceWatcher.Added += OnUsbDeviceAdded;

        _usbDeviceWatcher.Removed += OnUsbDeviceWatcher_Removed;

        //// Start the watcher
        _usbDeviceWatcher.Start();


        //Create the HID device watcher
        _hidDeviceWatcher = DeviceInformation.CreateWatcher(HidDevice.GetDeviceSelector(0xff14, 0x01));
        _hidDeviceWatcher.Added += OnHidDeviceAdded;
        _hidDeviceWatcher.Removed += OnHidDeviceRemoved;
        _hidDeviceWatcher.Start();

        // Create the serial device watcher
        //serialDeviceWatcher = DeviceInformation.CreateWatcher(SerialDevice.GetDeviceSelector());
        //serialDeviceWatcher.Added += DeviceWatcher_Added;
        //serialDeviceWatcher.Start();

    }

    private async void OnHidDeviceAdded(DeviceWatcher sender, DeviceInformation args)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ToastHelper.SendToast("Hw75ConnectedText".GetLocalized(), TimeSpan.FromSeconds(3));  
        });
        await Task.Run(async () =>
        {
            await Hw75GlobalTimerHelper.Instance.UpdateTimerIntervalAsync();
            await Hw75GlobalTimerHelper.Instance.UpdateHwViewAsync();
            Hw75GlobalTimerHelper.Instance.StartTimer();
        });        
    }

    private void OnHidDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ToastHelper.SendToast("Hw75DisconnectedText".GetLocalized(), TimeSpan.FromSeconds(3));   
        });
        Hw75GlobalTimerHelper.Instance.StopTimer();
    }


    private async void OnUsbDeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
    {
        try
        {
            //InvokeClockCanvasStop();

            var service = App.GetService<EmoticonActionFrameService>();

            service.ClearQueue();

            Thread.Sleep(1000);

            IntelligenceService.Current.CleanUp();

            await CameraService.Current.CleanupMediaCaptureAsync();

            await CameraFrameService.Current.CleanupMediaCaptureAsync();

            ElectronBot?.Disconnect();

            if (SerialPort.IsOpen)
            {
                SerialPort.Close();
            }
        }
        catch (Exception)
        {

        }

        EbConnected = false;

        await DisconnectDeviceAsync();
    }

    private async void OnUsbDeviceAdded(DeviceWatcher sender, DeviceInformation args)
    {
        try
        {
            if (ElectronBot is not null)
            {
                ElectronBot.Disconnect();

                ElectronBot = null;
            }

            IsLibUsbFW = false;

            Thread.Sleep(5000);

            ElectronBot = new WinUsbElectronLowLevel(App.GetService<ILogger<WinUsbElectronLowLevel>>());

            EbConnected = ElectronBot.Connect();

            //InvokeClockCanvasStart();

            await ConnectDeviceAsync();            
        }
        catch (Exception ex)
        {
            _context?.Post(async _ =>
            {
                ToastHelper.SendToast($"{ex.Message}", TimeSpan.FromSeconds(3));
                await Task.Delay(500);
            }, null);

            return;
        }
    }

    private async void ElectronBotHelper_PlayEmojisByNameId(object? sender, string e)
    {
        var localSettingsService = App.GetService<ILocalSettingsService>();
        var list = (await localSettingsService
             .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (list != null && list.Count > 0)
        {
            try
            {
                var emojis = list.FirstOrDefault(i => i.NameId == e);

                if (emojis == null)
                {
                    PlayEmojisLock = false;
                    return;
                }

                List<ElectronBotAction> actions = new();

                if (emojis.HasAction)
                {
                    if (!string.IsNullOrWhiteSpace(emojis.EmojisActionPath))
                    {
                        try
                        {
                            var path = string.Empty;

                            if (emojis.EmojisType == EmojisType.Default)
                            {
                                path = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.EmojisActionPath}";
                            }
                            else
                            {
                                path = emojis.EmojisActionPath;
                            }


                            var json = await File.ReadAllTextAsync(path);


                            var actionList = JsonSerializer.Deserialize<List<ElectronBotAction>>(json);

                            if (actionList != null && actionList.Count > 0)
                            {
                                actions = actionList;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                string? videoPath;

                if (emojis.EmojisType == EmojisType.Default)
                {
                    videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.NameId}.mp4";
                }
                else
                {
                    videoPath = emojis.EmojisVideoPath;
                }
                _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundAsync(videoPath);
                await App.GetService<IActionExpressionProvider>().PlayActionExpressionAsync(emojis, actions);
            }
            catch (Exception)
            {
                PlayEmojisLock = false;
            }
        }
    }

    private async void ElectronBotHelper_PlayEmojisRandom(object? sender, EventArgs e)
    {
        var localSettingsService = App.GetService<ILocalSettingsService>();
        var list = (await localSettingsService
             .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (list != null && list.Count > 0)
        {

            var r = new Random().Next(list.Count);

            try
            {
                var emojis = list[r];

                List<ElectronBotAction> actions = new();

                if (emojis.HasAction)
                {
                    if (!string.IsNullOrWhiteSpace(emojis.EmojisActionPath))
                    {
                        try
                        {
                            var path = string.Empty;

                            if (emojis.EmojisType == EmojisType.Default)
                            {
                                path = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.EmojisActionPath}";
                            }
                            else
                            {
                                path = emojis.EmojisActionPath;
                            }


                            var json = await File.ReadAllTextAsync(path);


                            var actionList = JsonSerializer.Deserialize<List<ElectronBotAction>>(json);

                            if (actionList != null && actionList.Count > 0)
                            {
                                actions = actionList;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                string? videoPath;

                if (emojis.EmojisType == EmojisType.Default)
                {
                    videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.NameId}.mp4";
                }
                else
                {
                    videoPath = emojis.EmojisVideoPath;
                }
                _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundAsync(videoPath);
                await App.GetService<IActionExpressionProvider>().PlayActionExpressionAsync(emojis, actions);
            }
            catch (Exception)
            {
                PlayEmojisLock = false;
            }
        }
    }

    public void PlayEmoticonActionFrame(EmoticonActionFrame frame)
    {
        if (EbConnected)
        {
            try
            {
                if (frame != null)
                {
                    if (ElectronBot is not null)
                    {
                        ElectronBot.SetImageSrc(frame.FrameBuffer);
                        ElectronBot.SetJointAngles(frame.J1, frame.J2, frame.J3, frame.J4, frame.J5, frame.J6, frame.Enable);
                        ElectronBot.Sync();
                        _angleList = ElectronBot.GetJointAngles();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

        }
    }
    private Task ConnectDeviceAsync()
    {
        _context?.Post(async _ =>
        {
            ToastHelper.SendToast("ElectronBotAddTip".GetLocalized(), TimeSpan.FromSeconds(3));

            await Task.Delay(500);
        }, null);

        return Task.CompletedTask;
    }

    private Task DisconnectDeviceAsync()
    {
        _context?.Post(async _ =>
        {
            ToastHelper.SendToast("ElectronBotRemoveTip".GetLocalized(), TimeSpan.FromSeconds(3));
            await Task.Delay(500);
        }, null);

        return Task.CompletedTask;
    }

    private async void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
    {
        try
        {
            if (IsLibUsbFW)
            {
                //InvokeClockCanvasStop();

                var service = App.GetService<EmoticonActionFrameService>();

                service.ClearQueue();

                Thread.Sleep(1000);

                IntelligenceService.Current.CleanUp();

                await CameraService.Current.CleanupMediaCaptureAsync();

                await CameraFrameService.Current.CleanupMediaCaptureAsync();

                ElectronBot?.Disconnect();

                if (SerialPort.IsOpen)
                {
                    SerialPort.Close();
                }

            }
            IsLibUsbFW = true;

        }
        catch (Exception)
        {

        }

        EbConnected = false;

        await DisconnectDeviceAsync();
    }

    private async void OnDeviceAdded(DeviceWatcher sender, DeviceInformation args)
    {
        if (args.Name.Contains("CP210") || args.Name.Contains("CH910"))
        {
            var comName = Regex.Replace(args.Name, @"(.*\()(.*)(\).*)", "$2"); //小括号()

            SerialPort.PortName = comName;

            SerialPort.BaudRate = 115200;

            if (StartupTask == true)
            {
                try
                {
                    if (!SerialPort.IsOpen)
                    {
                        SerialPort.Open();
                    }

                    if (SerialPort.IsOpen)
                    {
                        var byteData = new byte[]
                        {
                        0xea, 0x00, 0x00, 0x00, 0x00 ,0x0d, 0x02, 0x00 , 0x00, 0x0f, 0xea
                        };

                        SerialPort.Write(byteData, 0, byteData.Length);

                        Thread.Sleep(4000);

                        SerialPort.Close();
                    }

                }
                catch (Exception)
                {
                }
            }


            try
            {
                if (IsLibUsbFW)
                {
                    if (ElectronBot is not null)
                    {
                        ElectronBot.Disconnect();

                        ElectronBot = null;
                    }

                    Thread.Sleep(5000);

                    ElectronBot = new LibUsbElectronLowLevel(App.GetService<ILogger<LibUsbElectronLowLevel>>());

                    EbConnected = ElectronBot.Connect();

                    //InvokeClockCanvasStart();

                    await ConnectDeviceAsync();
                }
            }
            catch (Exception ex)
            {
                _context?.Post(async _ =>
                {
                    ToastHelper.SendToast($"串口打开异常：{ex.Message}", TimeSpan.FromSeconds(3));
                    await Task.Delay(500);
                }, null);

                return;
            }

            //if (StartupTask == true)
            //{
            //    Thread.Sleep(3000);

            //    for (var i = 0; i < _playEmojisCount; i++)
            //    {
            //        ToPlayEmojisRandom();

            //        Thread.Sleep(3000);
            //    }


            //}
        }
    }

    /// <summary>
    /// 播放表情声音
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public async Task MediaPlayerPlaySoundAsync(string path)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                var localSettingsService = App.GetService<ILocalSettingsService>();

                var audioModel = await localSettingsService
                    .ReadSettingAsync<ComboxItemModel>(Constants.DefaultAudioNameKey);

                var audioDevs = await EbHelper.FindAudioDeviceListAsync();

                if (audioModel != null)
                {
                    var audioSelect = audioDevs.FirstOrDefault(c => c.DataValue == audioModel.DataValue) ?? new ComboxItemModel();

                    var selectedDevice = (DeviceInformation)audioSelect.Tag!;

                    if (selectedDevice != null)
                    {
                        mediaPlayer.AudioDevice = selectedDevice;
                    }
                }
                mediaPlayer.SetUriSource(new Uri(path));
                mediaPlayer.Play();
            }
            catch (Exception)
            {
                PlayEmojisLock = false;
                VoiceLock = false;
            }
        }
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="content"></param>
    /// <param name="isOpenMediaEnded"></param>
    /// <returns></returns>
    public async Task MediaPlayerPlaySoundByTtsAsync(string content, bool isOpenMediaEnded = true)
    {
        _isOpenMediaEnded = isOpenMediaEnded;
        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                var localSettingsService = App.GetService<ILocalSettingsService>();

                var audioModel = await localSettingsService
                    .ReadSettingAsync<ComboxItemModel>(Constants.DefaultAudioNameKey);

                var audioDevs = await EbHelper.FindAudioDeviceListAsync();

                if (audioModel != null)
                {
                    var audioSelect = audioDevs.FirstOrDefault(c => c.DataValue == audioModel.DataValue) ?? new ComboxItemModel();

                    var selectedDevice = (DeviceInformation)audioSelect.Tag!;

                    if (selectedDevice != null)
                    {
                        mediaPlayer.AudioDevice = selectedDevice;
                    }
                }

                var speechAndTTSService = App.GetService<ISpeechAndTTSService>();

                var stream = await speechAndTTSService.TextToSpeechAsync(content);

                mediaPlayer.SetStreamSource(stream);
                mediaPlayer.Play();
                isTTS = true;
            }
            catch (Exception)
            {
            }
        }
    }

    public async Task CloseChatAsync()
    {
        try
        {
            var speechAndTTSService = App.GetService<ISpeechAndTTSService>();
            await speechAndTTSService.ReleaseRecognizerAsync();
            //await speechAndTTSService.CancelAsync();
        }
        catch (Exception)
        {

        }
    }


    public void ToPlayEmojisRandom()
    {
        PlayEmojisRandom?.Invoke(this, new EventArgs());
    }

    public void ToPlayEmojisByNameId(string nameId)
    {
        PlayEmojisByNameId?.Invoke(this, nameId);
    }

    public void ModelActionInvoke(ModelActionFrame frame)
    {
        frame.Actions = new OnlyAction(_angleList);
        ModelActionFrame?.Invoke(this, frame);
    }

    public void LoadAppList()
    {
        AppPackages.Clear();
        AppPackages = PackageManager.FindPackagesForUser(string.Empty)
           .Where(p => p.IsFramework == false && !string.IsNullOrEmpty(p.DisplayName)).ToList();
    }


    public async Task SessionHaSwitchAsync(SessionSwitchReason switchReason)
    {
        var localSettingsService = App.GetService<ILocalSettingsService>();

        var haSwitchModel = await localSettingsService.ReadSettingAsync<ComboxItemModel>(Constants.DefaultHaSwitchNameKey);

        var haSetting = await localSettingsService.ReadSettingAsync<HaSetting>(Constants.HaSettingKey);

        if (haSwitchModel != null && haSetting != null)
        {
            try
            {
                var client = new HomeAssistantClient(haSetting.BaseUrl, haSetting.HaToken);

                if (switchReason == SessionSwitchReason.SessionUnlock)
                {
                    await client.PostServiceAync(haSwitchModel?.DataKey?.Split('.')[0] ?? "", "turn_on", haSwitchModel?.DataKey ?? "");
                }
                else if (switchReason == SessionSwitchReason.SessionLock)
                {
                    await client.PostServiceAync(haSwitchModel?.DataKey?.Split('.')[0] ?? "", "turn_off", haSwitchModel?.DataKey ?? "");
                }

            }
            catch { }
        }
    }

    private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        try
        {
            var speechAndTTSService = App.GetService<ISpeechAndTTSService>();

            if (isTTS && _isOpenMediaEnded)
            {

                await speechAndTTSService.InitializeRecognizerAsync(SpeechRecognizer.SystemSpeechLanguage);


                await speechAndTTSService.StartAsync();
                isTTS = false;
            }
            else
            {
                await speechAndTTSService.ReleaseRecognizerAsync();
                //await speechAndTTSService.CancelAsync();
            }
        }
        catch (Exception)
        {

        }

        PlayEmojisLock = false;
        VoiceLock = false;
    }

    private async void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        var localSettingsService = App.GetService<ILocalSettingsService>();

        var botSetting = await localSettingsService.ReadSettingAsync<BotSetting>(Constants.BotSettingKey);

        var isHelloEnabled = botSetting == null || botSetting.IsHelloEnabled;

        if (isHelloEnabled && EbConnected)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionUnlock:
                    ToPlayEmojisByNameId("hello");
                    break;
                case SessionSwitchReason.SessionLock:
                    ToPlayEmojisByNameId("goodbye");
                    break;
            }
        }

        var haSetting = await localSettingsService.ReadSettingAsync<HaSetting>(Constants.HaSettingKey);

        if (haSetting != null && haSetting.IsSessionSwitchEnabled)
        {
            await SessionHaSwitchAsync(e.Reason);
        }
    }
}
