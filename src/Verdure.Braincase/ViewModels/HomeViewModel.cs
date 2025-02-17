using System.Diagnostics;
using System.IO.Ports;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Contracts.Services;
using Controls.CompactOverlay;
using Mediapipe.Net.Solutions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Services;
using Verdure.Braincase.Contracts.Services;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.Braincase.EbScreen.Views;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.Models;
using Verdure.Braincase.Services;
using Verdure.Braincase.WinUI.Common.Helpers;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;

namespace Verdure.Braincase.ViewModels;

public partial class HomeViewModel : ObservableRecipient, INavigationAware,IRecipient<ChangeClockView>
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
    public HomeViewModel(
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

        WeakReferenceMessenger.Default.Register<ChangeClockView>(this);
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
                var service = Ioc.Default.GetRequiredService<EmoticonActionFrameService>();
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
            var service = Ioc.Default.GetRequiredService<GestureClassificationService>();

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

        var localSettingsService = Ioc.Default.GetRequiredService<ILocalSettingsService>();

        var list = (await _localSettingsService
            .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (!list.Any(a => a.Type == EmojisFileType.Default))
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

                            if (emojis.Type == EmojisFileType.Default)
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

                if (emojis.Type == EmojisFileType.Default)
                {
                    videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.NameId}.mp4";
                }
                else
                {
                    videoPath = emojis.EmojisVideoPath;
                }
                _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(text, true);
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

        var localSettingsService = Ioc.Default.GetRequiredService<ILocalSettingsService>();

        var list = (await _localSettingsService
            .ReadSettingAsync<List<EmoticonAction>>(Constants.EmojisActionListKey)) ?? new List<EmoticonAction>();

        if (!list.Any(a => a.Type == EmojisFileType.Default))
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

                            if (emojis.Type == EmojisFileType.Default)
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

                if (emojis.Type == EmojisFileType.Default)
                {
                    videoPath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\{emojis.NameId}.mp4";
                }
                else
                {
                    videoPath = emojis.EmojisVideoPath;
                }
                _ = ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync("please wait for a moment", false);

                try
                {
                    //var chatGPTClient = Ioc.Default.GetRequiredService<IChatGPTService>();

                    //var resultText = await chatGPTClient.AskQuestionResultAsync(args.Result.Text);

                    //await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTTSAsync(resultText);

                    var chatBotClientFactory = Ioc.Default.GetRequiredService<IChatbotClientFactory>();

                    var chatBotClientName = (await Ioc.Default.GetRequiredService<ILocalSettingsService>()
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

            compactOverlay.Content = Ioc.Default.GetRequiredService<MiniModePage>();

            var appWindow = compactOverlay.AppWindow;

            appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

            appWindow.Show();

            App.MainWindow.Hide();
        }
        catch (Exception)
        {
        }
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

            var mediaPlayer = Ioc.Default.GetRequiredService<MediaPlayer>();

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
                var canvasDevice = Ioc.Default.GetRequiredService<CanvasDevice>();

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

        var service = Ioc.Default.GetRequiredService<EmoticonActionFrameService>();

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

                var clockName = ClockComBoxSelect?.DataKey;

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

                //var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\Pic\\eyes-closed.png");

                //var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                //var dataMeta = mat2.Data;

                //var data = new byte[240 * 240 * 3];

                //Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                //EbHelper.FaceData = data;

                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
                _dispatcherTimer.Start();
            }
        }
        else
        {
            _dispatcherTimer.Stop();
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

                    var service = Ioc.Default.GetRequiredService<EmoticonActionFrameService>();

                    service.ClearQueue();

                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                }
            }
        });
    }

    public async void OnNavigatedTo(object parameter)
    {
        var saveClockView = await _localSettingsService
            .ReadSettingAsync<string>(Constants.CurrentClockViewKey);

        var clockView = string.IsNullOrEmpty(saveClockView) ? "DefautView" : saveClockView;
        var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockView);

        Element = viewProvider.CreateClockView(clockView);

        if (modeNo == 3)
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                _dispatcherTimer.Start();
            }
        }
    }

    public void OnNavigatedFrom()
    {
        _dispatcherTimer.Stop();
    }

    public  void Receive(ChangeClockView view)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async() =>
        {
            var clockName = view.ClockViewName;
            if (!string.IsNullOrWhiteSpace(clockName))
            {
                var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

                service.ClearQueue();

                var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockName);

                if (clockName == "GooeyFooter" || clockName == "CustomView" || clockName == "GrooveView")
                {
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
                }
                else
                {
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                }

                Element = viewProvider.CreateClockView(clockName);

                await _localSettingsService
                    .SaveSettingAsync(Constants.CurrentClockViewKey, clockName);
            }
        });
    }
}
