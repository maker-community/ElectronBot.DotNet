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
using BoundingBox = SharpDX.BoundingBox;
using Camera = HelixToolkit.WinUI.Camera;

namespace ViewModels;
public partial class ModelLoadCompactOverlayViewModel : ObservableRecipient
{
    private readonly DispatcherTimer DispatcherTimer = new();

    public ModelLoadCompactOverlayViewModel(IEffectsManager effectsManager)
    {
        EffectsManager = effectsManager;
        DispatcherTimer.Interval = TimeSpan.FromMilliseconds(500);

        DispatcherTimer.Tick += ModelAcitonDispatcherTimer_Tick;
        Material = new DiffuseMaterial()
        {
            EnableUnLit = false,
            DiffuseMap = LoadTexture("eyes-closed.png")
        };
    }


    [RelayCommand]
    public async Task Loaded()
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
            "Face.obj",
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
            var importer = new Importer();

            foreach (var modelName in head)
            {
                var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";

                var newScene = importer.Load(modelPath);

                if (newScene != null)
                {
                    /// Pre-attach and calculate all scene info in a separate task.
                    newScene.Root.Attach(EffectsManager);

                    newScene.Root.UpdateAllTransformMatrix();

                    HeadModel.AddNode(newScene.Root);

                    if (modelName == "Head3.obj")
                    {
                        if (newScene != null && newScene.Root != null)
                        {
                            foreach (var node in newScene.Root.Traverse())
                            {
                                if (node is MeshNode meshNode)
                                {
                                    meshNode.Material = Material;
                                }
                            }
                        }
                    }
                }
            }


            foreach (var modelName in body)
            {
                var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";

                var newScene = importer.Load(modelPath);

                if (newScene != null)
                {
                    /// Pre-attach and calculate all scene info in a separate task.
                    newScene.Root.Attach(EffectsManager);
                    newScene.Root.UpdateAllTransformMatrix();

                    BodyModel.AddNode(newScene.Root);

                    if (modelName == "Body2.obj")
                    {
                        if (newScene.Root.TryGetCentroid(out var centroid))
                        {
                            /// Must use UI thread to set value back.
                            ModelCentroidPoint = centroid;
                        }
                    }
                }
            }

            foreach (var modelName in rightArm)
            {
                var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";
                var newScene = importer.Load(modelPath);
                if (newScene != null)
                {
                    /// Pre-attach and calculate all scene info in a separate task.
                    newScene.Root.Attach(EffectsManager);
                    newScene.Root.UpdateAllTransformMatrix();

                    RightArmModel.AddNode(newScene.Root);

                    if (newScene.Root.TryGetBound(out var bound))
                    {
                        /// Must use UI thread to set value back.
                        if (modelName == "RightShoulder.obj")
                        {
                            ModelBoundingBox = bound;
                        }

                    }
                }
            }


            foreach (var modelName in leftArm)
            {
                var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";
                var newScene = importer.Load(modelPath);
                if (newScene != null)
                {
                    /// Pre-attach and calculate all scene info in a separate task.
                    newScene.Root.Attach(EffectsManager);
                    newScene.Root.UpdateAllTransformMatrix();

                    LeftArmModel.AddNode(newScene.Root);

                    if (newScene.Root.TryGetBound(out var bound))
                    {
                        /// Must use UI thread to set value back.
                        if (modelName == "RightShoulder.obj")
                        {
                           // ModelBoundingBox = bound;
                        }

                    }
                }
            }

            foreach (var modelName in baseBody)
            {
                var modelPath = Package.Current.InstalledLocation.Path + $"\\Assets\\ElectronBotModel\\{modelName}";
                var newScene = importer.Load(modelPath);
                if (newScene != null)
                {
                    /// Pre-attach and calculate all scene info in a separate task.
                    newScene.Root.Attach(EffectsManager);
                    newScene.Root.UpdateAllTransformMatrix();

                    BaseModel.AddNode(newScene.Root);
                }
            }

            FocusCameraToScene();
            DispatcherTimer.Start();
        }
        catch (Exception)
        {
            ToastHelper.SendToast("模型加载失败", TimeSpan.FromSeconds(3));
        }
    }

    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(ModelBoundingBox.Width, ModelBoundingBox.Height), ModelBoundingBox.Depth) + 240;
        var pos = ModelBoundingBox.Center + new Vector3(0, 0, maxWidth);
        Camera.Position = pos;
        Camera.LookDirection = ModelBoundingBox.Center - pos;
        Camera.UpDirection = Vector3.UnitY;
        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }

    private void ModelAcitonDispatcherTimer_Tick(object? sender, object e)
    {
        try
        {
            var list = ModelBoundingBox.GetCorners();

            var average = new SharpDX.Vector3(
                (list[1].X + list[5].X) / 2f,
                ((list[1].Y + list[5].Y) / 2f) - 8f,
                (list[1].Z + list[5].Z) / 2f
            );

            var translationMatrix = Matrix.Translation(-average.X, -average.Y, -average.Z);
            var tr2 = RightArmModel.HxTransform3D * translationMatrix;
            //var tr3 = tr2 * Matrix.RotationZ(MathUtil.DegreesToRadians(-(DateTime.Now.Second)));
            var tr4 = tr2 * Matrix.RotationX(MathUtil.DegreesToRadians(-(DateTime.Now.Second)));
            var tr5 = tr4 * Matrix.Translation(average.X, average.Y, average.Z);

            RightArmModel.HxTransform3D = tr5;
        }
        catch
        {

        }

    }

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
    private bool _showWireframe = false;
    [ObservableProperty]
    private BoundingBox _modelBoundingBox = default;

    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    [RelayCommand]
    public void CompactOverlay()
    {
        //WindowEx compactOverlay = new CompactOverlayWindow();

        //compactOverlay.Content = new DefaultCompactOverlayPage();

        //var appWindow = compactOverlay.AppWindow;

        //appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

        //appWindow.Destroy();

        App.MainWindow.Show();
    }

    private TextureModel LoadTexture(string file)
    {
       var filePath =  Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\Pic\\{file}";
        
        return TextureModel.Create(filePath);
    }
}
