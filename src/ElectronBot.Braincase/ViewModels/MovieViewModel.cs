using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.CompactOverlay;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Helpers;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Models;
using SharpDX;
using Assimp;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using Microsoft.UI.Xaml.Media;
using BoundingBox = SharpDX.BoundingBox;
using Camera = HelixToolkit.WinUI.Camera;
using Matrix = SharpDX.Matrix;
using Microsoft.UI.Xaml.Controls;
using ElectronBot.Braincase.Services;
using Services;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Mediapipe.Net.Solutions;
using Microsoft.UI;
using Constants = ElectronBot.Braincase.Constants;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Windows.Media.Playback;
using ElectronBot.Braincase.Extensions;
using Vector3 = SharpDX.Vector3;
using Verdure.ElectronBot.Core.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Media.FaceAnalysis;
using Windows.Media;
using Windows.Graphics.Capture;
using WinRT.Interop;
using Microsoft.Graph.CallRecords;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.Graphics;
using Microsoft.Graphics.DirectX;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Windows.Storage.Streams;
using System.Runtime.InteropServices;
using Mediapipe.Net.Framework.Format;
using Mediapipe.Net.Framework.Protobuf;
using Color = SharpDX.Color;

namespace ElectronBot.Braincase.ViewModels;

public partial class MovieViewModel : ObservableRecipient
{
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

    private static PoseCpuSolution? calculator;

    private FaceDetector _faceDetector;
    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };


    // Capture API objects.
    private SizeInt32 _lastSize;
    private GraphicsCaptureItem _item;
    private Direct3D11CaptureFramePool _framePool;
    private GraphicsCaptureSession _session;



    // Non-API related members.
    private CanvasDevice _canvasDevice;
    private CompositionGraphicsDevice _compositionGraphicsDevice;
    private Compositor _compositor;
    private CompositionDrawingSurface _surface;
    private CanvasBitmap _currentFrame;
    private string _screenshotFilename = "test.png";


    private PoseRecognitionService _poseRecognitionService;
    private DateTime _lastFrameTime = DateTime.Now;

    private byte[] _faceData = new byte[240 * 240 * 3];

    public MovieViewModel(IEffectsManager effectsManager, PoseRecognitionService poseRecognitionService)
    {
        EffectsManager = effectsManager;

        _poseRecognitionService = poseRecognitionService;

        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("eyes-closed.png")
        };

        calculator = new(modelComplexity: 2, smoothLandmarks: false);

        var filePath = Package.Current.InstalledLocation.Path + "\\Assets\\Cubemap_Grandcanyon.dds";

        EnvironmentMap = LoadTextureByFullPath(filePath);
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
    public async void Loaded()
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

                //ElectronBotHelper.Instance.ModelActionFrame += Instance_ModelActionFrame;

                await InitAsync();

                Setup();
            }
            catch (Exception)
            {
                ToastHelper.SendToast("模型加载失败", TimeSpan.FromSeconds(3));
            }
        });
    }
    #endregion


    private void Setup()
    {
        _canvasDevice = new CanvasDevice();


        _compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(
            App.MainWindow.Compositor, _canvasDevice);

        _compositor = App.MainWindow.Compositor;

        _surface = _compositionGraphicsDevice.CreateDrawingSurface(
            new Size(400, 400),
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);    // This is the only value that currently works with
        // the composition APIs.

        var visual = _compositor.CreateSpriteVisual();
        visual.RelativeSizeAdjustment = System.Numerics.Vector2.One;
        var brush = _compositor.CreateSurfaceBrush(_surface);
        brush.HorizontalAlignmentRatio = 0.5f;
        brush.VerticalAlignmentRatio = 0.5f;
        brush.Stretch = CompositionStretch.Uniform;
        visual.Brush = brush;
        //ElementCompositionPreview.SetElementChildVisual(this, visual); //Commenting out because it only accepts a page and not a window
        ElementCompositionPreview.SetElementChildVisual(App.MainWindow.Content, visual);
    }


    private async Task InitAsync()
    {
        if (_isInitialized)
        {
            CameraFrameService.Current.SoftwareBitmapFramePosePredictResult -= Current_SoftwareBitmapFramePosePredictResult;
            await CameraFrameService.Current.CleanupMediaCaptureAsync();
        }
        else
        {
            await InitializeScreenAsync();
        }
    }

    private async Task InitializeScreenAsync()
    {
        CameraFrameService.Current.SoftwareBitmapFramePosePredictResult += Current_SoftwareBitmapFramePosePredictResult;

        _isInitialized = true;

        CameraBackground = new SolidColorBrush(Colors.Green);

        _faceDetector = await FaceDetector.CreateAsync();
    }

    [RelayCommand]
    public async Task StartCaptureAsync()
    {
        // The GraphicsCapturePicker follows the same pattern the
        // file pickers do.
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);

        var picker = new GraphicsCapturePicker();
        InitializeWithWindow.Initialize(picker, hwnd);
        GraphicsCaptureItem item = await picker.PickSingleItemAsync();

        try
        {
            // The item may be null if the user dismissed the
            // control without making a selection or hit Cancel.
            if (item != null)
            {
                StartCaptureInternal(item);
            }
        }
        catch (Exception)
        {

        }

    }

    private void Current_SoftwareBitmapFramePosePredictResult(object? sender, PoseOutput e)
    {

        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                var leftUpAngle = AngleHelper.GetPointAngle(
            new System.Numerics.Vector2(e.PoseLandmarks.Landmark[24].X * _lastSize.Width,
                e.PoseLandmarks.Landmark[24].Y * _lastSize.Height),
            new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * _lastSize.Width,
                e.PoseLandmarks.Landmark[14].Y * _lastSize.Height),
            new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _lastSize.Width,
                e.PoseLandmarks.Landmark[12].Y * _lastSize.Height));
                LeftUpResultLabel = $"LeftUp: {leftUpAngle}";

                var rightUpAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[13].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[23].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[23].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[11].Y * _lastSize.Height));
                RightUpResultLabel = $"RightUp: {rightUpAngle}";


                var leftWaveAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[16].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[16].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[12].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[14].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[14].Y * _lastSize.Height));
                LeftWaveResultLabel = $"LeftWave: {leftWaveAngle}";

                var rightWaveAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[15].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[15].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[11].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[13].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[13].Y * _lastSize.Height));
                RightWaveResultLabel = $"RightWave: {rightWaveAngle}";

                var headAngle = AngleHelper.GetPointAngle(
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[11].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[11].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[12].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[12].Y * _lastSize.Height),
                    new System.Numerics.Vector2(e.PoseLandmarks.Landmark[0].X * _lastSize.Width,
                        e.PoseLandmarks.Landmark[0].Y * _lastSize.Height));

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


                var frame = new EmoticonActionFrame(_faceData, true, j1, (rightWaveAngle / 180) * 30, rightUpAngle, (leftWaveAngle / 180) * 30, leftUpAngle, 0);

                //待处理面部数据
                await EbHelper.ShowDataToDeviceAsync(null, frame);
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

        CameraFrameService.Current.SoftwareBitmapFramePosePredictResult -= Current_SoftwareBitmapFramePosePredictResult;
        var service = App.GetService<EmoticonActionFrameService>();
        service.ClearQueue();
        await CleanUpAsync();

        StopCapture();
    }

    private void Instance_ModelActionFrame(object? sender, Verdure.ElectronBot.Core.Models.ModelActionFrame e)
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
            var bpf = imgSoftwareBitmap.BitmapPixelFormat;

            var uncroppedBitmap = SoftwareBitmap.Convert(imgSoftwareBitmap, BitmapPixelFormat.Nv12);
            var faces = await _faceDetector.DetectFacesAsync(uncroppedBitmap);

            if (faces.Count > 0)
            {
                //crop image to focus on face portion
                var faceBox = faces[0].FaceBox;

                using var inputFrame = VideoFrame.CreateWithSoftwareBitmap(imgSoftwareBitmap);


                var tmp = new VideoFrame(imgSoftwareBitmap.BitmapPixelFormat, (int)(faceBox.Width + faceBox.Width % 2) - 2,
                     (int)(faceBox.Height + faceBox.Height % 2) - 2);

                await inputFrame.CopyToAsync(tmp, new BitmapBounds(faceBox.X - 20, faceBox.Y - 20, faceBox.Width + 40, faceBox.Height + 40), null);

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



    private void StartCaptureInternal(GraphicsCaptureItem item)
    {
        // Stop the previous capture if we had one.
        StopCapture();

        _item = item;
        _lastSize = _item.Size;

        _framePool = Direct3D11CaptureFramePool.Create(
           _canvasDevice, // D3D device
           Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, // Pixel format
           2, // Number of frames
           _item.Size); // Size of the buffers

        _framePool.FrameArrived += (s, a) =>
        {
            // The FrameArrived event is raised for every frame on the thread
            // that created the Direct3D11CaptureFramePool. This means we
            // don't have to do a null-check here, as we know we're the only
            // one dequeueing frames in our application.  

            // NOTE: Disposing the frame retires it and returns  
            // the buffer to the pool.

            using (var frame = _framePool.TryGetNextFrame())
            {
                ProcessFrame(frame);
            }
        };

        _item.Closed += (s, a) =>
        {
            StopCapture();
        };

        _session = _framePool.CreateCaptureSession(_item);
        _session.StartCapture();
    }

    public void StopCapture()
    {
        _session?.Dispose();
        _framePool?.Dispose();
        _item = null;
        _session = null;
        _framePool = null;
    }

    private async void ProcessFrame(Direct3D11CaptureFrame frame)
    {
        // Resize and device-lost leverage the same function on the
        // Direct3D11CaptureFramePool. Refactoring it this way avoids
        // throwing in the catch block below (device creation could always
        // fail) along with ensuring that resize completes successfully and
        // isn’t vulnerable to device-lost.
        bool needsReset = false;
        bool recreateDevice = false;

        if ((frame.ContentSize.Width != _lastSize.Width) ||
            (frame.ContentSize.Height != _lastSize.Height))
        {
            needsReset = true;
            _lastSize = frame.ContentSize;
        }

        try
        {
            // Take the D3D11 surface and draw it into a  
            // Composition surface.

            using var canvasBitmap = CanvasBitmap.CreateFromDirect3D11Surface(
                _canvasDevice,
                frame.Surface);

            // Convert our D3D11 surface into a Win2D object.
            if (DateTime.Now - _lastFrameTime >= TimeSpan.FromSeconds(1.0 / 25))
            {
                _lastFrameTime = DateTime.Now;


                using IRandomAccessStream stream = new InMemoryRandomAccessStream();

                await canvasBitmap.SaveAsync(stream, CanvasBitmapFileFormat.Png);

                ////_currentFrame = canvasBitmap;

                var decoder = await BitmapDecoder.CreateAsync(stream);

                //using var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                var transform = new BitmapTransform();

                const float sourceImageHeightLimit = 1280;

                if (decoder.PixelHeight > sourceImageHeightLimit)
                {
                    var scalingFactor = (float)sourceImageHeightLimit / (float)decoder.PixelHeight;
                    transform.ScaledWidth = (uint)Math.Floor(decoder.PixelWidth * scalingFactor);
                    transform.ScaledHeight = (uint)Math.Floor(decoder.PixelHeight * scalingFactor);
                }

                using var softwareBitmap = await decoder.GetSoftwareBitmapAsync(decoder.BitmapPixelFormat, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);

                using var unCroppedBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Nv12);

                var faces = await _faceDetector.DetectFacesAsync(unCroppedBitmap);

                if (faces.Count > 0)
                {
                    //crop image to focus on face portion
                    var faceBox = faces[0].FaceBox;

                    using var inputFrame = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);


                   using  var tmp = new VideoFrame(softwareBitmap.BitmapPixelFormat, (int)(faceBox.Width + faceBox.Width % 2) - 2,
                        (int)(faceBox.Height + faceBox.Height % 2) - 2);

                    await inputFrame.CopyToAsync(tmp, new BitmapBounds(faceBox.X - 20, faceBox.Y - 20, faceBox.Width + 40, faceBox.Height + 40), null);

                    if (tmp.SoftwareBitmap is not null)
                    {
                        using IRandomAccessStream faceStream = new InMemoryRandomAccessStream();

                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, faceStream);

                        // Set the software bitmap
                        encoder.SetSoftwareBitmap(tmp.SoftwareBitmap);

                        await encoder.FlushAsync();

                        using var image = new System.Drawing.Bitmap(faceStream.AsStream());

                        using var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                        using var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

                        using var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                        var dataMeta = mat2.Data;

                        var faceData = new byte[240 * 240 * 3];

                        Marshal.Copy(dataMeta, faceData, 0, 240 * 240 * 3);

                        _faceData = faceData;
                    }
                }

                //using var face = await FaceDetectionAsync(softwareBitmap);

                //if (face is not null)
                //{
                //    using IRandomAccessStream faceStream = new InMemoryRandomAccessStream();

                //    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, faceStream);

                //    // Set the software bitmap
                //    encoder.SetSoftwareBitmap(face);

                //    await encoder.FlushAsync();

                //    using var image = new System.Drawing.Bitmap(faceStream.AsStream());

                //    using var mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);

                //    using var mat1 = mat.Resize(new OpenCvSharp.Size(240, 240), 0, 0, OpenCvSharp.InterpolationFlags.Area);

                //    using var mat2 = mat1.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                //    var dataMeta = mat2.Data;

                //    var faceData = new byte[240 * 240 * 3];

                //    Marshal.Copy(dataMeta, faceData, 0, 240 * 240 * 3);

                //    _faceData = faceData;
                //}

                using var imagePose = new System.Drawing.Bitmap(stream.AsStream());

                using var matPoseData = OpenCvSharp.Extensions.BitmapConverter.ToMat(imagePose);

                using var matPose2 = matPoseData.CvtColor(OpenCvSharp.ColorConversionCodes.BGR2RGB);

                var dataMetaPose = matPose2.Data;

                var length = matPose2.Width * matPose2.Height * matPose2.Channels();

                var data = new byte[length];

                Marshal.Copy(dataMetaPose, data, 0, length);

                var widthStep = (int)matPose2.Step();

                using var imgFrame = new ImageFrame(ImageFormat.Types.Format.Srgb, matPose2.Width, matPose2.Height, widthStep, data);

                var handsOutput = calculator!.Compute(imgFrame);

                if (handsOutput.PoseLandmarks != null)
                {
                    CameraFrameService.Current.NotifyPosePredictResult(handsOutput);
                }
                else
                {
                    Debug.WriteLine("No hand landmarks");
                }

                //_ = _poseRecognitionService.PosePredictResultUnUseQueueAsync(calculator, null, stream.AsStream());
            }
        }

        // This is the device-lost convention for Win2D.
        catch (Exception e) when (_canvasDevice.IsDeviceLost(e.HResult))
        {
            // We lost our graphics device. Recreate it and reset
            // our Direct3D11CaptureFramePool.  
            needsReset = true;
            recreateDevice = true;
        }

        if (needsReset)
        {
            ResetFramePool(frame.ContentSize, recreateDevice);
        }
    }

    private void FillSurfaceWithBitmap(CanvasBitmap canvasBitmap)
    {
        CanvasComposition.Resize(_surface, canvasBitmap.Size);

        using (var session = CanvasComposition.CreateDrawingSession(_surface))
        {
            session.Clear(Colors.Transparent);
            session.DrawImage(canvasBitmap);
        }
    }

    private void ResetFramePool(SizeInt32 size, bool recreateDevice)
    {
        do
        {
            try
            {
                if (recreateDevice)
                {
                    _canvasDevice = new CanvasDevice();
                }

                _framePool.Recreate(
                    _canvasDevice,
                    Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    2,
                    size);
            }
            // This is the device-lost convention for Win2D.
            catch (Exception e) when (_canvasDevice.IsDeviceLost(e.HResult))
            {
                _canvasDevice = null;
                recreateDevice = true;
            }
        } while (_canvasDevice == null);
    }
}
