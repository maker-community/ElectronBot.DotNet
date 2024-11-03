using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Verdure.Braincase.WinUI.Common.Contracts.Services;

namespace Verdure.Braincase.WinUI.Common.Services.Picker;

public partial class ObjectPicker<T> : ContentControl
{
    private readonly INavigationService _navigationService;

    private Popup? _popup;
    private Grid? _rootGrid;
    private TaskCompletionSource<PickResult<T>>? _taskSource;
    public ObjectPicker(INavigationService navigationService)
    {
        _navigationService = navigationService;

        Content = _navigationService.Frame = new Frame();

        HookUpEvents();
    }

    private IObjectPicker<T>? ViewModel;

    public PickerOpenOption PickerOpenOption { get; set; } = new PickerOpenOption();

    public async Task<PickResult<T>> PickSingleObjectAsync(Type sourcePageType, object parameter = null)
    {
        _taskSource = new TaskCompletionSource<PickResult<T>>();
        HorizontalContentAlignment = PickerOpenOption.HorizontalAlignment;
        VerticalContentAlignment = PickerOpenOption.VerticalAlignment;
        Margin = PickerOpenOption.Margin;

        _rootGrid = new Grid
        {
            Background = PickerOpenOption.Background,
            Width = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Width,
            Height = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Height,
            ChildrenTransitions = PickerOpenOption.Transitions
        };

        if (PickerOpenOption.EnableTapBlackAreaExit) _rootGrid.Tapped += RootGrid_Tapped;

        _popup = new Popup
        {
            XamlRoot = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Content.XamlRoot
        };

        _popup.Child = _rootGrid;

        _rootGrid.Children.Add(this);

        _navigationService.NavigateTo(sourcePageType.FullName!, parameter);

        ViewModel = (_navigationService?.Frame?.Content as Page)?.DataContext as IObjectPicker<T>;

        HookUpViewModelEvents();
        _popup.IsOpen = true;
        await FocusPicker();
        var result = await _taskSource.Task;
        Close();

        _navigationService!.Frame = Ioc.Default.GetRequiredService<ICompositorProvider>().GetRootFrame();

        return result;
    }

    private async Task FocusPicker()
    {
        var element = FocusManager.FindFirstFocusableElement(_rootGrid);
        if (element != null)
            await FocusManager.TryFocusAsync(element, FocusState.Programmatic);
    }

    private void Close()
    {
        if (_rootGrid != null)
        {
            _rootGrid.Tapped -= RootGrid_Tapped;
            _rootGrid.Children.Remove(this);
        }
    }

    #region HookEvent

    private void HookUpEvents()
    {
        Unloaded += ObjectPicker_Unloaded;
        _navigationService.Navigated += Frame_Navigated;
        Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().SizeChanged += Window_SizeChanged;
    }

    private void UnhookEvents()
    {
        Unloaded -= ObjectPicker_Unloaded;
        _navigationService.Navigated -= Frame_Navigated;
        Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().SizeChanged -= Window_SizeChanged;
    }

    private void HookUpViewModelEvents()
    {
        if (ViewModel != null)
        {
            ViewModel.ObjectPicked += ViewModel_ObjectPicked;
            ViewModel.Canceled += ViewModel_Canceled;
        }
    }

    private void UnhookViewModelEvents()
    {
        if (ViewModel != null)
        {
            ViewModel.ObjectPicked -= ViewModel_ObjectPicked;
            ViewModel.Canceled -= ViewModel_Canceled;
        }
    }

    #endregion

    #region PickerEvent handlers

    private void ObjectPicker_Unloaded(object sender, RoutedEventArgs e)
    {
        UnhookViewModelEvents();
        UnhookEvents();
        _popup.IsOpen = false;
    }

    private void Frame_Navigated(object sender, NavigationEventArgs e)
    {
        HookUpViewModelEvents();
    }

    private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
    {
        if (_rootGrid != null)
        {
            _rootGrid.Width = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Width;
            _rootGrid.Height = Ioc.Default.GetRequiredService<ICompositorProvider>().GetWindow().Bounds.Height;
        }
    }

    private void RootGrid_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (e.OriginalSource == _rootGrid)
            _taskSource.SetResult(new PickResult<T>
            {
                Canceled = true
            });
    }

    #endregion

    #region ViewModelEvent handlers

    private void ViewModel_ObjectPicked(object sender, ObjectPickedEventArgs<T> e)
    {
        _taskSource.SetResult(new PickResult<T>
        {
            Result = e.Result
        });
    }

    private void ViewModel_Canceled(object sender, EventArgs e)
    {
        _taskSource.SetResult(new PickResult<T>
        {
            Canceled = true
        });
    }

    #endregion
}