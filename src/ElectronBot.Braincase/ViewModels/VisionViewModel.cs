using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.WinUI;
using Services.ElectronBot;
using SharpDX;

namespace ElectronBot.Braincase.ViewModels;

public partial class VisionViewModel : ObservableRecipient, INavigationAware
{
    public VisionViewModel()
    {
        CurrentEmojis._emojis = new EmojiCollection();
    }

    [ObservableProperty]
    private Camera _camera = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    [ObservableProperty] private IEffectsManager _effectsManager;

    [ObservableProperty]
    private Vector3 _modelCentroidPoint = default;

    [ObservableProperty] private TextureModel _environmentMap;

    [ObservableProperty]
    private SceneNodeGroupModel3D _bodyModel;

    [ObservableProperty]
    private SceneNodeGroupModel3D _leftArmModel;

    [ObservableProperty]

    private SceneNodeGroupModel3D _rightArmModel;

    [ObservableProperty]

    private SceneNodeGroupModel3D _headModel;

    [ObservableProperty]
    private SceneNodeGroupModel3D _baseModel;
    public async void OnNavigatedFrom()
    {

        Camera = null;
        EnvironmentMap = null;
        BodyModel = null;
        LeftArmModel = null;
        RightArmModel = null;
        HeadModel = null;
        BaseModel = null;
        EffectsManager = null;
        await VisionService.Current.StopAsync();
    }
    public async void OnNavigatedTo(object parameter)
    {
        Camera = Bot3DHelper.Instance.Camera;
        EnvironmentMap = Bot3DHelper.Instance.EnvironmentMap;
        BodyModel = Bot3DHelper.Instance.BodyModel;
        LeftArmModel = Bot3DHelper.Instance.LeftArmModel;
        RightArmModel = Bot3DHelper.Instance.RightArmModel;
        HeadModel = Bot3DHelper.Instance.HeadModel;
        BaseModel = Bot3DHelper.Instance.BaseModel;
        ModelCentroidPoint = Bot3DHelper.Instance.ModelCentroidPoint;
        EffectsManager = Bot3DHelper.Instance.EffectsManager;
        await VisionService.Current.StartAsync();
    }
}
