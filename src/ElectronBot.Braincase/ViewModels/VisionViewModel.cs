using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using Models.ElectronBot;
using Services.ElectronBot;
using Verdure.ElectronBot.Core.Models;
using Windows.Graphics.Imaging;

namespace ElectronBot.Braincase.ViewModels;

public partial class VisionViewModel : ObservableRecipient, INavigationAware
{
    public VisionViewModel()
    {
        CurrentEmojis._emojis = new EmojiCollection();
    }

    [ObservableProperty]
    private string _faceText;

    [ObservableProperty]
    private string _faceIcon;

    [ObservableProperty]
    private string _HandText;

    public async void OnNavigatedFrom()
    {

        await VisionService.Current.StopAsync();

        VisionService.Current.SoftwareBitmapFramePoseAndHandsPredictResult -= Current_SoftwareBitmapFramePoseAndHandsPredictResult;
    }


    public async void OnNavigatedTo(object parameter)
    {
        await VisionService.Current.StartAsync();

        VisionService.Current.SoftwareBitmapFramePoseAndHandsPredictResult += Current_SoftwareBitmapFramePoseAndHandsPredictResult;
    }

    public void Current_SoftwareBitmapFramePoseAndHandsPredictResult(object? sender, VisionResult e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            if (e.FaceSoftwareBitmap is not null)
            {

                if (e.FaceSoftwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                          e.FaceSoftwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                {
                    e.FaceSoftwareBitmap = SoftwareBitmap.Convert(
                        e.FaceSoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }
            }
            if (e.HandResult != null)
            {
                HandText = e.HandResult;
            }

            if (e.Emoji != null)
            {
                //在这里就可以做自己的操作了
                CurrentEmojis._currentEmoji = e.Emoji;

                FaceText = CurrentEmojis._currentEmoji.Name;

                FaceIcon = CurrentEmojis._currentEmoji.Icon;
            }

            if (e.PoseLandmarks is not null)
            {
                var leftUpAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[24].X * e.Width,
                        e.PoseLandmarks.Landmark[24].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * e.Width,
                        e.PoseLandmarks.Landmark[14].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * e.Width,
                        e.PoseLandmarks.Landmark[12].Y * e.Height));
                //LeftUpResultLabel = $"LeftUp: {leftUpAngle}";

                var rightUpAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * e.Width,
                        e.PoseLandmarks.Landmark[13].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[23].X * e.Width,
                        e.PoseLandmarks.Landmark[23].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * e.Width,
                        e.PoseLandmarks.Landmark[11].Y * e.Height));
                //RightUpResultLabel = $"RightUp: {rightUpAngle}";


                var leftWaveAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[16].X * e.Width,
                        e.PoseLandmarks.Landmark[16].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * e.Width,
                        e.PoseLandmarks.Landmark[12].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * e.Width,
                        e.PoseLandmarks.Landmark[14].Y * e.Height));
                //LeftWaveResultLabel = $"LeftWave: {leftWaveAngle}";

                var rightWaveAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[15].X * e.Width,
                        e.PoseLandmarks.Landmark[15].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * e.Width,
                        e.PoseLandmarks.Landmark[11].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * e.Width,
                        e.PoseLandmarks.Landmark[13].Y * e.Height));
                //RightWaveResultLabel = $"RightWave: {rightWaveAngle}";

                var headAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * e.Width,
                        e.PoseLandmarks.Landmark[11].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * e.Width,
                        e.PoseLandmarks.Landmark[12].Y * e.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[0].X * e.Width,
                        e.PoseLandmarks.Landmark[0].Y * e.Height));

                float j1 = 0;
                if (headAngle < 90)
                {
                    headAngle = 180 - headAngle;
                    j1 = (headAngle / 180) * 20;
                }
                else if (headAngle > 90)
                {
                    j1 = (headAngle / 180) * 15 * (-1);
                }

                var data = new byte[240 * 240 * 3];

                var frame = new EmoticonActionFrame(data, true, j1, (rightWaveAngle / 180) * 30, rightUpAngle, (leftWaveAngle / 180) * 30, leftUpAngle, 0);

                //待处理面部数据
                await EbHelper.ShowDataToDeviceAsync(e.FaceSoftwareBitmap, frame);
            }

        });
    }
}
