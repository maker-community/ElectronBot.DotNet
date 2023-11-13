using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Controls;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Core.Models;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.UI.Xaml.Controls;
using Models;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Storage;

namespace ElectronBot.Braincase.ViewModels;

public partial class GestureAppConfigViewModel : ObservableRecipient, INavigationAware
{
    private readonly ILocalSettingsService _localSettingsService;

    private ObservableCollection<GestureAppConfig> _gestureAppConfigs = new();
    public ObservableCollection<GestureAppConfig> GestureAppConfigs
    {
        get => _gestureAppConfigs;
        set => SetProperty(ref _gestureAppConfigs, value);
    }

    [ObservableProperty] private ObservableCollection<LaunchAppConfig> _launchApps;


    private readonly PackageManager _packageManager = new();
    public List<string> GestureLabels
    {
        get; set;
    } = new()
    {
        Constants.Land,
        Constants.Up,
        Constants.Down,
        Constants.Right,
        Constants.Left,
        Constants.Stop,
        Constants.Forward,
        Constants.Back,
        Constants.FingerHeart,
        Constants.ThirdFinger,
    };

    [ObservableProperty] private ObservableCollection<Package> _appPackages;
    public GestureAppConfigViewModel(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
        Init();
    }

    public async void Init()
    {
        var gestureAppConfigs = (await _localSettingsService.ReadSettingAsync<List<GestureAppConfig>>
                   (Constants.CustomGestureAppConfigKey)) ?? new List<GestureAppConfig>();

        GestureAppConfigs = new ObservableCollection<GestureAppConfig>(gestureAppConfigs);
    }


    /// <summary>
    /// 保存配置Command
    /// </summary>
    private ICommand _saveConfigCommand;
    public ICommand SaveConfigCommand => _saveConfigCommand ??= new RelayCommand(SaveConfig);

    /// <summary>
    /// 保存配置
    /// </summary>
    public async void SaveConfig()
    {
        await _localSettingsService.SaveSettingAsync<List<GestureAppConfig>>(Constants.CustomGestureAppConfigKey, GestureAppConfigs.ToList());
        Init();
    }


    /// <summary>
    /// 删除配置项Command
    /// </summary>
    private ICommand _delConfigCommand;
    public ICommand DelConfigCommand => _delConfigCommand ??= new RelayCommand<string>(DelConfig);

    /// <summary>
    /// 删除配置项
    /// </summary>
    /// <param name="id"></param>
    public async void DelConfig(string id)
    {
        int index = -1;
        for (int i = 0; i < GestureAppConfigs.Count; i++)
        {
            if (GestureAppConfigs[i].Id == id)
            {
                index = i; break;
            }
        }

        if (index >= 0)
        {
            GestureAppConfigs.RemoveAt(index);
        }
    }


    /// <summary>
    /// 添加配置项Command
    /// </summary>
    private ICommand _addConfigCommand;
    public ICommand AddConfigCommand => _addConfigCommand ??= new RelayCommand(AddConfig);

    /// <summary>
    /// 删除配置项
    /// </summary>
    public async void AddConfig()
    {
        var gestureAppConfig = new GestureAppConfig()
        {
            Id = Guid.NewGuid().ToString(),
        };
        GestureAppConfigs.Add(gestureAppConfig);
    }

    [RelayCommand]
    public async Task AddLaunchApp()
    {
        try
        {
            var theme = App.GetService<IThemeSelectorService>();
            var addLaunchApDialog = new ContentDialog()
            {
                Title = "AddAppStartConfigTitle".GetLocalized(),
                PrimaryButtonText = "AddEmojisOkBtnContent".GetLocalized(),
                CloseButtonText = "AddEmojisCancelBtnContent".GetLocalized(),
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Content = new LaunchAppPage(),
                RequestedTheme = theme.Theme
            };

            addLaunchApDialog.PrimaryButtonClick += AddLaunchApDialog_PrimaryButtonClick;

            addLaunchApDialog.Closed += LaunchAppDialog_Closed;

            await addLaunchApDialog.ShowAsync();
        }
        catch
        {

        }
    }

    private async void AddLaunchApDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {

        if (sender.Content is LaunchAppPage page)
        {
            if (page.DataContext is LaunchAppViewModel viewModel)
            {
                if (viewModel is not null)
                {
                    if (string.IsNullOrWhiteSpace(viewModel.AppNameText))
                    {
                        ToastHelper.SendToast("AppNameNullText".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(viewModel.VoiceText))
                    {
                        ToastHelper.SendToast("VoiceNullText".GetLocalized(), TimeSpan.FromSeconds(3));
                        args.Cancel = true;
                        return;
                    }

                    await viewModel.SaveLaunchApp();

                    var launchAppConfig = new LaunchAppConfig
                    {
                        VoiceText = viewModel.VoiceText,
                        Win32Path = viewModel.Win32Path,
                        AppNameText = viewModel.AppNameText,
                        IsMsix = viewModel.IsMsix
                    };

                    LaunchApps.Add(launchAppConfig);
                }
            }
        }
    }


    [RelayCommand]
    public async Task DelEmojis(object? obj)
    {
        if (obj == null)
        {
            ToastHelper.SendToast("请选中一个项", TimeSpan.FromSeconds(3));
            return;
        }
        if (obj is LaunchAppConfig emojis)
        {
            try
            {
                LaunchApps.Remove(emojis);

                await _localSettingsService.SaveSettingAsync(Constants.LaunchAppConfigKey, LaunchApps.ToList());
            }
            catch (Exception ex)
            {
                ToastHelper.SendToast($"删除失败-{ex.Message}", TimeSpan.FromSeconds(3));
            }
        }
    }

    private  void LaunchAppDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
    {

    }

    public void OnNavigatedTo(object parameter)
    {
        Task.Run(async () =>
        {
            var launchAppConfigs = (await _localSettingsService.ReadSettingAsync<List<LaunchAppConfig>>
           (Constants.LaunchAppConfigKey)) ?? new List<LaunchAppConfig>();

            App.MainWindow.DispatcherQueue.TryEnqueue(() => LaunchApps = new ObservableCollection<LaunchAppConfig>(launchAppConfigs));
        });
    }

    public void OnNavigatedFrom()
    {

    }
}
