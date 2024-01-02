using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls.CompactOverlay;
using ElectronBot.Braincase.Contracts.Services;
using Verdure.ElectronBot.Core.Helpers;
using Verdure.ElectronBot.Core.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Services;
using ElectronBot.Braincase.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Services;
using Microsoft.UI.Windowing;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75ShellViewModel : ObservableRecipient
{
    private bool _isBackEnabled;
    private object? _selected;

    private RelayCommand _userProfileCommand;
    private UserViewModel _user;
    private bool _isBusy;
    private bool _isLoggedIn;
    private bool _isAuthorized;

    private readonly IdentityService _identityService;

    private readonly UserDataService _userDataService;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public bool IsBackEnabled
    {
        get => _isBackEnabled;
        set => SetProperty(ref _isBackEnabled, value);
    }

    public object? Selected
    {
        get => _selected;
        set => SetProperty(ref _selected, value);
    }

    public Hw75ShellViewModel(INavigationService navigationService,
        INavigationViewService navigationViewService,
        IdentityService identityService,
        UserDataService userDataService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        _identityService = identityService;
        _userDataService = userDataService;
    }

    public RelayCommand UserProfileCommand => _userProfileCommand ?? (_userProfileCommand = new RelayCommand(OnUserProfile, () => !IsBusy));

    [RelayCommand]
    private void ShowOrHideWindow()
    {
        if (App.MainWindow.Visible)
        {
            App.MainWindow.Hide();
        }
        else
        {
            App.MainWindow.Show();
        }
    }

    [RelayCommand]
    private void Settings()
    {

    }
    [RelayCommand]
    private async void Exit()
    {
        try
        {
            ElectronBotHelper.Instance.InvokeClockCanvasStop();

            var service = App.GetService<EmoticonActionFrameService>();

            service.ClearQueue();

            Thread.Sleep(1000);

            IntelligenceService.Current.CleanUp();

            await CameraService.Current.CleanupMediaCaptureAsync();

            await CameraFrameService.Current.CleanupMediaCaptureAsync();

            ElectronBotHelper.Instance?.ElectronBot?.Disconnect();
        }
        catch (Exception)
        {

        }

        App.MainWindow.Close();
    }
    public UserViewModel User
    {
        get => _user;
        set => SetProperty(ref _user, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            SetProperty(ref _isBusy, value);
            UserProfileCommand.NotifyCanExecuteChanged();
        }
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    public bool IsAuthorized
    {
        get => _isAuthorized;
        set => SetProperty(ref _isAuthorized, value);
    }


    public async void Initialize()
    {
        _identityService.LoggedIn += OnLoggedIn;
        _identityService.LoggedOut += OnLoggedOut;
        _userDataService.UserDataUpdated += OnUserDataUpdated;
        IsLoggedIn = _identityService.IsLoggedIn();
        IsAuthorized = IsLoggedIn && _identityService.IsAuthorized();
        User = await _userDataService.GetUserAsync();
    }

    private void OnUserDataUpdated(object sender, UserViewModel userData)
    {
        User = userData;
    }

    private void OnLoggedIn(object sender, EventArgs e)
    {
        IsLoggedIn = true;
        IsAuthorized = IsLoggedIn && _identityService.IsAuthorized();
        IsBusy = false;
    }

    private void OnLoggedOut(object sender, EventArgs e)
    {
        User = null;
        IsLoggedIn = false;
        IsAuthorized = false;
        CleanRestrictedPagesFromNavigationHistory();
        GoBackToLastUnrestrictedPage();
    }

    private void CleanRestrictedPagesFromNavigationHistory()
    {
        NavigationService.Frame.BackStack
        .Where(b => Attribute.IsDefined(b.SourcePageType, typeof(Restricted)))
        .ToList()
        .ForEach(page => NavigationService.Frame.BackStack.Remove(page));
    }

    private void GoBackToLastUnrestrictedPage()
    {
        var currentPage = NavigationService.Frame.Content as Page;
        var isCurrentPageRestricted = Attribute.IsDefined(currentPage.GetType(), typeof(Restricted));
        if (isCurrentPageRestricted)
        {
            NavigationService.GoBack();
        }
    }

    private async void OnUserProfile()
    {
        if (IsLoggedIn)
        {
            NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);
        }
        else
        {
            IsBusy = true;
            var loginResult = await _identityService.LoginAsync();
            if (loginResult != LoginResultType.Success)
            {
                await AuthenticationHelper.ShowLoginErrorAsync(loginResult);
                IsBusy = false;
            }
        }
    }
    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;
            return;
        }

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }
}
