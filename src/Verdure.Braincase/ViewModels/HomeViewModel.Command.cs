using BotSharp.Abstraction.Crontab.Models;
using BotSharp.Abstraction.Repositories;
using CommunityToolkit.Mvvm.Input;
using Controls.CompactOverlay;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Verdure.Braincase.EbScreen.Views;
using Verdure.Braincase.Helpers;
using Verdure.Braincase.WinUI.Common.Helpers;

namespace Verdure.Braincase.ViewModels;
public partial class HomeViewModel
{
    /// <summary>
    /// 表盘切换方法
    /// </summary>
    [RelayCommand]
    private async Task ClockChanged()
    {
        var clockName = ClockComBoxSelect?.DataKey;

        if (!string.IsNullOrWhiteSpace(clockName))
        {
            var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

            service.ClearQueue();

            var viewProvider = _viewProviderFactory.CreateClockViewProvider(clockName);

            if (clockName == "GooeyFooter" || clockName == "CustomView" || clockName == "GrooveView")
            {
                _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            }
            else
            {
                _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            }

            Element = viewProvider.CreateClockView(clockName);

            await _localSettingsService.SaveSettingAsync(Constants.CurrentClockViewKey, clockName);

            if (!_dispatcherTimer.IsEnabled)
            {
                _dispatcherTimer.Start();
            }
        }
    }


    [RelayCommand]
    private void ElectronEmulation()
    {
        try
        {
            _dispatcherTimer.Stop();
            WindowEx compactOverlay = new CompactOverlayWindow();

            compactOverlay.Content = Ioc.Default.GetRequiredService<MiniModePage>();

            var appWindow = compactOverlay.AppWindow;

            appWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);

            appWindow.Show();

            App.MainWindow.Hide();

        }
        catch (Exception)
        {
        }
    }

    [RelayCommand]
    private void RebootElectron()
    {
        try
        {
            if (!ElectronBotHelper.Instance.SerialPort.IsOpen)
            {
                ElectronBotHelper.Instance.SerialPort.Open();
            }

            var byteData = new byte[]
            {
                0xea, 0x00, 0x00, 0x00, 0x00 ,0x0d, 0x02, 0x00 , 0x00, 0x0f, 0xea
            };

            ElectronBotHelper.Instance.SerialPort.Write(byteData, 0, byteData.Length);

            Thread.Sleep(1000);

            if (ElectronBotHelper.Instance.SerialPort.IsOpen)
            {
                ElectronBotHelper.Instance.SerialPort.Close();
            }

        }
        catch (Exception)
        {
        }
    }

    [RelayCommand]
    public void Stop()
    {
        _dispatcherTimer.Stop();
    }

    [RelayCommand]
    public void Reconnect()
    {
        try
        {
            _dispatcherTimer.Stop();
            ElectronBotHelper.Instance?.ElectronBot?.ResetDevice();
        }
        catch (Exception)
        {

        }
        ToastHelper.SendToast("ReconnectText".GetLocalized(), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public async Task ResetAsync()
    {
        if (modeNo == 1)
        {
            if (ElectronBotHelper.Instance.EbConnected)
            {
                await ResetActionAsync();

                ToastHelper.SendToast("PlayResetToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
        }
        else
        {
            ToastHelper.SendToast("PlayErrorToastText".GetLocalized(), TimeSpan.FromSeconds(3));
        }
    }

    [RelayCommand]
    public void ClearTask()
    {
        var db = _services.GetRequiredService<IBotSharpRepository>();

        var tasks = db.GetCrontabItems(new CrontabItemFilter
        {
            Page = 1,
            Size = 100
        });

        if (tasks.Count > 0)
        {
            foreach (var task in tasks.Items)
            {
                db.DeleteCrontabItem(task.ConversationId);
            }
        }

        ToastHelper.SendToast("ClearTaskToastText".GetLocalized(), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public async Task OnRadioButtonSelectionChangedAsync(object parameter)
    {
        // 处理切换事件的逻辑
        var selectedRadioButton = parameter as RadioButton;
        if (selectedRadioButton != null)
        {
            await ChangeAppModeAsync(selectedRadioButton.Name);
        }
    }

    private async Task ChangeAppModeAsync(string modeName)
    {
        // 根据选中的RadioButton执行相应的逻辑
        var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

        service.ClearQueue();

        await _localSettingsService.SaveSettingAsync(Constants.CurrentModeKey, modeName);

        ModeIndex = ModeNameToIndex(modeName);

        if (modeName == "NaturalMode")
        {
            if (!ElectronBotHelper.Instance.EbConnected)
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                await ResetActionAsync();

                var clockName = ClockComBoxSelect?.DataKey;

                if (clockName != "GooeyFooter" && clockName != "CustomView")
                {
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                }
                _dispatcherTimer.Start();
            }
        }
        else if (modeName == "ClockMode")
        {
            if (!ElectronBotHelper.Instance.EbConnected)
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                await ResetActionAsync();

                var clockName = ClockComBoxSelect?.DataKey;

                if (clockName != "GooeyFooter" && clockName != "CustomView")
                {
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                }

                if (!_dispatcherTimer.IsEnabled)
                {
                    _dispatcherTimer.Start();
                }     
            }
        }
        else if (modeName == "NeedleMode")
        {
            if (!ElectronBotHelper.Instance.EbConnected)
            {
                ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
            }
            else
            {
                await ResetActionAsync();
                _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
                if (!_dispatcherTimer.IsEnabled)
                {
                    _dispatcherTimer.Start();
                }
            }
        }
        else
        {
            //_dispatcherTimer.Stop();
        }
    }

    private int ModeNameToIndex(string modeName)
    {
        if (modeName == "NaturalMode")
        {
            return 0;
        }
        else if (modeName == "ClockMode")
        {
            return 1;
        }
        else if (modeName == "NeedleMode")
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
}
