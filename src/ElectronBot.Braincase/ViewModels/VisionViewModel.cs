using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using Models.ElectronBot;
using Services.ElectronBot;
using Verdure.ElectronBot.Core.Models;

namespace ElectronBot.Braincase.ViewModels;

public partial class VisionViewModel : ObservableRecipient, INavigationAware
{
    public VisionViewModel()
    {
        CurrentEmojis._emojis = new EmojiCollection();
    }

    //[ObservableProperty]
    //private Camera _camera = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    //[ObservableProperty] private IEffectsManager _effectsManager;

    //[ObservableProperty]
    //private Vector3 _modelCentroidPoint = default;

    //[ObservableProperty] private TextureModel _environmentMap;

    //[ObservableProperty]
    //private SceneNodeGroupModel3D _bodyModel;

    //[ObservableProperty]
    //private SceneNodeGroupModel3D _leftArmModel;

    //[ObservableProperty]

    //private SceneNodeGroupModel3D _rightArmModel;

    //[ObservableProperty]

    //private SceneNodeGroupModel3D _headModel;

    //[ObservableProperty]
    //private SceneNodeGroupModel3D _baseModel;

    [ObservableProperty]
    private string _faceText;

    [ObservableProperty]
    private string _faceIcon;

    [ObservableProperty]
    private string _HandText;

    public async void OnNavigatedFrom()
    {

        //Camera = null;
        //EnvironmentMap = null;
        //BodyModel = null;
        //LeftArmModel = null;
        //RightArmModel = null;
        //HeadModel = null;
        //BaseModel = null;
        //EffectsManager = null;
        await VisionService.Current.StopAsync();

        VisionService.Current.SoftwareBitmapFramePoseAndHandsPredictResult -= Current_SoftwareBitmapFramePoseAndHandsPredictResult;
    }


    public async void OnNavigatedTo(object parameter)
    {
        //Camera = Bot3DHelper.Instance.Camera;
        //EnvironmentMap = Bot3DHelper.Instance.EnvironmentMap;
        //BodyModel = Bot3DHelper.Instance.BodyModel;
        //LeftArmModel = Bot3DHelper.Instance.LeftArmModel;
        //RightArmModel = Bot3DHelper.Instance.RightArmModel;
        //HeadModel = Bot3DHelper.Instance.HeadModel;
        //BaseModel = Bot3DHelper.Instance.BaseModel;
        //ModelCentroidPoint = Bot3DHelper.Instance.ModelCentroidPoint;
        //EffectsManager = Bot3DHelper.Instance.EffectsManager;
        await VisionService.Current.StartAsync();

        VisionService.Current.SoftwareBitmapFramePoseAndHandsPredictResult += Current_SoftwareBitmapFramePoseAndHandsPredictResult;
    }

    public void Current_SoftwareBitmapFramePoseAndHandsPredictResult(object? sender, VisionResult e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
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
                //var canvasDevice = App.GetService<CanvasDevice>();

                //if (_canvasImageSource == null)
                //{
                //    _canvasImageSource = new CanvasImageSource(canvasDevice, e.Width, e.Height, 96);//96); 

                //    PoseImageSource = _canvasImageSource;
                //}

                //using (var inputBitmap = CanvasBitmap.CreateFromSoftwareBitmap(canvasDevice, _frameServerDest))
                //{
                //    using (var ds = _canvasImageSource.CreateDrawingSession(Microsoft.UI.Colors.Black))
                //    {
                //        ds.DrawImage(inputBitmap);
                //        var poseLineList = e.GetPoseLines(e.Width, e.Height);
                //        foreach (var postLine in poseLineList)
                //        {
                //            ds.DrawLine(postLine.StartVector2, postLine.EndVector2, Microsoft.UI.Colors.Green, 8);
                //        }
                //        foreach (var Landmark in e.PoseLandmarks.Landmark)
                //        {

                //            var x = (int)e.Width * Landmark.X;
                //            var y = (int)e.Height * Landmark.Y;
                //            ds.DrawCircle(x, y, 4, Microsoft.UI.Colors.Red, 8);
                //        }
                //    }
                //}

                var data = new byte[240 * 240 * 3];

                var frame = new EmoticonActionFrame(data, true, j1, (rightWaveAngle / 180) * 30, rightUpAngle, (leftWaveAngle / 180) * 30, leftUpAngle, 0);

                //待处理面部数据
                await EbHelper.ShowDataToDeviceAsync(null, frame);
            }

        });
    }
}
