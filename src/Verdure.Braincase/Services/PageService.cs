using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.Copilot.ViewModels;
using Verdure.Braincase.Copilot.Views;
using Verdure.Braincase.EBConfiguration.ViewModels;
using Verdure.Braincase.EBConfiguration.Views;
using Verdure.Braincase.ViewModels;
using Verdure.Braincase.Views;
using Verdure.Braincase.WinUI.Common.ViewModels;
using Views;

namespace Verdure.Braincase.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<MainViewModel, MainPage>();
        Configure<HomeViewModel, HomePage>();
        Configure<CameraEmojisViewModel, CameraEmojisPage>();
        Configure<BlankViewModel, BlankPage>();
        Configure<EmojisEditViewModel, EmojisEditPage>();
        Configure<TodoViewModel, TodoPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<ImageCropperPickerViewModel, ImageCropperPage>();
        Configure<GamepadViewModel, GamepadPage>();
        Configure<GestureInteractionViewModel, GestureInteractionPage>();
        Configure<PoseRecognitionViewModel, PoseRecognitionPage>();
        Configure<VisionViewModel, VisionPage>();
        Configure<MovieViewModel, MoviePage>();
        Configure<GestureAppConfigViewModel, GestureAppConfigPage>();
        Configure<Hw75ViewModel, Hw75Page>();

        Configure<AgentViewModel, AgentPage>();
        Configure<EBDebugViewModel, EBDebugPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.Any(p => p.Value == type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
