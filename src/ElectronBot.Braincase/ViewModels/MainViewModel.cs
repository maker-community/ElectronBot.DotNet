using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contracts.Services;
using Controls.CompactOverlay;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Mediapipe.Net.Solutions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Services;
using Verdure.ElectronBot.Core.Models;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace ElectronBot.Braincase.ViewModels;

public partial class MainViewModel : ObservableRecipient, INavigationAware
{
    private readonly DispatcherTimer _dispatcherTimer;

    private readonly IClockViewProviderFactory _viewProviderFactory;

    private readonly IActionExpressionProvider _actionExpressionProvider;

    private readonly IActionExpressionProviderFactory _expressionProviderFactory;

    private readonly ISpeechAndTTSService _speechAndTTSService;

    private readonly ILocalSettingsService _localSettingsService;

    private static HandsCpuSolution calculator = new();

    private bool _isBeginning = false;

    private readonly string modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\MLModel1.zip";

    private bool _isInitialized = false;

    private int modeNo = 0;

    private int count = 0;

    private int actionCount = 0;

    private readonly MediaPlayer _mediaPlayer;

    SoftwareBitmap? frameServerDest = null;

    CanvasImageSource? canvasImageSource = null;

    private readonly ElementTheme _elementTheme;

    private GestureAppService _gestureAppService = new();

    private readonly IntPtr _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
    public MainViewModel(
        ILocalSettingsService localSettingsService,
        IClockViewProviderFactory viewProviderFactory,
        ComboxDataService comboxDataService,
        DispatcherTimer dispatcherTimer,
        ObjectPickerService objectPickerService,
        MediaPlayer mediaPlayer,
        IActionExpressionProviderFactory actionExpressionProviderFactory,
        ISpeechAndTTSService speechAndTTSService,
        IThemeSelectorService elementTheme)
    {
        _localSettingsService = localSettingsService;

        _dispatcherTimer = dispatcherTimer;

        _speechAndTTSService = speechAndTTSService;

        _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;

        _viewProviderFactory = viewProviderFactory;

        _expressionProviderFactory = actionExpressionProviderFactory;

        ClockComboxModels = comboxDataService.GetClockViewComboxList();

        _mediaPlayer = mediaPlayer;

        _mediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;

        _mediaPlayer.IsVideoFrameServerEnabled = true;

        var defaultProvider = _expressionProviderFactory.CreateActionExpressionProvider("Default");

        _actionExpressionProvider = defaultProvider;

        ElectronBotHelper.Instance.SerialPort.DataReceived += SerialPort_DataReceived;

        ElectronBotHelper.Instance.ClockCanvasStop += Instance_ClockCanvasStop;
        ElectronBotHelper.Instance.ClockCanvasStart += Instance_ClockCanvasStart;
        _elementTheme = elementTheme.Theme;
    }

    private void Instance_ClockCanvasStart(object? sender, EventArgs e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            _dispatcherTimer.Start();
        });
    }

    private void Instance_ClockCanvasStop(object? sender, EventArgs e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            _dispatcherTimer.Stop();
        });
    }


    [RelayCommand]
    private async void OpenGesture(bool isOn)
    {
        try
        {
            //按钮开启
            if (!isOn)
            {
                await InitAsync();
            }
            else
            {
                var service = App.GetService<EmoticonActionFrameService>();
                service.ClearQueue();
                await CleanUpAsync();
            }
        }
        catch (Exception)
        {
        }
    }



    private async Task InitAsync()
    {
        if (_isInitialized)
        {

            CameraFrameService.Current.SoftwareBitmapFrameCaptured -= Current_SoftwareBitmapFrameCaptured;

            CameraFrameService.Current.SoftwareBitmapFrameHandPredictResult -= Current_SoftwareBitmapFrameHandPredictResult;

            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        else
        {
            await InitializeScreenAsync();
        }

        var gestureAppConfigs = (await _localSettingsService.ReadSettingAsync<List<GestureAppConfig>>
                  (Constants.CustomGestureAppConfigKey)) ?? new List<GestureAppConfig>();
        _gestureAppService.Init(gestureAppConfigs);
    }

    private async Task InitializeScreenAsync()
    {
        await CameraFrameService.Current.PickNextMediaSourceWorkerAsync(FaceImage);

        CameraFrameService.Current.SoftwareBitmapFrameCaptured += Current_SoftwareBitmapFrameCaptured;

        CameraFrameService.Current.SoftwareBitmapFrameHandPredictResult += Current_SoftwareBitmapFrameHandPredictResult;

        _isInitialized = true;
    }

    private void Current_SoftwareBitmapFrameHandPredictResult(object? sender, string e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            ResultLabel = e;

            if (e == Constants.FingerHeart && _isBeginning == false)
            {
                _isBeginning = true;

                var config = (await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>
                (Constants.CustomClockTitleConfigKey)) ?? new CustomClockTitleConfig();

                var textList = config.AnswerText.Split(",").ToList();

                var r = new Random().Next(textList.Count);

                var text = textList[r];

                ToastHelper.SendToast(text, TimeSpan.FromSeconds(2));

                await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(text, true);
            }
            else if (e == Constants.FingerHeart && _isBeginning == true)
            {
                //当前处于启动状态
                //不做处理
            }
            else if (e == Constants.Land && _isBeginning == true)
            {
                _isBeginning = false;
            }

            //if (!_gestureAppService.GetInExecuting())
            //{
            //    await _gestureAppService.Execute(ResultLabel);
            //}
        });
    }

    private void Current_SoftwareBitmapFrameCaptured(object? sender, SoftwareBitmapEventArgs e)
    {
        if (e.SoftwareBitmap is not null)
        {

            if (e.SoftwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                  e.SoftwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                e.SoftwareBitmap = SoftwareBitmap.Convert(
                    e.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }
            var service = App.GetService<GestureClassificationService>();

            _ = service.HandPredictResultUnUseQueueAsync(calculator, modelPath, e.SoftwareBitmap);
        }
    }

    private async Task CleanUpAsync()
    {
        try
        {
            _isInitialized = false;

            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        catch (Exception)
        {

        }
    }

    [RelayCommand]
    private void RebootElectron()
    {
        try
        {
            if (!ElectronBotHelper.Instance.SerialPort.IsOpen)
            {
                ElectronBotHelper.Instance.SerialPort.Open();
            }

            var byteData = new byte[]
            {
                0xea, 0x00, 0x00, 0x00, 0x00 ,0x0d, 0x02, 0x00 , 0x00, 0x0f, 0xea
            };

            ElectronBotHelper.Instance.SerialPort.Write(byteData, 0, byteData.Length);

            Thread.Sleep(1000);

            if (ElectronBotHelper.Instance.SerialPort.IsOpen)
            {
                ElectronBotHelper.Instance.SerialPort.Close();
            }

        }
        catch (Exception)
        {
        }
    }

    [RelayCommand]
    private async Task StartChat()
    {
        var config = (await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>
            (Constants.CustomClockTitleConfigKey)) ?? new CustomClockTitleConfig();

        var textList = config.AnswerText.Split(",").ToList();

        var r = new Random().Next(textList.Count);

        var text = textList[r];

        ToastHelper.SendToast(text, TimeSpan.FromSeconds(4));

        var localSettingsService = App.GetService<ILocalSettingsService>();

        var list = (await _localSettingsService
            .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (!list.Any(a => a.EmojisType == EmojisType.Default))
        {
            var emoticonActions = Constants.EMOJI_ACTION_LIST;
            await _localSettingsService.SaveSettingAsync(Constants.EmojisActionListKey, emoticonActions.ToList());
            list = emoticonActions.ToList();
        }

        if (list != null && list.Count > 0)
        {
            try
            {
                var emojis = list.First(l => l.NameId == "normal");

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
                _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(text, true);
                await App.GetService<IActionExpressionProvider>().PlayActionExpressionAsync(emojis, actions);
            }
            catch (Exception)
            {
            }
        }
    }


    [RelayCommand]
    private async Task SendChat()
    {
        var config = (await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>
            (Constants.CustomClockTitleConfigKey)) ?? new CustomClockTitleConfig();

        var textList = config.AnswerText.Split(",").ToList();

        var r = new Random().Next(textList.Count);


        var text = textList[r];

        ToastHelper.SendToast("please wait for a moment", TimeSpan.FromSeconds(4));

        var localSettingsService = App.GetService<ILocalSettingsService>();

        var list = (await _localSettingsService
            .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (!list.Any(a => a.EmojisType == EmojisType.Default))
        {
            var emoticonActions = Constants.EMOJI_ACTION_LIST;
            await _localSettingsService.SaveSettingAsync(Constants.EmojisActionListKey, emoticonActions.ToList());
            list = emoticonActions.ToList();
        }

        if (list != null && list.Count > 0)
        {
            try
            {
                var emojis = list.First(l => l.NameId == "normal");

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
                _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync("please wait for a moment", false);
                await App.GetService<IActionExpressionProvider>().PlayActionExpressionAsync(emojis, actions);

                try
                {
                    //var chatGPTClient = App.GetService<IChatGPTService>();

                    //var resultText = await chatGPTClient.AskQuestionResultAsync(args.Result.Text);

                    //await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTTSAsync(resultText);

                    var chatBotClientFactory = App.GetService<IChatbotClientFactory>();

                    var chatBotClientName = (await App.GetService<ILocalSettingsService>()
                         .ReadSettingAsync<ComboxItemModel>(Constants.DefaultChatBotNameKey))?.DataKey;

                    if (string.IsNullOrEmpty(chatBotClientName))
                    {
                        throw new Exception("no app key in the config");
                    }

                    var chatBotClient = chatBotClientFactory.CreateChatbotClient(chatBotClientName);

                    var resultText = await chatBotClient.AskQuestionResultAsync(SendText);

                    await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(resultText, false);
                }
                catch (Exception ex)
                {
                    App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(3));
                    });

                }
            }
            catch (Exception)
            {
            }
        }
    }


    [RelayCommand]
    private async Task EndChat()
    {

        ToastHelper.SendToast("end chat", TimeSpan.FromSeconds(4));

        await ElectronBotHelper.Instance.CloseChatAsync();
    }


    [RelayCommand]
    private void ElectronEmulation()
    {
        try
        {
            WindowEx compactOverlay = new CompactOverlayWindow();

            compactOverlay.Content = App.GetService<ModelLoadCompactOverlayPage>();

            var appWindow = compactOverlay.AppWindow;

            appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

            appWindow.Show();

            App.MainWindow.Hide();
        }
        catch (Exception)
        {
        }
    }

    [RelayCommand]
    private async void TestVoice()
    {
        var textList = new List<string>()
        {
            "哥哥你好啊",
            "哥哥在干嘛",
            "哥哥想我没",
            "哥哥最好啦",
            "最喜欢哥哥啦",
            "人家好想哥哥",
            "哥哥喜欢妹妹不"
        };

        var r = new Random().Next(textList.Count);

        var text = textList[r];

        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ToastHelper.SendToast(text, TimeSpan.FromSeconds(2));
        });

        await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(text);


        //var stream = await _speechAndTTSService.TextToSpeechAsync(text);

        //_mediaPlayer.SetStreamSource(stream);

        //var selectedDevice = (DeviceInformation)AudioSelect?.Tag;

        //if (selectedDevice != null)
        //{
        //    _mediaPlayer.AudioDevice = selectedDevice;
        //}

        //_mediaPlayer.Play();

        //var ret = RuntimeHelper.IsAdminRun();

        //App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        //{
        //    ToastHelper.SendToast($"是否在管理权权限运行：{ret}", TimeSpan.FromSeconds(2));
        //});
    }


    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        var indata = sp.ReadExisting();
        Debug.WriteLine("Data Received:");
        Debug.Write(indata);

        if (indata.Contains("Clockwise"))
        {
            var r = new Random().Next(Constants.POTENTIAL_EMOJI_LIST.Count);

            var mediaPlayer = App.GetService<MediaPlayer>();

            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/Emoji/{Constants.POTENTIAL_EMOJI_LIST[r]}.mp4"));

            //var selectedDevice = (DeviceInformation)AudioSelect?.Tag;

            //if (selectedDevice != null)
            //{
            //    mediaPlayer.AudioDevice = selectedDevice;
            //}

            mediaPlayer.Play();
        }
    }

    private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        await _speechAndTTSService.InitializeRecognizerAsync(SpeechRecognizer.SystemSpeechLanguage);

        await _speechAndTTSService.StartAsync();
    }

    [RelayCommand]
    private void TestPlayEmoji()
    {
        try
        {
            _dispatcherTimer.Stop();

            var r = new Random().Next(Constants.POTENTIAL_EMOJI_LIST.Count);

            _mediaPlayer.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/Emoji/{Constants.POTENTIAL_EMOJI_LIST[r]}.mp4"));

            //var selectedDevice = (DeviceInformation)AudioSelect?.Tag;

            //if (selectedDevice != null)
            //{
            //    _mediaPlayer.AudioDevice = selectedDevice;
            //}
            _mediaPlayer.Play();

            _actionExpressionProvider.PlayActionExpressionAsync($"{Constants.POTENTIAL_EMOJI_LIST[r]}", actions.ToList());
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 媒体播放帧处理事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void MediaPlayer_VideoFrameAvailable(MediaPlayer sender, object args)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                var canvasDevice = App.GetService<CanvasDevice>();

                if (frameServerDest == null)
                {
                    // FrameServerImage in this example is a XAML image control
                    frameServerDest =
                        new SoftwareBitmap(BitmapPixelFormat.Rgba8, 240, 240, BitmapAlphaMode.Ignore);

                }
                if (canvasImageSource == null)
                {
                    canvasImageSource = new CanvasImageSource(canvasDevice, 240, 240, 96);//96); 

                    EmojiImageSource = canvasImageSource;

                }

                using var inputBitmap = CanvasBitmap.CreateFromSoftwareBitmap(canvasDevice, frameServerDest);

                using var ds = canvasImageSource.CreateDrawingSession(Microsoft.UI.Colors.Black);

                _mediaPlayer.CopyFrameToVideoSurface(inputBitmap);

                ds.DrawImage(inputBitmap);
            }
            catch (Exception ex)
            {

            }
        });
    }

    /// <summary>
    /// 表盘切换方法
    /// </summary>
    [RelayCommand]
    private void ClockChanged()
    {
        var clockName = clockComBoxSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(clockName))
        {
            var service = App.GetService<EmoticonActionFrameService>();

            service.ClearQueue();

            var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockName);

            if (clockName == "GooeyFooter" || clockName == "CustomView")
            {
                _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            }
            else
            {
                _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            }

            Element = viewProvider.CreateClockView(clockName);
        }
    }


    /// <summary>
    /// 定时器处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        if (modeNo == 2)
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                await EbHelper.ShowClockCanvasToDeviceAsync(Element);
            }
        }
        else if (modeNo == 3)
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                var data = new byte[240 * 240 * 3];

                var frame = new EmoticonActionFrame(data);

                ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);

                var jointAngles = ElectronBotHelper.Instance?.ElectronBot?.GetJointAngles();

                if (jointAngles != null)
                {
                    var actionData = new ElectronBotAction()
                    {
                        Id = Guid.NewGuid().ToString(),
                        J1 = (int)jointAngles[0],
                        J2 = (int)jointAngles[1],
                        J3 = (int)jointAngles[2],
                        J4 = (int)jointAngles[3],
                        J5 = (int)jointAngles[4],
                        J6 = (int)jointAngles[5]
                    };

                    Actions.Add(actionData);
                }
            }
        }
        else if (modeNo == 4)
        {
            var (x, y) = EbHelper.GetScreenCursorPos();

            var screenSize = EbHelper.GetScreenSize(_hwnd);
            var mdButton = EbHelper.IsVkMButtonEnabled();
            var str = $"height:{screenSize.height}width:{screenSize.width}x:{x}y:{y} mdButton:{mdButton}";
            Debug.WriteLine(str);

            if (mdButton)
            {
                var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                if (mdButton && !playEmojisLock)
                {
                    //随机播放表情
                    ElectronBotHelper.Instance.ToPlayEmojisRandom();
                }

                ElectronBotHelper.Instance.PlayEmojisLock = true;
            }
            else
            {
                if (!ElectronBotHelper.Instance.PlayEmojisLock)
                {
                    if (screenSize.height > screenSize.width)
                    {
                        await EbHelper.ShowClockCanvasAndPosToDeviceAsync(Element, screenSize.width, screenSize.height, x, y);
                    }
                    else
                    {
                        await EbHelper.ShowClockCanvasAndPosToDeviceAsync(Element, screenSize.height, screenSize.width, x, y);
                    }
                }
            }
        }
    }

    public void Head_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (ElectronBotHelper.Instance.EbConnected && modeNo == 1)
        {
            Task.Run(() =>
            {
                if (ElectronBotHelper.Instance.EbConnected)
                {
                    var data = new byte[240 * 240 * 3];

                    var frame = new EmoticonActionFrame(data, true, j1, j2, j3, j4, j5, j6);

                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                }
            });
        }
    }

    public async void RadioButtons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var radioButtons = (RadioButtons)sender;

        var service = App.GetService<EmoticonActionFrameService>();

        service.ClearQueue();

        var list = radioButtons.Items;

        List<RadioButton> rbList = new();

        if (list != null && list.Count > 0)
        {
            foreach (var item in list)
            {
                rbList.Add((RadioButton)item);
            }
        }

        var index = rbList.IndexOf(rbList.Where(l => l.IsChecked == true).FirstOrDefault());

        if (index > -1)
        {
            modeNo = index;
        }

        if (index == 2)
        {
            if (!ElectronBotHelper.Instance.EbConnected)
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                await ResetActionAsync();

                var clockName = clockComBoxSelect?.DataKey;

                if (clockName != "GooeyFooter" && clockName != "CustomView")
                {
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                }

                _dispatcherTimer.Start();
            }
        }
        else if (index == 3)
        {
            if (!ElectronBotHelper.Instance.EbConnected)
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                await ResetActionAsync();
                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Interval);
                _dispatcherTimer.Start();
            }
        }
        else if (index == 4)
        {
            if (!ElectronBotHelper.Instance.EbConnected)
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                await ResetActionAsync();

                var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\Pic\\eyes-closed.png");

                var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                var dataMeta = mat2.Data;

                var data = new byte[240 * 240 * 3];

                Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                EbHelper.FaceData = data;

                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
                _dispatcherTimer.Start();
            }
        }
        else
        {
            _dispatcherTimer.Stop();
        }
    }


    /// <summary>
    /// 导入动作列表
    /// </summary>
    [RelayCommand]
    public async Task ImportAsync()
    {
        var list = await EbHelper.ImportActionListAsync(_hwnd);

        Actions = new ObservableCollection<ElectronBotAction>(list);
    }

    [RelayCommand]
    public async Task PlayAsync()
    {
        if (modeNo == 1)
        {
            if (actions.Count > 0)
            {
                await ResetActionAsync();

                await EbHelper.PlayActionListAsync(Actions.ToList(), Interval);

            }
            else
            {
                ToastHelper.SendToast("PlayEmptyToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }

        }
        else
        {
            ToastHelper.SendToast("PlayErrorToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        }
    }

    [RelayCommand]
    public void Stop()
    {
        _dispatcherTimer.Stop();
    }

    [RelayCommand]
    public void Clear()
    {
        actions.Clear();

        count = 0;

        actionCount = 0;

        ToastHelper.SendToast("PlayClearToastText".GetLocalized(), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public void Reconnect()
    {
        try
        {
            _dispatcherTimer.Stop();
            //ElectronBotHelper.Instance?.ElectronBot?.Disconnect();
            ElectronBotHelper.Instance?.ElectronBot?.ResetDevice();
        }
        catch (Exception)
        {

        }


        ToastHelper.SendToast("ReconnectText".GetLocalized(), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public async Task ResetAsync()
    {
        if (modeNo == 1)
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                await ResetActionAsync();

                ToastHelper.SendToast("PlayResetToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }

        }
        else
        {
            ToastHelper.SendToast("PlayErrorToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        }
    }

    private ICommand _pauseCommand;

    public ICommand PauseCommand
    {
        get
        {
            _pauseCommand ??= new RelayCommand(
                    () =>
                    {

                    });

            return _pauseCommand;
        }
    }

    [RelayCommand]
    public async Task ExportAsync()
    {
        StorageFolder destinationFolder = null;

        try
        {
            destinationFolder = await KnownFolders.PicturesLibrary
            .CreateFolderAsync("ElectronBot", CreationCollisionOption.OpenIfExists);
        }
        catch (Exception ex)
        {
            return;
        }

        if (Actions != null && Actions.Count > 0)
        {
            var fileName = $"electronbot-action-{DateTime.Now:yyyy-MM-dd-hh-mm-ss}.json";

            var destinationFile = await destinationFolder
                .CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            var content = JsonSerializer
                .Serialize(Actions, options: new JsonSerializerOptions { WriteIndented = true });

            await FileIO.WriteTextAsync(destinationFile, content);

            var text = "ExportToastText".GetLocalized();

            ToastHelper.SendToast($"{text}-{destinationFile.Path}", TimeSpan.FromSeconds(5));
        }
        else
        {
            ToastHelper.SendToast("PlayEmptyToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        }
    }

    [RelayCommand]
    public void Add()
    {
        if (SelectIndex < 0)
        {
            SelectIndex = 0;
        }
        else if (SelectIndex > Actions.Count)
        {
            SelectIndex = Actions.Count;
        }

        if (Actions.Count > 0)
        {
            Actions.Insert(SelectIndex + 1, new ElectronBotAction
            {
                J1 = J1,
                J2 = J2,
                J3 = J3,
                J4 = J4,
                J5 = J5,
                J6 = J6
            });
        }
        else
        {
            Actions.Add(new ElectronBotAction
            {
                J1 = J1,
                J2 = J2,
                J3 = J3,
                J4 = J4,
                J5 = J5,
                J6 = J6
            });
        }
    }

    [RelayCommand]
    public void RemoveAction()
    {
        if (SelectIndex < 0)
        {
            SelectIndex = 0;
        }
        else if (SelectIndex > Actions.Count)
        {
            SelectIndex = Actions.Count;
        }

        Actions.RemoveAt(SelectIndex);
    }

    [RelayCommand]
    public async Task AddPictureAsync()
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,

            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads
        };

        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");

        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file != null)
        {
            var config = new ImageCropperConfig
            {
                ImageFile = file,
                AspectRatio = 1
            };

            var croppedImage = await ImageHelper.CropImage(config);

            if (croppedImage != null)
            {
                SelectdAction.BitmapImageData = croppedImage;

                var act = Actions.Where(i => i.Id == selectdAction.Id).FirstOrDefault();

                if (act != null)
                {
                    var bytes = croppedImage.PixelBuffer.ToArray();

                    var imageData = await EbHelper.ToBase64Async(
                        bytes, (uint)croppedImage.PixelWidth, (uint)croppedImage.PixelWidth);

                    act.ImageData = $"data:image/png;base64,{imageData}";

                    act.BitmapImageData = croppedImage;
                }
            }
        }
    }

    private async Task ResetActionAsync()
    {
        J1 = 0;
        J2 = 0;
        J3 = 0;
        J4 = 0;
        J5 = 0;
        J6 = 0;

        await Task.Run(() =>
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                if (ElectronBotHelper.Instance.EbConnected)
                {
                    var data = new byte[240 * 240 * 3];

                    var frame = new EmoticonActionFrame(data, true);

                    var service = App.GetService<EmoticonActionFrameService>();

                    service.ClearQueue();

                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                }
            }
        });
    }

    public void OnNavigatedTo(object parameter)
    {
        var viewProvider = _viewProviderFactory.CreateClockViewProvider("DefautView");

        Element = viewProvider.CreateClockView("DefautView");

        if (modeNo == 3)
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                _dispatcherTimer.Start();
            }
        }
    }

    public async void OnNavigatedFrom()
    {
        try
        {
            _isInitialized = false;
            CameraFrameService.Current.SoftwareBitmapFrameCaptured -= Current_SoftwareBitmapFrameCaptured;

            CameraFrameService.Current.SoftwareBitmapFrameHandPredictResult -= Current_SoftwareBitmapFrameHandPredictResult;

            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        catch (Exception)
        {

        }
        _dispatcherTimer.Stop();
    }
}
