namespace Verdure.Braincase.EBConfiguration.ViewModels;
public partial class GamepadActionViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _leftX;

    [ObservableProperty]
    private string _leftY;

    [ObservableProperty]
    private string _rightX;

    [ObservableProperty]
    private string _rightY;

    [ObservableProperty]
    private string _pbLeft;

    [ObservableProperty]
    private string _pbRight;

    [ObservableProperty]
    private double _leftThumbstickX;

    [ObservableProperty]
    private double _leftThumbstickY;

    [ObservableProperty]
    private double _rightThumbstickX;

    [ObservableProperty]
    private double _rightThumbstickY;

    [ObservableProperty]
    private bool isHoldRightThumbstick = false;

    [ObservableProperty]
    private bool isReleaseRightThumbstick = false;

    [ObservableProperty]
    private ObservableCollection<ComboxItemModel> _gamePads = new();
}
