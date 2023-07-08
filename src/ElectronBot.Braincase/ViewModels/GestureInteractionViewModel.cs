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
using Windows.Graphics.Imaging;
using Mediapipe.Net.Solutions;
using Microsoft.UI;
using Constants = ElectronBot.Braincase.Constants;

namespace ElectronBot.Braincase.ViewModels;
public partial class GestureInteractionViewModel : ObservableRecipient
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

    [ObservableProperty]
    private Image _faceImage = new();

    private static HandsCpuSolution? calculator;
    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    public GestureInteractionViewModel(IEffectsManager effectsManager)
    {
        EffectsManager = effectsManager;

        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("eyes-closed.png")
        };

        calculator = new HandsCpuSolution();

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
    private string _resultLabel;

    [ObservableProperty] private Stack<string> _menuStack = new();

    private int _firstMenuIndex = 0;

    private int _secondMenuIndex = 0;

    private bool _isFirstMenu = true;

    [ObservableProperty]
    private List<string> _firstMenu = new()
    {
        "ClockMode",
        "EmojisMode",
    };

    [ObservableProperty]
    private Dictionary<string, List<string>> _secondMenu = new()
    {
        {"ClockMode", new List<string>()
        {
            "DefautView",
            "LongShadowView",
            "GooeyFooter",
            "GradientsWithBlend",
            "CustomView",
        } },
        { "EmojisMode", new List<string>()
        {
            "anger",
            "disdain",
            "excited",
            "fear",
            "sad",
        } },
    };

    [ObservableProperty] private string _firstMenuSelected;

    [ObservableProperty] private string _secondMenuSelected;

    private readonly string _modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\MLModel1.zip";

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

                ElectronBotHelper.Instance.ModelActionFrame += Instance_ModelActionFrame;

                await InitAsync();
            }
            catch (Exception)
            {
                ToastHelper.SendToast("模型加载失败", TimeSpan.FromSeconds(3));
            }
        });
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

        CameraBackground = new SolidColorBrush(Colors.Green);
    }

    private void Current_SoftwareBitmapFrameHandPredictResult(object? sender, string e)
    {

        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            ResultLabel = e;

            if (e == "up")
            {
                if (!ElectronBotHelper.Instance.UpLock)
                {
                    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                    if (!playEmojisLock)
                    {
                        //根据表情ID播放表情
                        ElectronBotHelper.Instance.ToPlayEmojisByNameId("excited");
                        ElectronBotHelper.Instance.PlayEmojisLock = true;
                    }

                    ElectronBotHelper.Instance.UpLock = true;
                    ElectronBotHelper.Instance.DownLock = false;
                    ElectronBotHelper.Instance.BackLock = false;
                    ElectronBotHelper.Instance.ForwardLock = false;
                    ElectronBotHelper.Instance.LandLock = false;
                    ElectronBotHelper.Instance.StopLock = false;
                    ElectronBotHelper.Instance.LeftLock = false;
                    ElectronBotHelper.Instance.RightLock = false;
                    ElectronBotHelper.Instance.FingerHeartLock = false;
                    ElectronBotHelper.Instance.ThirdFingerLock = false;
                }
            }
            else if (e == "down")
            {
                if (!ElectronBotHelper.Instance.DownLock)
                {
                    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                    if (!playEmojisLock)
                    {
                        //根据表情ID播放表情
                        ElectronBotHelper.Instance.ToPlayEmojisByNameId("disdain");
                        ElectronBotHelper.Instance.PlayEmojisLock = true;
                    }
                    ElectronBotHelper.Instance.UpLock = false;
                    ElectronBotHelper.Instance.DownLock = true;
                    ElectronBotHelper.Instance.BackLock = false;
                    ElectronBotHelper.Instance.ForwardLock = false;
                    ElectronBotHelper.Instance.LandLock = false;
                    ElectronBotHelper.Instance.StopLock = false;
                    ElectronBotHelper.Instance.LeftLock = false;
                    ElectronBotHelper.Instance.RightLock = false;
                    ElectronBotHelper.Instance.FingerHeartLock = false;
                    ElectronBotHelper.Instance.ThirdFingerLock = false;
                }
            }
            else if (e == "back")
            {
                if (!ElectronBotHelper.Instance.BackLock)
                {
                    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                    if (!playEmojisLock)
                    {
                        //根据表情ID播放表情
                        ElectronBotHelper.Instance.ToPlayEmojisByNameId("normal");
                        ElectronBotHelper.Instance.PlayEmojisLock = true;
                    }
                    ElectronBotHelper.Instance.UpLock = false;
                    ElectronBotHelper.Instance.DownLock = false;
                    ElectronBotHelper.Instance.BackLock = true;
                    ElectronBotHelper.Instance.ForwardLock = false;
                    ElectronBotHelper.Instance.LandLock = false;
                    ElectronBotHelper.Instance.StopLock = false;
                    ElectronBotHelper.Instance.LeftLock = false;
                    ElectronBotHelper.Instance.RightLock = false;
                    ElectronBotHelper.Instance.FingerHeartLock = false;
                    ElectronBotHelper.Instance.ThirdFingerLock = false;
                }
            }
            else if (e == "forward")
            {
                if (!ElectronBotHelper.Instance.ForwardLock)
                {
                    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                    if (!playEmojisLock)
                    {
                        //根据表情ID播放表情
                        ElectronBotHelper.Instance.ToPlayEmojisByNameId("anger");
                        ElectronBotHelper.Instance.PlayEmojisLock = true;
                    }

                    ElectronBotHelper.Instance.UpLock = false;
                    ElectronBotHelper.Instance.DownLock = false;
                    ElectronBotHelper.Instance.BackLock = false;
                    ElectronBotHelper.Instance.ForwardLock = true;
                    ElectronBotHelper.Instance.LandLock = false;
                    ElectronBotHelper.Instance.StopLock = false;
                    ElectronBotHelper.Instance.LeftLock = false;
                    ElectronBotHelper.Instance.RightLock = false;
                    ElectronBotHelper.Instance.FingerHeartLock = false;
                    ElectronBotHelper.Instance.ThirdFingerLock = false;
                }

            }
            else if (e == "land")
            {
                if (!ElectronBotHelper.Instance.LandLock)
                {
                    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                    if (!playEmojisLock)
                    {
                        //根据表情ID播放表情
                        ElectronBotHelper.Instance.ToPlayEmojisByNameId("sad");
                        ElectronBotHelper.Instance.PlayEmojisLock = true;
                    }
                    ElectronBotHelper.Instance.UpLock = false;
                    ElectronBotHelper.Instance.DownLock = false;
                    ElectronBotHelper.Instance.BackLock = false;
                    ElectronBotHelper.Instance.ForwardLock = false;
                    ElectronBotHelper.Instance.LandLock = true;
                    ElectronBotHelper.Instance.StopLock = false;
                    ElectronBotHelper.Instance.LeftLock = false;
                    ElectronBotHelper.Instance.RightLock = false;
                    ElectronBotHelper.Instance.FingerHeartLock = false;
                    ElectronBotHelper.Instance.ThirdFingerLock = false;
                }

            }
            else if (e == "stop")
            {
                //if (!ElectronBotHelper.Instance.StopLock)
                //{
                //    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                //    if (!playEmojisLock)
                //    {
                //        //根据表情ID播放表情
                //        ElectronBotHelper.Instance.ToPlayEmojisByNameId("normal");
                //        ElectronBotHelper.Instance.PlayEmojisLock = true;
                //    }
                //    ElectronBotHelper.Instance.UpLock = false;
                //    ElectronBotHelper.Instance.DownLock = false;
                //    ElectronBotHelper.Instance.BackLock = false;
                //    ElectronBotHelper.Instance.ForwardLock = false;
                //    ElectronBotHelper.Instance.LandLock = false;
                //    ElectronBotHelper.Instance.StopLock = true;
                //    ElectronBotHelper.Instance.LeftLock = false;
                //    ElectronBotHelper.Instance.RightLock = false;
                //    ElectronBotHelper.Instance.FingerHeartLock = false;
                //    ElectronBotHelper.Instance.ThirdFingerLock = false;
                //}

            }
            else if (e == "left")
            {
                if (!ElectronBotHelper.Instance.LeftLock)
                {
                    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                    if (!playEmojisLock)
                    {
                        //根据表情ID播放表情
                        ElectronBotHelper.Instance.ToPlayEmojisByNameId("left");
                        ElectronBotHelper.Instance.PlayEmojisLock = true;
                    }
                    ElectronBotHelper.Instance.UpLock = false;
                    ElectronBotHelper.Instance.DownLock = false;
                    ElectronBotHelper.Instance.BackLock = false;
                    ElectronBotHelper.Instance.ForwardLock = false;
                    ElectronBotHelper.Instance.LandLock = false;
                    ElectronBotHelper.Instance.StopLock = false;
                    ElectronBotHelper.Instance.LeftLock = true;
                    ElectronBotHelper.Instance.RightLock = false;
                    ElectronBotHelper.Instance.FingerHeartLock = false;
                    ElectronBotHelper.Instance.ThirdFingerLock = false;
                }

            }
            else if (e == "right")
            {
                if (!ElectronBotHelper.Instance.RightLock)
                {
                    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                    if (!playEmojisLock)
                    {
                        //根据表情ID播放表情
                        ElectronBotHelper.Instance.ToPlayEmojisByNameId("right");
                        ElectronBotHelper.Instance.PlayEmojisLock = true;
                    }
                    ElectronBotHelper.Instance.UpLock = false;
                    ElectronBotHelper.Instance.DownLock = false;
                    ElectronBotHelper.Instance.BackLock = false;
                    ElectronBotHelper.Instance.ForwardLock = false;
                    ElectronBotHelper.Instance.LandLock = false;
                    ElectronBotHelper.Instance.StopLock = false;
                    ElectronBotHelper.Instance.LeftLock = false;
                    ElectronBotHelper.Instance.RightLock = true;
                    ElectronBotHelper.Instance.FingerHeartLock = false;
                    ElectronBotHelper.Instance.ThirdFingerLock = false;
                }

            }
            else if (e == "finger-heart")
            {
                //if (!ElectronBotHelper.Instance.FingerHeartLock)
                //{
                //    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                //    if (!playEmojisLock)
                //    {
                //        //根据表情ID播放表情
                //        ElectronBotHelper.Instance.ToPlayEmojisByNameId("normal");
                //        ElectronBotHelper.Instance.PlayEmojisLock = true;
                //    }
                //    ElectronBotHelper.Instance.UpLock = false;
                //    ElectronBotHelper.Instance.DownLock = false;
                //    ElectronBotHelper.Instance.BackLock = false;
                //    ElectronBotHelper.Instance.ForwardLock = false;
                //    ElectronBotHelper.Instance.LandLock = false;
                //    ElectronBotHelper.Instance.StopLock = false;
                //    ElectronBotHelper.Instance.LeftLock = false;
                //    ElectronBotHelper.Instance.RightLock = false;
                //    ElectronBotHelper.Instance.FingerHeartLock = true;
                //    ElectronBotHelper.Instance.ThirdFingerLock = false;
                //}

            }
            else if (e == "third-finger")
            {
                //if (!ElectronBotHelper.Instance.FingerHeartLock)
                //{
                //    var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

                //    if (!playEmojisLock)
                //    {
                //        //根据表情ID播放表情
                //        ElectronBotHelper.Instance.ToPlayEmojisByNameId("normal");
                //        ElectronBotHelper.Instance.PlayEmojisLock = true;
                //    }
                //    ElectronBotHelper.Instance.UpLock = false;
                //    ElectronBotHelper.Instance.DownLock = false;
                //    ElectronBotHelper.Instance.BackLock = false;
                //    ElectronBotHelper.Instance.ForwardLock = false;
                //    ElectronBotHelper.Instance.LandLock = false;
                //    ElectronBotHelper.Instance.StopLock = false;
                //    ElectronBotHelper.Instance.LeftLock = false;
                //    ElectronBotHelper.Instance.RightLock = false;
                //    ElectronBotHelper.Instance.FingerHeartLock = false;
                //    ElectronBotHelper.Instance.ThirdFingerLock = true;
                //}

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

            _ = service.HandPredictResultUnUseQueueAsync(calculator, _modelPath, e.SoftwareBitmap);
        }
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

        CameraFrameService.Current.SoftwareBitmapFrameHandPredictResult -= Current_SoftwareBitmapFrameHandPredictResult;
        var service = App.GetService<EmoticonActionFrameService>();
        service.ClearQueue();
        await CleanUpAsync();
    }

    private void Instance_ModelActionFrame(object? sender, Verdure.ElectronBot.Core.Models.ModelActionFrame e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            BodyModel.HxTransform3D = _bodyMt * Matrix.RotationY(MathUtil.DegreesToRadians((e.J6)));

            Material = new DiffuseMaterial()
            {
                EnableUnLit = false,
                DiffuseMap = LoadTextureByStream(e.FrameStream)
            };


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
}