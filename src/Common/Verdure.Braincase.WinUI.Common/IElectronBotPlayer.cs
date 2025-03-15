using Microsoft.UI.Xaml;
using Verdure.Braincase.WinUI.Common.Models;

namespace Verdure.Braincase.WinUI.Common;
public interface IElectronBotPlayer
{
    Task PlayVideoByPathAsync(string path, List<ElectronBotAction>? actions = null);
    Task PlayVideoByNameIdAsync(string nameId);
    Task PlayLottieByNameIdAsync(string nameId, int times = 1);
    Task StopLottiePlaybackAsync();
    Task PlayAudioByTextAsync(string text);
    Task PlayAudioByPathAsync(string path);
    Task PlayAudioAsync(Stream stream);
    Task PlayImageAsync(string path);
    Task PlayImageAsync(Stream stream);
    Task PlayImageAsync(byte[] bytes);
    Task PlayImageAsync(UIElement element);
}
