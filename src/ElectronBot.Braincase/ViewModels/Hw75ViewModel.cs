using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using HelloWordKeyboard.DotNet.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Verdure.ElectronBot.Core.Models;

namespace ElectronBot.Braincase.ViewModels;

public partial class Hw75ViewModel : ObservableRecipient, INavigationAware
{
    /// <summary>
    /// eink content
    /// </summary>
    [ObservableProperty]
    UIElement? _element;

    /// <summary>
    /// 时钟选中数据
    /// </summary>
    [ObservableProperty]
    ComboxItemModel? clockComBoxSelect;

    /// <summary>
    /// 瀚文设备信息
    /// </summary>
    [ObservableProperty]
    DeviceInfo? _deviceInfo;

    /// <summary>
    /// 固件版本
    /// </summary>
    [ObservableProperty]
    string? _firmwareVersion;

    /// <summary>
    /// ZMK版本
    /// </summary>
    [ObservableProperty]
    string? _zmkVersion;

    /// <summary>
    /// Zephyr版本
    /// </summary>
    [ObservableProperty]
    string? _zephyrVersion;

    /// <summary>
    /// 瀚文界面列表
    /// </summary>
    [ObservableProperty]
    public ObservableCollection<ComboxItemModel> clockComboxModels;

    [ObservableProperty]
    private CustomClockTitleConfig _clockTitleConfig;

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
            Hw75Helper.Instance.ViewName = clockName;

            var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider(clockName);

            Element = viewProvider.CreateHw75DynamickView(clockName);

            ClockTitleConfig.Hw75ViewName = clockName;

            await _localSettingsService.SaveSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey, ClockTitleConfig);
        }
    }

    [RelayCommand]
    private void OnLoaded()
    {
        // var result = Hw75Helper.Instance.Hw75DynamicDevice?.SetKnobSwitchModeConfig(true);
    }

    public async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var isOn = toggleSwitch.IsOn;
            if (isOn)
            {
                Hw75Helper.Instance.StartTimer();
            }
            else
            {
                Hw75Helper.Instance.StopTimer();
            }
            await Task.Run(() =>
            {
                try
                {

                    if (isOn)
                    {
                        Hw75Helper.Instance.Hw75DynamicDevice?.SetKnobSwitchModeConfig(true, UsbComm.KnobConfig.Types.Mode.Inertia);
                    }
                    else
                    {
                        Hw75Helper.Instance.Hw75DynamicDevice?.SetKnobSwitchModeConfig(false, UsbComm.KnobConfig.Types.Mode.Encoder);
                    }
                }
                catch
                {
                }

            });
        }
    }

    public async void OnNavigatedTo(object parameter)
    {

        var ret2 = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);

        ClockTitleConfig = ret2 ?? new CustomClockTitleConfig();

        var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider(ClockTitleConfig.Hw75ViewName);

        Element = viewProvider.CreateHw75DynamickView(ClockTitleConfig.Hw75ViewName);

        Hw75Helper.Instance.UpdateDataToDeviceHandler += Instance_UpdateDataToDeviceHandler;

        try
        {
            DeviceInfo = Hw75Helper.Instance.Hw75DynamicDevice?.Open();

            Hw75Helper.Instance.IsConnected = true;

            var firmwareInfo = Hw75Helper.Instance.Hw75DynamicDevice?.GetVersion();

            ZmkVersion = firmwareInfo?.ZmkVersion;

            FirmwareVersion = firmwareInfo?.AppVersion;

            ZephyrVersion = firmwareInfo?.ZephyrVersion;

            Hw75Helper.Instance.StartTimer();
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

            Hw75Helper.Instance.IsConnected = false;
        }
    }

    private async void Instance_UpdateDataToDeviceHandler(object? sender, EventArgs e)
    {
        await Task.Delay(1000);
        await Hw75Helper.Instance.SyncDataToDeviceAsync(Element);
    }

    public void OnNavigatedFrom()
    {
        Hw75Helper.Instance.Hw75DynamicDevice?.Close();
        Hw75Helper.Instance.IsConnected = false;
        Hw75Helper.Instance.ViewName = "Hw75CustomView";
        Hw75Helper.Instance.UpdateDataToDeviceHandler -= Instance_UpdateDataToDeviceHandler;
        Hw75Helper.Instance.StopTimer();

        //Task.Run(() =>
        //{
        //    Hw75Helper.Instance.Hw75DynamicDevice?.SetKnobSwitchModeConfig(false, UsbComm.KnobConfig.Types.Mode.Encoder);
        //});
    }
}
