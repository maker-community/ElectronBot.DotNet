using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using ElectronBot.Braincase.Services;
using Microsoft.Graph;
using Microsoft.UI.Xaml;
using Verdure.ElectronBot.Core.Contracts.Services;

namespace ViewModels;
public partial class Hw75DynamicViewModel : ObservableRecipient
{

    private readonly IMicrosoftGraphService _microsoftGraphService;

    private readonly IdentityService _identityService;

    private readonly DispatcherTimer _dispatcherTimer = new();

    private const string ViewName = "TodoView";

    [ObservableProperty]
    private ObservableCollection<TodoTaskList>? _todoTaskLists;
    public Hw75DynamicViewModel(IMicrosoftGraphService microsoftGraphService, IdentityService identityService)
    {
        _microsoftGraphService = microsoftGraphService;
        _identityService = identityService;
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 5);

        _dispatcherTimer.Tick += DispatcherTimer_Tick;

        _dispatcherTimer.Start();
    }

    private async void DispatcherTimer_Tick(object? sender, object e)
    {
        if (_identityService.IsLoggedIn())
        {
            await _microsoftGraphService.PrepareGraphAsync();

            var todos = await _microsoftGraphService.GetTodoTaskListAsync();

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
        var todos = await _microsoftGraphService.GetTodoTaskListAsync();

        TodoTaskLists = new ObservableCollection<TodoTaskList>(todos);
    }

}

