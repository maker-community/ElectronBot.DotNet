using CommunityToolkit.Mvvm.Input;
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
}
