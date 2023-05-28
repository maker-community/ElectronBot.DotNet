using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Services;
using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Services;
using Microsoft.Graph;
using Controls.CompactOverlay;
using Microsoft.UI.Windowing;

namespace ElectronBot.Braincase.ViewModels;

public partial class TodoViewModel : ObservableRecipient
{
    private ICommand _loadedCommand;
    public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

    private readonly IMicrosoftGraphService _microsoftGraphService;

    private readonly IdentityService _identityService;

    private bool _isLogin = false;

    private ObservableCollection<TodoTaskList> _todoTaskLists = new();
    public TodoViewModel(IMicrosoftGraphService microsoftGraphService,
        IdentityService identityService)
    {
        _microsoftGraphService = microsoftGraphService;
        _identityService = identityService;
    }

    public ObservableCollection<TodoTaskList> TodoTaskLists
    {
        get => _todoTaskLists;
        set => SetProperty(ref _todoTaskLists, value);
    }

    private async void OnLoaded()
    {
        if (_identityService.IsLoggedIn())
        {
            await _microsoftGraphService.PrepareGraphAsync();

            var todos =  await _microsoftGraphService.GetTodoTaskListAsync();
            
            TodoTaskLists = new ObservableCollection<TodoTaskList>(todos);
        }
        else
        {
            _identityService.LoggedIn += IdentityService_LoggedIn;
            await _identityService.LoginAsync();
        }
    }

    private async void IdentityService_LoggedIn(object? sender, EventArgs e)
    {
        _isLogin = true;

        var todos = await _microsoftGraphService.GetTodoTaskListAsync();

        TodoTaskLists = new ObservableCollection<TodoTaskList>(todos);
    }

    [RelayCommand]
    public void CompactOverlay()
    {
        WindowEx compactOverlay = new CompactOverlayWindow();

        compactOverlay.Content = new DefaultCompactOverlayPage();

        var appWindow = compactOverlay.AppWindow;

        appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

        appWindow.Show();

        App.MainWindow.Hide();
    }
}
