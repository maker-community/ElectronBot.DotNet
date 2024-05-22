using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Helpers;
using HelixToolkit.SharpDX.Core;
using Microsoft.UI.Xaml;

namespace ViewModels;
public partial class ModelLoadCompactOverlayViewModel : ObservableRecipient
{
    private readonly DispatcherTimer _timer = new()
    {
        Interval = TimeSpan.FromMilliseconds(200)
    };

    [ObservableProperty] private string _voiceResult = string.Empty;

    public ModelLoadCompactOverlayViewModel()
    {

        _timer.Tick += Timer_Tick;
    }

    private async void Timer_Tick(object? sender, object e)
    {
        var resultState = EbHelper.IsVoiceEnabled();

        if (resultState)
        {
            VoiceResult = "空格+E组合键按下";
            var voiceLock = ElectronBotHelper.Instance.VoiceLock;

            if (resultState && !voiceLock)
            {
                await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync("你需要帮忙吗", true);
            }

            ElectronBotHelper.Instance.VoiceLock = true;
        }
        else
        {
            VoiceResult = "空格+E组合键松开";
        }
    }

    [RelayCommand]
    public void PlayAction()
    {
        var playEmojisLock = ElectronBotHelper.Instance.PlayEmojisLock;

        if (!playEmojisLock)
        {
            //随机播放表情
            ElectronBotHelper.Instance.ToPlayEmojisRandom();
        }

        ElectronBotHelper.Instance.PlayEmojisLock = true;
    }


    [RelayCommand]
    public void Loaded()
    {
        _timer.Start();
        ElectronBotHelper.Instance.LoadAppList();
    }

    public void UnLoaded()
    {
        ElectronBotHelper.Instance.PlayEmojisLock = false;

        _timer.Stop();
    }


    [RelayCommand]
    public void CompactOverlay()
    {
        App.MainWindow.Show();
    }
}
