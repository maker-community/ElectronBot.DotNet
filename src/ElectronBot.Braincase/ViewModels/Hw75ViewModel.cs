﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using HelloWordKeyboard.DotNet.Models;
using Microsoft.UI.Xaml;
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

    private readonly IHw75DynamicViewProviderFactory _viewProviderFactory;

    public Hw75ViewModel(ComboxDataService comboxDataService, IHw75DynamicViewProviderFactory viewProviderFactory)
    {
        ClockComboxModels = comboxDataService.GetHw75ViewComboxList();
        _viewProviderFactory = viewProviderFactory;
    }

    /// <summary>
    /// 表盘切换方法
    /// </summary>
    [RelayCommand]
    private void ClockChanged()
    {
        var clockName = ClockComBoxSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(clockName))
        {
            Hw75Helper.Instance.ViewName = clockName;

            var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider(clockName);

            Element = viewProvider.CreateHw75DynamickView(clockName);
        }
    }

    [RelayCommand]
    private void OnLoaded()
    {

    }

    public void OnNavigatedTo(object parameter)
    {
        var viewProvider = _viewProviderFactory.CreateHw75DynamicViewProvider("Hw75CustomView");

        Element = viewProvider.CreateHw75DynamickView("Hw75CustomView");

        Hw75Helper.Instance.UpdateDataToDeviceHandler += Instance_UpdateDataToDeviceHandler;

        try
        {
            DeviceInfo = Hw75Helper.Instance.Hw75DynamicDevice?.Open();

            Hw75Helper.Instance.IsConnected = true;

            var firmwareInfo = Hw75Helper.Instance.Hw75DynamicDevice?.GetVersion();

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
    }
}
