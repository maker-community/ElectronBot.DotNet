using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using HelloWordKeyboard.DotNet;
using HelloWordKeyboard.DotNet.Models;
using Helpers;
using Microsoft.UI.Xaml;
using Verdure.ElectronBot.Core.Models;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75ViewModel : ObservableRecipient, INavigationAware
{
    /// <summary>
    /// eink content
    /// </summary>
    [ObservableProperty]
    private UIElement? _element;

    /// <summary>
    /// 时钟选中数据
    /// </summary>
    [ObservableProperty]
    private ComboxItemModel? _clockComBoxSelect;

    /// <summary>
    /// 瀚文设备信息
    /// </summary>
    [ObservableProperty]
    private DeviceInfo? _deviceInfo;

    /// <summary>
    /// 固件版本
    /// </summary>
    [ObservableProperty]
    private string? _firmwareVersion;

    /// <summary>
    /// ZMK版本
    /// </summary>
    [ObservableProperty]
    private string? _zmkVersion;

    /// <summary>
    /// Zephyr版本
    /// </summary>
    [ObservableProperty]
    private string? _zephyrVersion;

    /// <summary>
    /// 瀚文界面列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel>? clockComboxModels;

    [ObservableProperty]
    private CustomClockTitleConfig? _clockTitleConfig;

    private readonly IHw75DynamicViewProviderFactory _viewProviderFactory;

    private readonly ILocalSettingsService _localSettingsService;

    public Hw75ViewModel(ComboxDataService comboxDataService, IHw75DynamicViewProviderFactory viewProviderFactory, ILocalSettingsService localSettingsService)
    {
        ClockComboxModels = comboxDataService.GetHw75ViewComboxList();
        _viewProviderFactory = viewProviderFactory;
        _localSettingsService = localSettingsService;
    }

    /// <summary>
    /// 表盘切换方法
    /// </summary>
    [RelayCommand]
    private async Task ClockChanged()
    {
        var clockName = ClockComBoxSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(clockName))
        {
            var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider(clockName);

            Element = viewProvider.CreateHw75DynamickView(clockName);

            ClockTitleConfig!.Hw75ViewName = clockName;

            await _localSettingsService.SaveSettingAsync(Constants.CustomClockTitleConfigKey, ClockTitleConfig);

            await Task.Run(async () =>
            {
                await Hw75GlobalTimerHelper.Instance.UpdateTimerIntervalAsync();
                await Hw75GlobalTimerHelper.Instance.UpdateHwViewAsync();
            });

        }
    }

    public async void OnNavigatedTo(object parameter)
    {

        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider(ClockTitleConfig.Hw75ViewName);

        Element = viewProvider.CreateHw75DynamickView(ClockTitleConfig.Hw75ViewName);

        try
        {
            var device = App.GetService<IHw75DynamicDevice>();
            DeviceInfo = device.Open();

            var firmwareInfo = device.GetVersion();

            ZmkVersion = firmwareInfo?.ZmkVersion;

            FirmwareVersion = firmwareInfo?.AppVersion;

            ZephyrVersion = firmwareInfo?.ZephyrVersion;
        }
        catch (Exception ex)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast(ex.Message, TimeSpan.FromSeconds(2));
            });

            DeviceInfo = new DeviceInfo
            {
                DeviceName = ex.Message
            };
        }
    }

    public void OnNavigatedFrom()
    {

    }
}
