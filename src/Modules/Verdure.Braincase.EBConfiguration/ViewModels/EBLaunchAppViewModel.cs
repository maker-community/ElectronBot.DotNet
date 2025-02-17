using Windows.ApplicationModel;

namespace Verdure.Braincase.EBConfiguration.ViewModels;
public partial class EBLaunchAppViewModel : ObservableRecipient
{

    [ObservableProperty] private ObservableCollection<Package> _appPackages;

    [ObservableProperty] private string _win32Path;

    [ObservableProperty] private string _VoiceText;

    [ObservableProperty] private string _appNameText;

    [ObservableProperty] private bool _IsMsix = true;

    [ObservableProperty] private Package? _selectPackage;

    [ObservableProperty] private Visibility _win32PathVisibility = Visibility.Collapsed;

    [ObservableProperty] private Visibility _msixVisibility = Visibility.Visible;

}
