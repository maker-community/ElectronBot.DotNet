using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.WinUI;
using SharpDX;
using Windows.ApplicationModel;

namespace ElectronBot.Braincase.Helpers;
public class Bot3DHelper
{

    private static Bot3DHelper? _instance;
    public static Bot3DHelper Instance => _instance ??= new Bot3DHelper();


    private readonly Importer _importer = new();

    private readonly DiffuseMaterial _pinkModelMaterial = new()
    {
        Name = "Pink",
        DiffuseColor = Color.LightPink// DiffuseMaterials.ToColor(255, 192, 203, 1.0),
    };


    public Bot3DHelper()
    {
        EffectsManager = App.GetService<IEffectsManager>();

        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("eyes-closed.png")
        };


        var filePath = Package.Current.InstalledLocation.Path + "\\Assets\\Cubemap_Grandcanyon.dds";

        EnvironmentMap = LoadTextureByFullPath(filePath);
    }

    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };


    public Matrix BodyMt { get; set; } = default;

    public Matrix LeftArmMt { get; set; } = default;

    public Matrix RightArmMt { get; set; } = default;

    public Matrix HeadMt { get; set; } = default;

    public Matrix BaseMt { get; set; } = default;


    public Vector3 ModelCentroidPoint { get; set; } = default;


    public Vector3 HeadModelCentroidPoint { get; set; } = default;


    public BoundingBox RightShoulderBoundingBox { get; set; } = default;


    public BoundingBox LeftShoulderBoundingBox { get; set; } = default;

    public BoundingBox BodyBoundingBox { get; set; } = default;


    public BoundingBox HeadBoundingBox { get; set; } = default;


    public BoundingBox BaseBoundingBox { get; set; } = default;

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

    public TextureModel EnvironmentMap
    {
        private set;
        get;
    }

    public Task LoadModelFileAsync()
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

                    HeadMt = HeadModel.HxTransform3D;

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

                    BodyMt = BodyModel.HxTransform3D;

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

                    RightArmMt = RightArmModel.HxTransform3D;

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

                    LeftArmMt = LeftArmModel.HxTransform3D;

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

                    BaseMt = BaseModel.HxTransform3D;

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
        }
        catch (Exception)
        {
            ToastHelper.SendToast("模型加载失败", TimeSpan.FromSeconds(3));
        }

        return Task.CompletedTask;
    }

    public Task ClearModelFileAsync()
    {
        HeadModel.Dispose();
        BodyModel.Dispose();
        RightArmModel.Dispose();
        LeftArmModel.Dispose();
        BaseModel.Dispose();
        EffectsManager.Dispose();
        _importer.Dispose();

        return Task.CompletedTask;
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

    private TextureModel LoadTexture(string file)
    {
        var filePath = Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\Pic\\{file}";

        return TextureModel.Create(filePath);
    }

    private TextureModel LoadTextureByFullPath(string filePath)
    {
        return TextureModel.Create(filePath);
    }
}
