using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Extensions;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.WinUI;
using Mediapipe.Net.Solutions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using SharpDX;
using Verdure.ElectronBot.Core.Models;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.FaceAnalysis;
using BoundingBox = SharpDX.BoundingBox;
using Camera = HelixToolkit.WinUI.Camera;
using Matrix = SharpDX.Matrix;
using Vector3 = SharpDX.Vector3;

namespace ElectronBot.Braincase.ViewModels;

public partial class PoseRecognitionViewModel : ObservableRecipient
{

    /// <summary>
    /// 姿态数据
    ///</summary>
    [ObservableProperty]
    ImageSource _poseImageSource;

    CanvasImageSource? _canvasImageSource = null;

    public IEffectsManager EffectsManager
    {
        get;
    }

    public SceneNodeGroupModel3D BodyModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D LeftArmModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D RightArmModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D HeadModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public SceneNodeGroupModel3D BaseModel
    {
        get;
    } = new SceneNodeGroupModel3D();

    public DiffuseMaterial Material
    {
        private set;
        get;
    }

    [ObservableProperty] bool _showAxis = true;

    [ObservableProperty]
    private Vector3 _modelCentroidPoint = default;

    [ObservableProperty]
    private Vector3 _headModelCentroidPoint = default;

    [ObservableProperty]
    private bool _showWireframe = false;
    [ObservableProperty]
    private BoundingBox _rightShoulderBoundingBox = default;

    [ObservableProperty]
    private BoundingBox _leftShoulderBoundingBox = default;

    [ObservableProperty]
    private BoundingBox _bodyBoundingBox = default;

    [ObservableProperty]
    private BoundingBox _headBoundingBox = default;

    [ObservableProperty]
    private BoundingBox _baseBoundingBox = default;

    private Matrix _bodyMt = default;

    private Matrix _leftArmMt = default;

    private Matrix _rightArmMt = default;

    private Matrix _headMt = default;

    private Matrix _baseMt = default;

    [ObservableProperty] private TextureModel _environmentMap;

    private bool _isInitialized = false;

    private readonly Importer _importer = new();

    private readonly DiffuseMaterial _pinkModelMaterial = new()
    {
        Name = "Pink",
        DiffuseColor = Color.LightPink// DiffuseMaterials.ToColor(255, 192, 203, 1.0),
    };

    [ObservableProperty] private SolidColorBrush _cameraBackground = new(Colors.Red);

    [ObservableProperty]
    private Image _faceImage = new();

    [ObservableProperty]
    private SoftwareBitmapSource _faceBoxSource;

    [ObservableProperty]
    private string _faceText;

    [ObservableProperty]
    private string _faceIcon;

    SoftwareBitmap? _frameServerDest = null;

    SoftwareBitmap? _faceSoftwareBitmap = null;

    private static PoseCpuSolution? calculator;

    private FaceDetector _faceDetector;
    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    public PoseRecognitionViewModel(IEffectsManager effectsManager)
    {
        EffectsManager = effectsManager;

        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("eyes-closed.png")
        };

        calculator = new(modelComplexity: 2, smoothLandmarks: false);

        var filePath = Package.Current.InstalledLocation.Path + "\\Assets\\Cubemap_Grandcanyon.dds";

        EnvironmentMap = LoadTextureByFullPath(filePath);

        CurrentEmojis._emojis = new EmojiCollection();
    }

    [RelayCommand]
    public void PlayAction()
    {
        var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

        if (!playEmojisLock)
        {
            //随机播放表情
            ElectronBotHelper.Instance.ToPlayEmojisRandom();
        }

        ElectronBotHelper.Instance.PlayEmojisLock = true;
    }

    [ObservableProperty]
    private string _leftUpResultLabel;
    [ObservableProperty]
    private string _RightUpResultLabel;
    [ObservableProperty]
    private string _leftWaveResultLabel;
    [ObservableProperty]
    private string _rightWaveResultLabel;

    #region load model
    [RelayCommand]
    public void Loaded()
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                var body = new List<string>()
                {
                    "Body1.obj",
                    "Body2.obj",
                };

                var head = new List<string>()
                {
                    "Head1.obj",
                    "Head2.obj",
                    "Head3.obj",
                };

                var leftArm = new List<string>()
                {
                    "LeftArm1.obj",
                    "LeftArm2.obj",
                    "LeftShoulder.obj",
                };

                var rightArm = new List<string>()
                {
                    "RightArm1.obj",
                    "RightArm2.obj",
                    "RightShoulder.obj"
                };


                var baseBody = new List<string>()
                {
                    "Base.obj",
                };

                foreach (var modelName in head)
                {
                    var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";

                    var newScene = _importer.Load(modelPath);

                    if (newScene != null && newScene.Root != null)
                    {
                        // Pre-attach and calculate all scene info in a separate task.
                        newScene.Root.Attach(EffectsManager);

                        newScene.Root.UpdateAllTransformMatrix();

                        HeadModel.AddNode(newScene.Root);

                        _headMt = HeadModel.HxTransform3D;

                        if (modelName == "Head3.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    // this is face
                                    meshNode.Material = Material;
                                }
                            }
                        }
                        else if (modelName == "Head1.obj")
                        {
                            if (newScene.Root.TryGetCentroid(out var centroid))
                            {
                                // Must use UI thread to set value back.
                                HeadModelCentroidPoint = centroid;
                            }

                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = _pinkModelMaterial;
                                }
                            }
                        }
                        else if (modelName == "Head2.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = DiffuseMaterials.PureWhite;
                                }
                            }
                        }
                    }
                }


                foreach (var modelName in body)
                {
                    var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";

                    var newScene = _importer.Load(modelPath);

                    if (newScene != null && newScene.Root != null)
                    {
                        // Pre-attach and calculate all scene info in a separate task.
                        newScene.Root.Attach(EffectsManager);
                        newScene.Root.UpdateAllTransformMatrix();

                        BodyModel.AddNode(newScene.Root);

                        _bodyMt = BodyModel.HxTransform3D;

                        if (modelName == "Body2.obj")
                        {
                            if (newScene.Root.TryGetCentroid(out var centroid))
                            {
                                // Must use UI thread to set value back.
                                ModelCentroidPoint = centroid;
                            }

                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = DiffuseMaterials.PureWhite;
                                }
                            }
                        }
                        else if (modelName == "Body1.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = _pinkModelMaterial;
                                }
                            }
                        }
                    }
                }

                foreach (var modelName in rightArm)
                {
                    var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";
                    var newScene = _importer.Load(modelPath);
                    if (newScene != null && newScene.Root != null)
                    {
                        // Pre-attach and calculate all scene info in a separate task.
                        newScene.Root.Attach(EffectsManager);
                        newScene.Root.UpdateAllTransformMatrix();

                        RightArmModel.AddNode(newScene.Root);

                        _rightArmMt = RightArmModel.HxTransform3D;

                        if (newScene.Root.TryGetBound(out var bound))
                        {
                            // Must use UI thread to set value back.
                            if (modelName == "RightShoulder.obj")
                            {
                                RightShoulderBoundingBox = bound;
                            }

                        }

                        if (modelName == "RightArm1.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = DiffuseMaterials.PureWhite;
                                }
                            }
                        }
                        else if (modelName == "RightArm2.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = _pinkModelMaterial;
                                }
                            }
                        }
                    }
                }


                foreach (var modelName in leftArm)
                {
                    var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";
                    var newScene = _importer.Load(modelPath);
                    if (newScene != null && newScene.Root != null)
                    {
                        // Pre-attach and calculate all scene info in a separate task.
                        newScene.Root.Attach(EffectsManager);
                        newScene.Root.UpdateAllTransformMatrix();

                        LeftArmModel.AddNode(newScene.Root);

                        _leftArmMt = LeftArmModel.HxTransform3D;

                        if (newScene.Root.TryGetBound(out var bound))
                        {
                            // Must use UI thread to set value back.
                            if (modelName == "LeftShoulder.obj")
                            {
                                LeftShoulderBoundingBox = bound;
                            }

                        }

                        if (modelName == "LeftArm1.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = DiffuseMaterials.PureWhite;
                                }
                            }
                        }
                        else if (modelName == "LeftArm2.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = _pinkModelMaterial;
                                }
                            }
                        }
                    }
                }

                foreach (var modelName in baseBody)
                {
                    var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";
                    var newScene = _importer.Load(modelPath);
                    if (newScene != null && newScene.Root != null)
                    {
                        // Pre-attach and calculate all scene info in a separate task.
                        newScene.Root.Attach(EffectsManager);
                        newScene.Root.UpdateAllTransformMatrix();

                        BaseModel.AddNode(newScene.Root);

                        _baseMt = BaseModel.HxTransform3D;

                        if (newScene.Root.TryGetBound(out var bound))
                        {
                            // Must use UI thread to set value back.
                            if (modelName == "Base.obj")
                            {
                                BaseBoundingBox = bound;
                            }

                        }

                        if (modelName == "Base.obj")
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = DiffuseMaterials.PureWhite;
                                }
                            }
                        }
                    }
                }

                FocusCameraToScene();

                ElectronBotHelper.Instance.ModelActionFrame += Instance_ModelActionFrame;

                await InitAsync();
            }
            catch (Exception)
            {
                ToastHelper.SendToast("模型加载失败", TimeSpan.FromSeconds(3));
            }
        });
    }
    #endregion


    private async Task InitAsync()
    {
        if (_isInitialized)
        {
            CameraFrameService.Current.SoftwareBitmapFrameCaptured -= Current_SoftwareBitmapFrameCaptured;

            CameraFrameService.Current.SoftwareBitmapFramePosePredictResult -= Current_SoftwareBitmapFramePosePredictResult;

            IntelligenceService.Current.CleanUp();

            IntelligenceService.Current.IntelligenceServiceEmotionClassified -= Current_IntelligenceServiceEmotionClassified;

            CurrentEmojis._currentEmoji = null;

            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        else
        {
            await InitializeScreenAsync();
        }
    }

    private async Task InitializeScreenAsync()
    {

        CameraBackground = new SolidColorBrush(Colors.Green);

        _faceDetector = await FaceDetector.CreateAsync();

        await CameraFrameService.Current.PickNextMediaSourceWorkerAsync(FaceImage);

        CameraFrameService.Current.SoftwareBitmapFrameCaptured += Current_SoftwareBitmapFrameCaptured;

        CameraFrameService.Current.SoftwareBitmapFramePosePredictResult += Current_SoftwareBitmapFramePosePredictResult;

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

        IntelligenceService.Current.IntelligenceServiceEmotionClassified += Current_IntelligenceServiceEmotionClassified;

        _isInitialized = true;

    }


    /// <summary>
    /// 表情识别回调
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Current_IntelligenceServiceEmotionClassified(object? sender, ClassifiedEmojiEventArgs e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            //在这里就可以做自己的操作了
            CurrentEmojis._currentEmoji = e.ClassifiedEmoji;

            FaceText = CurrentEmojis._currentEmoji.Name;

            FaceIcon = CurrentEmojis._currentEmoji.Icon;
        });
    }

    private void Current_SoftwareBitmapFramePosePredictResult(object? sender, PoseOutput e)
    {

        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                if (e.PoseLandmarks is not null)
                {
                    var leftUpAngle = AngleHelper.GetPointAngle(
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[24].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[24].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[14].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[12].Y * _frameServerDest.PixelHeight));
                    LeftUpResultLabel = $"LeftUp: {leftUpAngle}";

                    var rightUpAngle = AngleHelper.GetPointAngle(
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[13].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[23].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[23].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[11].Y * _frameServerDest.PixelHeight));
                    RightUpResultLabel = $"RightUp: {rightUpAngle}";


                    var leftWaveAngle = AngleHelper.GetPointAngle(
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[16].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[16].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[12].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[14].Y * _frameServerDest.PixelHeight));
                    LeftWaveResultLabel = $"LeftWave: {leftWaveAngle}";

                    var rightWaveAngle = AngleHelper.GetPointAngle(
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[15].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[15].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[11].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[13].Y * _frameServerDest.PixelHeight));
                    RightWaveResultLabel = $"RightWave: {rightWaveAngle}";

                    var headAngle = AngleHelper.GetPointAngle(
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[11].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[12].Y * _frameServerDest.PixelHeight),
                        new System.Numerics.Vector2(e.PoseLandmarks.Landmark[0].X * _frameServerDest.PixelWidth,
                            e.PoseLandmarks.Landmark[0].Y * _frameServerDest.PixelHeight));

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

                    var canvasDevice = App.GetService<CanvasDevice>();

                    if (_canvasImageSource == null)
                    {
                        _canvasImageSource = new CanvasImageSource(canvasDevice, _frameServerDest.PixelWidth, _frameServerDest.PixelHeight, 96);//96); 

                        PoseImageSource = _canvasImageSource;
                    }

                    using (var inputBitmap = CanvasBitmap.CreateFromSoftwareBitmap(canvasDevice, _frameServerDest))
                    {
                        using (var ds = _canvasImageSource.CreateDrawingSession(Microsoft.UI.Colors.Black))
                        {
                            ds.DrawImage(inputBitmap);
                            var poseLineList = e.GetPoseLines(_frameServerDest.PixelWidth, _frameServerDest.PixelHeight);
                            foreach (var postLine in poseLineList)
                            {
                                ds.DrawLine(postLine.StartVector2, postLine.EndVector2, Microsoft.UI.Colors.Green, 8);
                            }
                            foreach (var Landmark in e.PoseLandmarks.Landmark)
                            {

                                var x = (int)_frameServerDest.PixelWidth * Landmark.X;
                                var y = (int)_frameServerDest.PixelHeight * Landmark.Y;
                                ds.DrawCircle(x, y, 4, Microsoft.UI.Colors.Red, 8);
                            }
                        }
                    }

                    var data = new byte[240 * 240 * 3];

                    var frame = new EmoticonActionFrame(data, true, j1, (rightWaveAngle / 180) * 30, rightUpAngle, (leftWaveAngle / 180) * 30, leftUpAngle, 0);

                    //待处理面部数据
                    await EbHelper.ShowDataToDeviceAsync(_faceSoftwareBitmap, frame);
                }
            }
            catch (Exception ex)
            {
                ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(3));
            }
        });
    }

    private void Current_SoftwareBitmapFrameCaptured(object? sender, SoftwareBitmapEventArgs e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                if (e.SoftwareBitmap is not null)
                {

                    if (e.SoftwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                        e.SoftwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                    {
                        e.SoftwareBitmap = SoftwareBitmap.Convert(
                            e.SoftwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }
                    var service = App.GetService<PoseRecognitionService>();

                    _frameServerDest = e.SoftwareBitmap;

                    var face = await FaceDetectionAsync(e.SoftwareBitmap);

                    if (face is not null)
                    {

                        var source = new SoftwareBitmapSource();

                        await source.SetBitmapAsync(face);

                        _faceSoftwareBitmap = face;
                        // Set the source of the Image control
                        FaceBoxSource = source;

                    }
              
                    _ = service.PosePredictResultUnUseQueueAsync(calculator, e.SoftwareBitmap);

                    _ = IntelligenceService.Current.EmotionClassificationAsync(e.SoftwareBitmap);
                }
            }
            catch (Exception ex)
            {
                ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(3));
            }
        });
    }

    public async void UnLoaded()
    {
        ElectronBotHelper.Instance.ModelActionFrame -= Instance_ModelActionFrame;
        HeadModel.Dispose();
        BodyModel.Dispose();
        RightArmModel.Dispose();
        LeftArmModel.Dispose();
        BaseModel.Dispose();
        EffectsManager.Dispose();
        _importer.Dispose();

        CameraFrameService.Current.SoftwareBitmapFrameCaptured -= Current_SoftwareBitmapFrameCaptured;

        CameraFrameService.Current.SoftwareBitmapFramePosePredictResult -= Current_SoftwareBitmapFramePosePredictResult;
        var service = App.GetService<EmoticonActionFrameService>();
        service.ClearQueue();
        await CleanUpAsync();

        _frameServerDest?.Dispose();
        _faceSoftwareBitmap?.Dispose();
    }

    private void Instance_ModelActionFrame(object? sender, ModelActionFrame e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            BodyModel.HxTransform3D = _bodyMt * Matrix.RotationY(MathUtil.DegreesToRadians((e.J6)));

            if (e.FrameStream.Length > 0)
            {
                Material = new DiffuseMaterial()
                {
                    EnableUnLit = false,
                    DiffuseMap = LoadTextureByStream(e.FrameStream)
                };
            }

            var nodeList = HeadModel.GroupNode;

            foreach (var itemMode in nodeList.Items)
            {
                if (itemMode.Name == "Head3.obj")
                {
                    foreach (var node in itemMode.Traverse())
                    {
                        if (node is MeshNode meshNode)
                        {
                            meshNode.Material = Material;
                        }
                    }
                }
            }

            var rightList = RightShoulderBoundingBox.GetCorners();

            var rightAverage = new SharpDX.Vector3(
                (rightList[1].X + rightList[5].X) / 2f,
                ((rightList[1].Y + rightList[5].Y) / 2f) - 8f,
                (rightList[1].Z + rightList[5].Z) / 2f);

            var leftList = LeftShoulderBoundingBox.GetCorners();

            var leftAverage = new SharpDX.Vector3(
                (leftList[0].X + leftList[4].X) / 2f,
                ((leftList[0].Y + leftList[4].Y) / 2f) - 8f,
                (leftList[0].Z + leftList[4].Z) / 2f);

            var translationMatrix = Matrix.Translation(-rightAverage.X, -rightAverage.Y, -rightAverage.Z);

            var tr2 = _rightArmMt * translationMatrix;

            var tr3 = tr2 * Matrix.RotationZ(MathUtil.DegreesToRadians(-(e.J2)));
            var tr4 = tr3 * Matrix.RotationX(MathUtil.DegreesToRadians(-(e.J3)));

            var tr5 = tr4 * Matrix.Translation(rightAverage.X, rightAverage.Y, rightAverage.Z);


            var tr6 = tr5 * Matrix.RotationY(MathUtil.DegreesToRadians((e.J6)));

            RightArmModel.HxTransform3D = tr6;


            var leftMatrix = Matrix.Translation(-leftAverage.X, -leftAverage.Y, -leftAverage.Z);

            var leftTr2 = _leftArmMt * leftMatrix;

            var leftTr3 = leftTr2 * Matrix.RotationZ(MathUtil.DegreesToRadians((e.J4)));
            var leftTr4 = leftTr3 * Matrix.RotationX(MathUtil.DegreesToRadians(-(e.J5)));

            var leftTr5 = leftTr4 * Matrix.Translation(leftAverage.X, leftAverage.Y, leftAverage.Z);


            var leftTr6 = leftTr5 * Matrix.RotationY(MathUtil.DegreesToRadians((e.J6)));

            LeftArmModel.HxTransform3D = leftTr6;

            var headMatrix = Matrix.Translation(-HeadModelCentroidPoint.X, -HeadModelCentroidPoint.Y, -HeadModelCentroidPoint.Z);

            var headTr2 = _headMt * headMatrix;

            var headTr3 = headTr2 * Matrix.RotationX(MathUtil.DegreesToRadians(-(e.J1)));

            var headTr4 = headTr3 * Matrix.Translation(HeadModelCentroidPoint.X, HeadModelCentroidPoint.Y, HeadModelCentroidPoint.Z);


            var headTr5 = headTr4 * Matrix.RotationY(MathUtil.DegreesToRadians((e.J6)));

            HeadModel.HxTransform3D = headTr5;
        });
    }

    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(RightShoulderBoundingBox.Width, RightShoulderBoundingBox.Height), BodyBoundingBox.Depth) + 240;
        var pos = RightShoulderBoundingBox.Center + new Vector3(0, 0, maxWidth);
        Camera.Position = pos;
        Camera.LookDirection = RightShoulderBoundingBox.Center - pos;
        Camera.UpDirection = Vector3.UnitY;
        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }

    [RelayCommand]
    public void CompactOverlay()
    {
        App.MainWindow.Show();
    }

    private TextureModel LoadTextureByFullPath(string filePath)
    {
        return TextureModel.Create(filePath);
    }

    private TextureModel LoadTexture(string file)
    {
        var filePath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\Pic\\{file}";

        return TextureModel.Create(filePath);
    }

    private TextureModel LoadTextureByStream(Stream data)
    {

        return TextureModel.Create(data);
    }

    private async Task CleanUpAsync()
    {
        try
        {
            _isInitialized = false;

            IntelligenceService.Current.CleanUp();

            IntelligenceService.Current.IntelligenceServiceEmotionClassified -= Current_IntelligenceServiceEmotionClassified;

            CurrentEmojis._currentEmoji = null;

            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        catch (Exception)
        {

        }
    }

    private async Task<SoftwareBitmap?> FaceDetectionAsync(SoftwareBitmap? imgSoftwareBitmap)
    {
        Debug.WriteLine("FrameCaptured");
        Debug.WriteLine($"Frame evaluation started {DateTime.Now}");
        if (imgSoftwareBitmap != null)
        {
            BitmapPixelFormat bpf = imgSoftwareBitmap.BitmapPixelFormat;

            var uncroppedBitmap = SoftwareBitmap.Convert(imgSoftwareBitmap, BitmapPixelFormat.Nv12);
            var faces = await _faceDetector.DetectFacesAsync(uncroppedBitmap);

            if (faces.Count > 0)
            {
                //crop image to focus on face portion
                var faceBox = faces[0].FaceBox;

                VideoFrame inputFrame = VideoFrame.CreateWithSoftwareBitmap(imgSoftwareBitmap);

                VideoFrame tmp = null;

                tmp = new VideoFrame(imgSoftwareBitmap.BitmapPixelFormat, (int)(faceBox.Width + faceBox.Width % 2) - 2,
                    (int)(faceBox.Height + faceBox.Height % 2) - 2);

                await inputFrame.CopyToAsync(tmp, faceBox, null);

                if (tmp.SoftwareBitmap is not null)
                {
                    return tmp.SoftwareBitmap;
                }
            }
        }
        return null;
    }

    private static BitmapBounds GetCropBounds(int srcWidth, int srcHeight, int targetWidth, int targetHeight)
    {
        var modelHeight = targetHeight;
        var modelWidth = targetWidth;
        BitmapBounds bounds = new BitmapBounds();
        // we need to recalculate the crop bounds in order to correctly center-crop the input image
        float flRequiredAspectRatio = (float)modelWidth / modelHeight;

        if (flRequiredAspectRatio * srcHeight > (float)srcWidth)
        {
            // clip on the y axis
            bounds.Height = (uint)Math.Min((srcWidth / flRequiredAspectRatio + 0.5f), srcHeight);
            bounds.Width = (uint)srcWidth;
            bounds.X = 0;
            bounds.Y = (uint)(srcHeight - bounds.Height) / 2;
        }
        else // clip on the x axis
        {
            bounds.Width = (uint)Math.Min((flRequiredAspectRatio * srcHeight + 0.5f), srcWidth);
            bounds.Height = (uint)srcHeight;
            bounds.X = (uint)(srcWidth - bounds.Width) / 2; ;
            bounds.Y = 0;
        }
        return bounds;
    }
}
