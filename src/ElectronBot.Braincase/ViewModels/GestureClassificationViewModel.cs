using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ClockViews;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Core.Models;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Services;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Mediapipe.Net.Solutions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using OpenCvSharp.Extensions;
using Services;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Image = Microsoft.UI.Xaml.Controls.Image;

namespace ElectronBot.Braincase.ViewModels;

public partial class GestureClassificationViewModel : ObservableRecipient, INavigationAware
{
    private bool _isInitialized = false;
    private static HandsCpuSolution? calculator;

    private bool _isBeginning = false;

    DispatcherTimer dispatcherTimer = new DispatcherTimer();

    private readonly DispatcherTimer _dispatcherTimer;

    private readonly string modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\MLModel1.zip";
    private readonly ILocalSettingsService _localSettingsService;
    public GestureClassificationViewModel(DispatcherTimer dispatcherTimer1,
        ILocalSettingsService localSettingsService)
    {
        calculator = new HandsCpuSolution();
        dispatcherTimer.Interval = TimeSpan.FromMilliseconds(600);
        dispatcherTimer.Tick += DispatcherTimer_Tick;
        _localSettingsService = localSettingsService;
        _dispatcherTimer = dispatcherTimer1;
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 80);

        _dispatcherTimer.Tick += DispatcherTimer_ClockTick;
        Element = new RandomContentView(this);
    }

    private async void DispatcherTimer_ClockTick(object? sender, object e)
    {
        if (ElectronBotHelper.Instance.EbConnected)
        {
            await EbHelper.ShowClockCanvasToDeviceAsync(Element);
        }
    }

    private Storyboard storyboard = new();
    [RelayCommand]
    public async void Loaded(object obj)
    {
        if (obj is Storyboard sb)
        {
            storyboard = sb;
        }

        var list = (await _localSettingsService.ReadSettingAsync<List<RandomContent>>(Constants.RandomContentListKey)) ?? new List<RandomContent>();

        RandomContentList = new(list);
    }
    private void DispatcherTimer_Tick(object sender, object e)
    {
        if (RandomContentList.Count > 0)
        {
            var index = new Random().Next(0, RandomContentList.Count);

            var text = RandomContentList[index].Content;
            RandomContentText = $"{text}";
            storyboard.Children[0].SetValue(DoubleAnimation.FromProperty, 0);
            storyboard.Children[0].SetValue(DoubleAnimation.ToProperty, 180);
            storyboard.Begin();
        }
    }

    [ObservableProperty]
    private string randomContentText;

    [ObservableProperty]
    private string randomContentResultText;


    /// <summary>
    /// 随机内容
    /// </summary>
    [ObservableProperty]
    UIElement element;
    [RelayCommand]
    private async Task TestGestureClassficationAsync()
    {
        var handResult = string.Empty;
        var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\hand.png");

        var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

        var dataMeta = mat2.Data;

        var length = mat2.Width * mat2.Height * mat2.Channels();

        var data = new byte[length];

        Marshal.Copy(dataMeta, data, 0, length);

        var widthStep = (int)mat2.Step();

        System.Drawing.Bitmap bitmap = BitmapConverter.ToBitmap(matData);

        var ret = await BitmapToBitmapImage(bitmap);

        if (ret.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                ret.BitmapAlphaMode == BitmapAlphaMode.Straight)
        {
            ret = SoftwareBitmap.Convert(ret, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        }

        var imgframe = new ImageFrame(ImageFormat.Types.Format.Srgb, mat2.Width, mat2.Height, widthStep, data);

        var handsOutput = calculator.Compute(imgframe);

        if (handsOutput.MultiHandLandmarks != null)
        {
            var landmarks = handsOutput.MultiHandLandmarks[0].Landmark;

            Debug.WriteLine($"Got hands output with {landmarks.Count} landmarks");

            handResult = HandDataFormatHelper.PredictResult(landmarks.ToList(), modelPath);
        }
        else
        {
            Debug.WriteLine("No hand landmarks");
        }

        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            var source = new SoftwareBitmapSource();

            await source.SetBitmapAsync(ret);

            // Set the source of the Image control
            ImgFileSource = source;

            ResultLabel = handResult;
        });
    }

    [ObservableProperty]
    public Image faceImage = new();

    [ObservableProperty]
    public SoftwareBitmapSource faceImageSource;

    [ObservableProperty]
    string resultLabel;

    [ObservableProperty]
    SoftwareBitmapSource imgFileSource;
    public async Task<SoftwareBitmap> BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
    {
        MemoryStream ms = new MemoryStream();

        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        ms.Seek(0, SeekOrigin.Begin);

        // Create the decoder from the stream
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ms.AsRandomAccessStream());

        // Get the SoftwareBitmap representation of the file
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        return softwareBitmap;
    }

    public async void OnNavigatedTo(object parameter)
    {
        await InitAsync();
        _dispatcherTimer.Start();
    }

    [ObservableProperty]
    private ObservableCollection<RandomContent> randomContentList = new();

    [RelayCommand]
    public async void RandomContentEdit()
    {
        try
        {
            var theme = App.GetService<IThemeSelectorService>();
            var randomContentEditDialog = new ContentDialog()
            {
                Title = "AddRandomContentTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Content = new RandomContentPage(),
                RequestedTheme = theme.Theme
            };

            randomContentEditDialog.Closed += RandomContentEditDialog_Closed;

            await randomContentEditDialog.ShowAsync();
        }
        catch (Exception)
        {

        }
    }

    private async void RandomContentEditDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {
        var list = (await _localSettingsService.ReadSettingAsync<List<RandomContent>>(Constants.RandomContentListKey)) ?? new List<RandomContent>();

        RandomContentList = new(list);
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
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ResultLabel = e;

            if (RandomContentList.Count > 0)
            {
                if (e == "back" && _isBeginning == false)
                {
                    var service = App.GetService<EmoticonActionFrameService>();
                    service.ClearQueue();
                    //todo：启动动画 并播放 
                    _isBeginning = true;
                    RandomContentResultText = "";
                    dispatcherTimer.Start();
                    Debug.WriteLine("启动动画");
                }
                else if (e == "back" && _isBeginning == true)
                {
                    //当前处于启动状态
                    //不做处理
                }
                else if (e == "land" && _isBeginning == true)
                {
                    var index = new Random().Next(0, RandomContentList.Count);
                    var text = RandomContentList[index].Content;
                    RandomContentResultText = $"{text}";
                    _isBeginning = false;
                    Debug.WriteLine("停止动画");
                    RandomContentText = "";
                    dispatcherTimer.Stop();
                }
            }
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
    public async void OnNavigatedFrom()
    {
        CameraFrameService.Current.SoftwareBitmapFrameCaptured -= Current_SoftwareBitmapFrameCaptured;

        CameraFrameService.Current.SoftwareBitmapFrameHandPredictResult -= Current_SoftwareBitmapFrameHandPredictResult;
        var service = App.GetService<EmoticonActionFrameService>();
        service.ClearQueue();
        await CleanUpAsync();
        dispatcherTimer.Stop();
        _dispatcherTimer.Stop();
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
}
