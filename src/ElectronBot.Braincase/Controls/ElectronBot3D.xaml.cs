using ElectronBot.Braincase;
using ElectronBot.Braincase.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Controls;
public sealed partial class ElectronBot3D : UserControl
{

    public ElectronBot3DViewModel ViewModel
    {
        get;
    }


    public ElectronBot3D()
    {
        this.InitializeComponent();
        ViewModel = App.GetService<ElectronBot3DViewModel>();
    }

    private async void UserControl_Loading(Microsoft.UI.Xaml.FrameworkElement sender, object args)
    {
        await ViewModel.Loaded();
    }


    private void UserControl_Unload(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.UnLoaded();
    }


    //public Camera Camera
    //{
    //    get
    //    {
    //        return (Camera)GetValue(CameraProperty);
    //    }
    //    set
    //    {
    //        SetValue(CameraProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for Camera.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty CameraProperty =
    //    DependencyProperty.Register("Camera", typeof(Camera), typeof(ElectronBot3D), new PropertyMetadata(null));




    //public IEffectsManager EffectsManager
    //{
    //    get
    //    {
    //        return (IEffectsManager)GetValue(EffectsManagerProperty);
    //    }
    //    set
    //    {
    //        SetValue(EffectsManagerProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for EffectsManager.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty EffectsManagerProperty =
    //    DependencyProperty.Register("EffectsManager", typeof(IEffectsManager), typeof(ElectronBot3D), new PropertyMetadata(null));




    //public Vector3 ModelCentroidPoint
    //{
    //    get
    //    {
    //        return (Vector3)GetValue(ModelCentroidPointProperty);
    //    }
    //    set
    //    {
    //        SetValue(ModelCentroidPointProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for ModelCentroidPoint.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty ModelCentroidPointProperty =
    //    DependencyProperty.Register("ModelCentroidPoint", typeof(Vector3), typeof(ElectronBot3D), new PropertyMetadata(default));




    //public SceneNodeGroupModel3D BaseModel
    //{
    //    get
    //    {
    //        return (SceneNodeGroupModel3D)GetValue(BaseModelProperty);
    //    }
    //    set
    //    {
    //        SetValue(BaseModelProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for BaseModel.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty BaseModelProperty =
    //    DependencyProperty.Register("BaseModel", typeof(SceneNodeGroupModel3D), typeof(ElectronBot3D), new PropertyMetadata(null));




    //public SceneNodeGroupModel3D HeadModel
    //{
    //    get
    //    {
    //        return (SceneNodeGroupModel3D)GetValue(HeadModelProperty);
    //    }
    //    set
    //    {
    //        SetValue(HeadModelProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for HeadModel.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty HeadModelProperty =
    //    DependencyProperty.Register("HeadModel", typeof(SceneNodeGroupModel3D), typeof(ElectronBot3D), new PropertyMetadata(null));




    //public SceneNodeGroupModel3D RightArmModel
    //{
    //    get
    //    {
    //        return (SceneNodeGroupModel3D)GetValue(RightArmModelProperty);
    //    }
    //    set
    //    {
    //        SetValue(RightArmModelProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for RightArmModel.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty RightArmModelProperty =
    //    DependencyProperty.Register("RightArmModel", typeof(SceneNodeGroupModel3D), typeof(ElectronBot3D), new PropertyMetadata(null));




    //public SceneNodeGroupModel3D LeftArmModel
    //{
    //    get
    //    {
    //        return (SceneNodeGroupModel3D)GetValue(LeftArmModelProperty);
    //    }
    //    set
    //    {
    //        SetValue(LeftArmModelProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for LeftArmModel.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty LeftArmModelProperty =
    //    DependencyProperty.Register("LeftArmModel", typeof(SceneNodeGroupModel3D), typeof(ElectronBot3D), new PropertyMetadata(null));



    //public SceneNodeGroupModel3D BodyModel
    //{
    //    get
    //    {
    //        return (SceneNodeGroupModel3D)GetValue(BodyModelProperty);
    //    }
    //    set
    //    {
    //        SetValue(BodyModelProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for BodyModel.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty BodyModelProperty =
    //    DependencyProperty.Register("BodyModel", typeof(SceneNodeGroupModel3D), typeof(ElectronBot3D), new PropertyMetadata(null));




    //public TextureModel EnvironmentMap
    //{
    //    get
    //    {
    //        return (TextureModel)GetValue(EnvironmentMapProperty);
    //    }
    //    set
    //    {
    //        SetValue(EnvironmentMapProperty, value);
    //    }
    //}

    //// Using a DependencyProperty as the backing store for EnvironmentMap.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty EnvironmentMapProperty =
    //    DependencyProperty.Register("EnvironmentMap", typeof(TextureModel), typeof(ElectronBot3D), new PropertyMetadata(null));


}
