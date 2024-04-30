using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.Braincase.Helpers;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Services;
using SharpDX;
using Verdure.ElectronBot.Core.Models;
using Windows.ApplicationModel;
using BoundingBox = SharpDX.BoundingBox;
using Camera = HelixToolkit.WinUI.Camera;
using Matrix = SharpDX.Matrix;
using Vector3 = SharpDX.Vector3;

namespace ElectronBot.Braincase.ViewModels;

public partial class ElectronBot3DViewModel : ObservableRecipient
{

    /// <summary>
    /// 姿态数据
    ///</summary>
    [ObservableProperty]
    ImageSource _poseImageSource;

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

    private Dictionary<string, SceneNode> _scenes = new();

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

    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    public ElectronBot3DViewModel(IEffectsManager effectsManager)
    {
        EffectsManager = effectsManager;

        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("eyes-closed.png")
        };

        var filePath = Package.Current.InstalledLocation.Path + "\\Assets\\Cubemap_Grandcanyon.dds";

        EnvironmentMap = LoadTextureByFullPath(filePath);

        CurrentEmojis._emojis = new EmojiCollection();
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
    public async Task Loaded()
    {
        await Task.Run(() =>
        {
            var models = new List<string>()
                {
                    "Body1.obj",
                    "Body2.obj",
                    "Head1.obj",
                    "Head2.obj",
                    "Head3.obj",
                    "LeftArm1.obj",
                    "LeftArm2.obj",
                    "LeftShoulder.obj",
                    "RightArm1.obj",
                    "RightArm2.obj",
                    "RightShoulder.obj",
                    "Base.obj",
                };

            foreach (var modelName in models)
            {
                var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";

                var newScene = _importer.Load(modelPath);

                if (newScene != null && newScene.Root != null)
                {
                    _scenes.Add(modelName, newScene.Root);
                }
            }

            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                foreach (var scene in _scenes)
                {
                    scene.Value.Attach(EffectsManager);
                    scene.Value.UpdateAllTransformMatrix();

                    if (scene.Key.StartsWith("Head"))
                    {
                        HeadModel.AddNode(scene.Value);
                    }
                    else if (scene.Key.StartsWith("Body"))
                    {
                        BodyModel.AddNode(scene.Value);
                    }
                    else if (scene.Key.StartsWith("Left"))
                    {
                        LeftArmModel.AddNode(scene.Value);
                    }
                    else if (scene.Key.StartsWith("Right"))
                    {
                        RightArmModel.AddNode(scene.Value);
                    }
                    else if (scene.Key.StartsWith("Base"))
                    {
                        BaseModel.AddNode(scene.Value);
                    }

                    if (scene.Key == "Head3.obj")
                    {
                        foreach (var node in scene.Value.Traverse())
                        {
                            if (node is MeshNode meshNode)
                            {
                                // this is face
                                meshNode.Material = Material;
                            }
                        }
                    }
                    else if (scene.Key == "Head1.obj")
                    {
                        if (scene.Value.TryGetCentroid(out var centroid))
                        {
                            // Must use UI thread to set value back.
                            HeadModelCentroidPoint = centroid;
                        }

                        foreach (var node in scene.Value.Traverse())
                        {
                            if (node is MeshNode meshNode)
                            {
                                meshNode.Material = _pinkModelMaterial;
                            }
                        }
                    }
                    else if (scene.Key == "Head2.obj" || scene.Key == "LeftArm1.obj" || scene.Key == "RightArm1.obj")
                    {
                        foreach (var node in scene.Value.Traverse())
                        {
                            if (node is MeshNode meshNode)
                            {
                                meshNode.Material = DiffuseMaterials.PureWhite;
                            }
                        }
                    }

                    else if (scene.Key == "Body2.obj")
                    {
                        if (scene.Value.TryGetCentroid(out var centroid))
                        {
                            // Must use UI thread to set value back.
                            ModelCentroidPoint = centroid;
                        }

                        foreach (var node in scene.Value.Traverse())
                        {
                            if (node is MeshNode meshNode)
                            {
                                meshNode.Material = DiffuseMaterials.PureWhite;
                            }
                        }
                    }
                    else if (scene.Key == "Body1.obj" || scene.Key == "RightArm2.obj" || scene.Key == "LeftArm2.obj")
                    {
                        foreach (var node in scene.Value.Traverse())
                        {
                            if (node is MeshNode meshNode)
                            {
                                meshNode.Material = _pinkModelMaterial;
                            }
                        }
                    }
                    // Must use UI thread to set value back.
                    else if (scene.Key == "RightShoulder.obj")
                    {
                        if (scene.Value.TryGetBound(out var bound))
                        {

                            RightShoulderBoundingBox = bound;
                        }

                    }

                    // Must use UI thread to set value back.
                    else if (scene.Key == "LeftShoulder.obj")
                    {
                        if (scene.Value.TryGetBound(out var bound))
                        {

                            LeftShoulderBoundingBox = bound;
                        }

                    }

                    // Must use UI thread to set value back.
                    else if (scene.Key == "Base.obj")
                    {
                        if (scene.Value.TryGetBound(out var bound))
                        {
                            BaseBoundingBox = bound;

                        }

                        foreach (var node in scene.Value.Traverse())
                        {
                            if (node is MeshNode meshNode)
                            {
                                meshNode.Material = DiffuseMaterials.PureWhite;
                            }
                        }

                    }
                }

                _headMt = HeadModel.HxTransform3D;
                _bodyMt = BodyModel.HxTransform3D;
                _rightArmMt = RightArmModel.HxTransform3D;
                _leftArmMt = LeftArmModel.HxTransform3D;
                _baseMt = BaseModel.HxTransform3D;

                ElectronBotHelper.Instance.ModelActionFrame += Instance_ModelActionFrame;
                FocusCameraToScene();
            });
        });
    }
    #endregion

    public void UnLoaded()
    {
        ElectronBotHelper.Instance.ModelActionFrame -= Instance_ModelActionFrame;
        HeadModel.Dispose();
        BodyModel.Dispose();
        RightArmModel.Dispose();
        LeftArmModel.Dispose();
        BaseModel.Dispose();
        EffectsManager.Dispose();
        _importer.Dispose();

        var service = App.GetService<EmoticonActionFrameService>();
        service.ClearQueue();
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
        var maxWidth = Math.Max(Math.Max(RightShoulderBoundingBox.Width, RightShoulderBoundingBox.Height), BodyBoundingBox.Depth) + 200;
        var pos = RightShoulderBoundingBox.Center + new Vector3(0, 0, maxWidth);
        Camera.Position = pos;
        Camera.LookDirection = RightShoulderBoundingBox.Center - pos;
        Camera.UpDirection = Vector3.UnitY;
        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }

    private TextureModel LoadTextureByStream(Stream data)
    {

        return TextureModel.Create(data);
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
}
