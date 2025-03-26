using CommunityToolkit.Mvvm.Messaging;
using Verdure.Braincase.WinUI.Common.Models;
using Windows.ApplicationModel;

namespace Verdure.Braincase.WinUI.Common.Players;

public partial class ElectronBotPlayer
{
    private async Task ProcessFrame(LottieFrameEventArgs frameData)
    {
        var name = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);
        if (name == "NaturalMode")
        {
            //var frame = new EmoticonActionFrame(frameData.FrameData, false);
            await _actionFrameService.SendToUsbDeviceAsync(frameData.ActionFrameData);
        }
    }

    private async void FrameRendered(object? sender, LottieFrameRenderedEventArgs e)
    {
        var name = await _localSettingsService.ReadSettingAsync<string>(Constants.CurrentModeKey);
        if (name == "NaturalMode")
        {
            WeakReferenceMessenger.Default.Send(e);
        }

    }

    public async Task PlayLottieByNameIdAsync(string nameId, int times)
    {
        var path = Package.Current.InstalledLocation.Path + $"\\Assets\\LottieFiles\\{nameId}.json";
        await _lottiePlayer.PlayAsync(nameId, path, times);
    }

    // 提供停止播放的方法
    public async Task StopLottiePlaybackAsync()
    {
        await _lottiePlayer.StopAsync();
    }

    // 通过事件触发停止示例
    //public void SetupStopTrigger(Button stopButton)
    //{
    //    stopButton.Click += async (s, e) =>
    //    {
    //        await StopLottiePlaybackAsync();
    //    };
    //}
}
