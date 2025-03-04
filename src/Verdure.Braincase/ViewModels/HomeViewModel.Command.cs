using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.UI.Xaml.Controls;
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

            await _localSettingsService
                .SaveSettingAsync(Constants.CurrentClockViewKey, clockName);
        }
    }


    [RelayCommand]
    public void Stop()
    {
        _dispatcherTimer.Stop();
    }

    [RelayCommand]
    public void Clear()
    {
        actions.Clear();

        count = 0;

        actionCount = 0;

        ToastHelper.SendToast("PlayClearToastText".GetLocalized(), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public void Reconnect()
    {
        try
        {
            _dispatcherTimer.Stop();
            //ElectronBotHelper.Instance?.ElectronBot?.Disconnect();
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
    public async Task OnRadioButtonSelectionChangedAsync(object parameter)
    {
        // 处理切换事件的逻辑
        var selectedRadioButton = parameter as RadioButton;
        if (selectedRadioButton != null)
        {
            // 根据选中的RadioButton执行相应的逻辑
            var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

            service.ClearQueue();

            await _localSettingsService.SaveSettingAsync(Constants.CurrentModeKey, selectedRadioButton.Name);

            if (selectedRadioButton.Name == "NaturalMode")
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
            else if (selectedRadioButton.Name == "ClockMode")
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
            else if (selectedRadioButton.Name == "NeedleMode")
            {
                if (!ElectronBotHelper.Instance.EbConnected)
                {
                    ToastHelper.SendToast("PleaseConnectToastText".GetLocalized(), TimeSpan.FromSeconds(3));
                }
                else
                {
                    await ResetActionAsync();

                    //var matData = new OpenCvSharp.Mat(Package.Current.InstalledLocation.Path + $"\\Assets\\Emoji\\Pic\\eyes-closed.png");

                    //var mat2 = matData.CvtColor(OpenCvSharp.ColorConversionCodes.RGBA2BGR);

                    //var dataMeta = mat2.Data;

                    //var data = new byte[240 * 240 * 3];

                    //Marshal.Copy(dataMeta, data, 0, 240 * 240 * 3);

                    //EbHelper.FaceData = data;

                    _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
                    _dispatcherTimer.Start();
                }
            }
            else
            {
                _dispatcherTimer.Stop();
            }
        }
    }
}
