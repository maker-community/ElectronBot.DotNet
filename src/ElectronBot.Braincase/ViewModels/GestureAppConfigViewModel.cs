using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Contracts.ViewModels;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;

namespace ElectronBot.Braincase.ViewModels;

public partial class GestureAppConfigViewModel : ObservableRecipient
{
    private readonly ILocalSettingsService _localSettingsService;

    private ObservableCollection<GestureAppConfig> _gestureAppConfigs = new();
    public ObservableCollection<GestureAppConfig> GestureAppConfigs
    {
        get => _gestureAppConfigs;
        set => SetProperty(ref _gestureAppConfigs, value);
    }

    public List<string> GestureLabels { get; set; } = new()
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
        for(int i = 0; i < GestureAppConfigs.Count; i++)
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
}
