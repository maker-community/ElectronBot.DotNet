using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using Windows.Graphics.Imaging;
using CommunityToolkit.Mvvm.Input;

namespace ElectronBot.Braincase.ViewModels;

public partial class CameraEmojisViewModel : ObservableRecipient, INavigationAware
{

    private bool _isInitialized = false;

    private string _faceText;

    private string _faceIcon;

    private SoftwareBitmapSource _faceImageSource;

    private SoftwareBitmapSource _faceBoxSource;

    private Image _image = new();

    public CameraEmojisViewModel()
    {
        CurrentEmojis._emojis = new EmojiCollection();
    }

    public Image FaceImage
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    public SoftwareBitmapSource FaceImageSource
    {
        get => _faceImageSource;
        set => SetProperty(ref _faceImageSource, value);
    }

    public SoftwareBitmapSource FaceBoxSource
    {
        get => _faceBoxSource;
        set => SetProperty(ref _faceBoxSource, value);
    }
    public string FaceText
    {
        get => _faceText;
        set => SetProperty(ref _faceText, value);
    }

    public string FaceIcon
    {
        get => _faceIcon;
        set => SetProperty(ref _faceIcon, value);
    }

    [ObservableProperty]
    private bool _isEntityFirstEnabled = true;

    private async Task InitAsync()
    {
        if (_isInitialized)
        {
            await CameraService.Current.CleanupMediaCaptureAsync();
        }
        else
        {
            await InitializeScreenAsync();
        }
    }

    private async Task InitializeScreenAsync()
    {
        await CameraService.Current.PickNextMediaSourceWorkerAsync(FaceImage);

        var isModelLoaded = await IntelligenceService.Current.InitializeAsync();

        if (!isModelLoaded)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast("ModelLoadedFailed".GetLocalized(),
                             TimeSpan.FromSeconds(5));
            });

            return;
        }

        IntelligenceService.Current
            .IntelligenceServiceEmotionClassified += Current_IntelligenceServiceEmotionClassified;

        IntelligenceService.Current.FaceBoxFrameCaptured += Current_FaceBoxFrameCaptured;

        _isInitialized = true;
    }

    [RelayCommand]
    private void OpenEntityFirst(bool isOn)
    {
        try
        {
            //按钮开启
            if (!isOn)
            {
                ElectronBotHelper.Instance.IsEntityFirstEnabled = true;
            }
            else
            {
                ElectronBotHelper.Instance.IsEntityFirstEnabled = false;
            }
        }
        catch (Exception)
        {
        }
    }

    private async void Current_FaceBoxFrameCaptured(object sender, SoftwareBitmapEventArgs e)
    {
        if (e.SoftwareBitmap is not null)
        {

            if (e.SoftwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                  e.SoftwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
            {
                e.SoftwareBitmap = SoftwareBitmap.Convert(
                    e.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }

            App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
            {
                var source = new SoftwareBitmapSource();

                await source.SetBitmapAsync(e.SoftwareBitmap);

                // Set the source of the Image control
                FaceBoxSource = source;
            });

            await EbHelper.ShowFaceToDeviceAsync(e.SoftwareBitmap);
        }

    }

    private void Current_IntelligenceServiceEmotionClassified(object sender, ClassifiedEmojiEventArgs e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            //在这里就可以做自己的操作了
            CurrentEmojis._currentEmoji = e.ClassifiedEmoji;

            FaceText = CurrentEmojis._currentEmoji.Name;

            FaceIcon = CurrentEmojis._currentEmoji.Icon;

        });
    }

    public async void OnNavigatedTo(object parameter)
    {
        var service = App.GetService<EmoticonActionFrameService>();
        service.ClearQueue();
        await InitAsync();
        ElectronBotHelper.Instance.IsEntityFirstEnabled = true;
    }
    public async void OnNavigatedFrom()
    {

        var service = App.GetService<EmoticonActionFrameService>();
        service.ClearQueue();
        await CleanUpAsync();
    }


    private async Task CleanUpAsync()
    {
        try
        {
            _isInitialized = false;

            await CameraService.Current.CleanupMediaCaptureAsync();

            IntelligenceService.Current.CleanUp();

            IntelligenceService.Current.IntelligenceServiceEmotionClassified -= Current_IntelligenceServiceEmotionClassified;

            IntelligenceService.Current.FaceBoxFrameCaptured -= Current_FaceBoxFrameCaptured;

            CurrentEmojis._currentEmoji = null;
        }
        catch (Exception)
        {

        }
    }
}
