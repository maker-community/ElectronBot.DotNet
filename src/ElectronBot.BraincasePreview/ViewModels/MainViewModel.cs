using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.BraincasePreview.Contracts.Services;
using ElectronBot.BraincasePreview.Contracts.ViewModels;
using Verdure.ElectronBot.Core.Models;
using ElectronBot.BraincasePreview.Helpers;
using ElectronBot.BraincasePreview.Models;
using ElectronBot.BraincasePreview.Picker;
using ElectronBot.BraincasePreview.Services;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace ElectronBot.BraincasePreview.ViewModels;

public class MainViewModel : ObservableRecipient, INavigationAware
{
    private readonly DispatcherTimer _dispatcherTimer;

    private readonly IClockViewProviderFactory _viewProviderFactory;

    private readonly IActionExpressionProvider _actionExpressionProvider;

    private readonly IActionExpressionProviderFactory _expressionProviderFactory;

    private readonly ISpeechAndTTSService _speechAndTTSService;

    private ObservableCollection<ElectronBotAction> actions = new();

    private ElectronBotAction selectdAction = new();

    private readonly ILocalSettingsService _localSettingsService;

    private readonly ObjectPickerService _objectPickerService;

    private int actionCount = 0;

    private int count = 0;

    private int modeNo = 0;

    private int _selectIndex = 0;

    private int _interval = 500;

    private UIElement _element;

    private ImageSource _emojiImageSource;

    private ObservableCollection<ComboxItemModel> _clockComboxModels;

    private ObservableCollection<ComboxItemModel> _cameras;

    private ObservableCollection<ComboxItemModel> _audioDevs;

    private ComboxItemModel _clockComboxSelect;

    private ComboxItemModel _cameraSelect;

    private ComboxItemModel _audioSelect;

    private readonly MediaPlayer _mediaPlayer;

    SoftwareBitmap frameServerDest = null;

    CanvasImageSource canvasImageSource = null;

    private float j1 = 0;
    private float j2 = 0;
    private float j3 = 0;
    private float j4 = 0;
    private float j5 = 0;
    private float j6 = 0;

    private ICommand _clockChangedCommand;
    public ICommand ClockChangedCommand => _clockChangedCommand ??= new RelayCommand(ClockChanged);

    private ICommand _cameraCommand;
    public ICommand CameraCommand => _cameraCommand ??= new RelayCommand(CameraChanged);

    private ICommand _audioCommand;
    public ICommand AudioCommand => _audioCommand ??= new RelayCommand(AudioChanged);

    private ICommand _testPlayEmojiCommand;

    private ICommand _testVoiceCommand;
    public ICommand TestPlayEmojiCommand => _testPlayEmojiCommand ??= new RelayCommand(TestPlayEmoji);

    public ICommand TestVoiceCommand => _testVoiceCommand ??= new RelayCommand(TestVoice);

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


        var stream = await _speechAndTTSService.TextToSpeechAsync(text);

        _mediaPlayer.SetStreamSource(stream);

        var selectedDevice = (DeviceInformation)AudioSelect?.Tag;

        if (selectedDevice != null)
        {
            _mediaPlayer.AudioDevice = selectedDevice;
        }

        _mediaPlayer.Play();
    }

    private readonly IntPtr _hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
    public MainViewModel(
        ILocalSettingsService localSettingsService,
        IClockViewProviderFactory viewProviderFactory,
        ComboxDataService comboxDataService,
        DispatcherTimer dispatcherTimer,
        ObjectPickerService objectPickerService,
        MediaPlayer mediaPlayer,
        IActionExpressionProviderFactory actionExpressionProviderFactory,
        ISpeechAndTTSService speechAndTTSService)
    {
        _localSettingsService = localSettingsService;

        _dispatcherTimer = dispatcherTimer;

        _speechAndTTSService = speechAndTTSService;

        _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;

        _viewProviderFactory = viewProviderFactory;

        _expressionProviderFactory = actionExpressionProviderFactory;

        ClockComboxModels = comboxDataService.GetClockViewComboxList();

        _objectPickerService = objectPickerService;

        _mediaPlayer = mediaPlayer;

        _mediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;

        _mediaPlayer.IsVideoFrameServerEnabled = true;

        var defaultProvider = _expressionProviderFactory.CreateActionExpressionProvider("Default");

        _actionExpressionProvider = defaultProvider;

        ElectronBotHelper.Instance.SerialPort.DataReceived += SerialPort_DataReceived;

        EmojiPlayHelper.Current.Start();
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

            var selectedDevice = (DeviceInformation)AudioSelect?.Tag;

            if (selectedDevice != null)
            {
                mediaPlayer.AudioDevice = selectedDevice;
            }

            mediaPlayer.Play();
        }
    }

    private async void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        await _speechAndTTSService.InitializeRecognizerAsync(SpeechRecognizer.SystemSpeechLanguage);

        await _speechAndTTSService.StartAsync();
    }

    public int SelectIndex
    {
        get => _selectIndex;
        set => SetProperty(ref _selectIndex, value);
    }

    public int Interval
    {
        get => _interval;
        set => SetProperty(ref _interval, value);
    }

    /// <summary>
    /// 时钟选中数据
    /// </summary>
    public ComboxItemModel ClockComBoxSelect
    {
        get => _clockComboxSelect;
        set => SetProperty(ref _clockComboxSelect, value);
    }

    /// <summary>
    /// 选中的相机
    /// </summary>
    public ComboxItemModel CameraSelect
    {
        get => _cameraSelect;
        set => SetProperty(ref _cameraSelect, value);
    }

    /// <summary>
    /// 选中的音频设备
    /// </summary>
    public ComboxItemModel AudioSelect
    {
        get => _audioSelect;
        set => SetProperty(ref _audioSelect, value);
    }

    /// <summary>
    /// 表盘列表
    /// </summary>
    public ObservableCollection<ComboxItemModel> ClockComboxModels
    {
        get => _clockComboxModels;
        set => SetProperty(ref _clockComboxModels, value);
    }

    /// <summary>
    /// 相机列表
    /// </summary>
    public ObservableCollection<ComboxItemModel> Cameras
    {
        get => _cameras;
        set => SetProperty(ref _cameras, value);
    }

    /// <summary>
    /// 音频设备列表
    /// </summary>
    public ObservableCollection<ComboxItemModel> AudioDevs
    {
        get => _audioDevs;
        set => SetProperty(ref _audioDevs, value);
    }

    /// <summary>
    /// 当前播放表情
    /// </summary>
    public ImageSource EmojiImageSource
    {
        get => _emojiImageSource;
        set => SetProperty(ref _emojiImageSource, value);
    }

    /// <summary>
    /// 表盘内容
    /// </summary>
    public UIElement Element
    {
        get => _element;
        set => SetProperty(ref _element, value);
    }

    private void TestPlayEmoji()
    {
        try
        {
            _dispatcherTimer.Stop();

            EmojiPlayHelper.Current.Interval = 0;

            var r = new Random().Next(Constants.POTENTIAL_EMOJI_LIST.Count);

            _mediaPlayer.Source = MediaSource.CreateFromUri(new Uri($"ms-appx:///Assets/Emoji/{Constants.POTENTIAL_EMOJI_LIST[r]}.mp4"));

            var selectedDevice = (DeviceInformation)AudioSelect?.Tag;

            if (selectedDevice != null)
            {
                _mediaPlayer.AudioDevice = selectedDevice;
            }
            _mediaPlayer.Play();

            _actionExpressionProvider.PlayActionExpressionAsync($"{Constants.POTENTIAL_EMOJI_LIST[r]}", actions.ToList());
        }
        catch (Exception ex)
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
    /// 音频切换方法
    /// </summary>
    private async void AudioChanged()
    {
        var audioName = _audioSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(audioName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultAudioNameKey, _audioSelect);
        }

        var selectedDevice = (DeviceInformation)_audioSelect?.Tag ?? (DeviceInformation)(AudioDevs.FirstOrDefault()).Tag;

        if (selectedDevice != null)
        {
            _mediaPlayer.AudioDevice = selectedDevice;
        }
    }

    /// <summary>
    /// 表盘切换方法
    /// </summary>
    private void ClockChanged()
    {
        var clockName = _clockComboxSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(clockName))
        {
            var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockName);

            if (clockName == "GooeyFooter")
            {
                EmojiPlayHelper.Current.Clear();

                _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 40);
            }
            else
            {
                EmojiPlayHelper.Current.Clear();

                _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            }

            Element = viewProvider.CreateClockView(clockName);
        }
    }

    /// <summary>
    /// 相机选择方法
    /// </summary>
    private async void CameraChanged()
    {
        var cameraName = _cameraSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(cameraName))
        {
            await _localSettingsService.SaveSettingAsync(Constants.DefaultCameraNameKey, _cameraSelect);
        }
    }


    /// <summary>
    /// 头部舵机
    /// </summary>
    public float J1
    {
        get => j1;
        set => SetProperty(ref j1, value);
    }

    /// <summary>
    /// 左臂展开
    /// </summary>
    public float J2
    {
        get => j2;
        set => SetProperty(ref j2, value);
    }
    /// <summary>
    /// 左臂旋转
    /// </summary>
    public float J3
    {
        get => j3;
        set => SetProperty(ref j3, value);
    }
    /// <summary>
    /// 右臂展开
    /// </summary>
    public float J4
    {
        get => j4;
        set => SetProperty(ref j4, value);
    }
    /// <summary>
    /// 右臂旋转
    /// </summary>
    public float J5
    {
        get => j5;
        set => SetProperty(ref j5, value);
    }
    /// <summary>
    /// 底盘转动
    /// </summary>
    public float J6
    {
        get => j6;
        set => SetProperty(ref j6, value);
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

                var jointAngles = ElectronBotHelper.Instance.ElectronBot.GetJointAngles();

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

                EmojiPlayHelper.Current.Interval = 0;

                var clockName = _clockComboxSelect?.DataKey;

                if (clockName != "GooeyFooter")
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

                EmojiPlayHelper.Current.Interval = 0;

                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Interval);
                _dispatcherTimer.Start();
            }
        }
        else
        {
            _dispatcherTimer.Stop();
        }
    }

    public ElectronBotAction SelectdAction
    {
        get => selectdAction;
        set => SetProperty(ref selectdAction, value);
    }

    public ObservableCollection<ElectronBotAction> Actions
    {
        get => actions;
        set => SetProperty(ref actions, value);
    }

    private ICommand _importCommand;

    /// <summary>
    /// 导入动作列表
    /// </summary>
    public ICommand ImportCommand
    {
        get
        {
            _importCommand ??= new RelayCommand(
                    async () =>
                    {
                        var list = await EbHelper.ImportActionListAsync(_hwnd);

                        Actions = new ObservableCollection<ElectronBotAction>(list);
                    });

            return _importCommand;
        }
    }

    private ICommand _playCommand;

    public ICommand PlayCommand
    {
        get
        {
            _playCommand ??= new RelayCommand(
                   async () =>
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
                    });

            return _playCommand;
        }
    }

    private ICommand _stopCommand;

    public ICommand StopCommand
    {
        get
        {
            _stopCommand ??= new RelayCommand(
                    () =>
                    {
                        _dispatcherTimer.Stop();
                    });

            return _stopCommand;
        }
    }
    private ICommand _clearCommand;

    public ICommand ClearCommand
    {
        get
        {
            _clearCommand ??= new RelayCommand(
                    () =>
                    {
                        actions.Clear();

                        count = 0;

                        actionCount = 0;

                        ToastHelper.SendToast("PlayClearToastText".GetLocalized(), TimeSpan.FromSeconds(3));
                    });

            return _clearCommand;
        }
    }

    private ICommand _resetCommand;
    public ICommand ResetCommand
    {
        get
        {
            _resetCommand ??= new RelayCommand(
                    async () =>
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
                    });

            return _resetCommand;
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


    private ICommand _exportCommand;

    public ICommand ExportCommand
    {
        get
        {
            _exportCommand ??= new RelayCommand(
                    async () =>
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
                    });

            return _exportCommand;
        }
    }

    private ICommand _addCommand;

    public ICommand AddCommand
    {
        get
        {
            _addCommand ??= new RelayCommand(
                    () =>
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
                    });

            return _addCommand;
        }
    }


    private ICommand _removeActionCommand;

    public ICommand RemoveActionCommand
    {
        get
        {
            _removeActionCommand ??= new RelayCommand(
                    () =>
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
                    });

            return _removeActionCommand;
        }
    }


    private ICommand _addPictureCommand;

    public ICommand AddPictureCommand
    {
        get
        {
            _addPictureCommand ??= new RelayCommand(
                    async () =>
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

                            var croppedImage = await CropImage(config);

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
                    });

            return _addPictureCommand;
        }
    }

    private async Task<WriteableBitmap> CropImage(ImageCropperConfig config)
    {
        var startOption = new PickerOpenOption
        {
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var ret = await _objectPickerService
            .PickSingleObjectAsync<WriteableBitmap>
            (typeof(ImageCropperPickerViewModel).FullName, config, startOption);

        if (!ret.Canceled)
        {
            return ret.Result;
        }
        return null;
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

                    ElectronBotHelper.Instance.PlayEmoticonActionFrame(frame);
                }
            }
        });
    }

    public async void OnNavigatedTo(object parameter)
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

        var audioDevs = await EbHelper.FindAudioDeviceListAsync();

        var audioModel = await _localSettingsService
            .ReadSettingAsync<ComboxItemModel>(Constants.DefaultAudioNameKey);

        if (audioModel != null)
        {
            AudioSelect = audioDevs.FirstOrDefault(c => c.DataValue == audioModel.DataValue);
        }
    }
    public void OnNavigatedFrom()
    {
        EmojiPlayHelper.Current.Interval = 0;
        _dispatcherTimer.Stop();
        // await _speechAndTTSService.ReleaseRecognizerAsync();
    }
}
