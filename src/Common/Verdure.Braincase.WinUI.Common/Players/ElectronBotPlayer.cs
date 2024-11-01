using System.Diagnostics;
using Microsoft.Graphics.Canvas;
using SixLabors.ImageSharp.Processing;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.WinUI.Common.Models;
using Windows.Media.Playback;

namespace Verdure.Braincase.WinUI.Common.Players;
public partial class ElectronBotPlayer : IElectronBotPlayer, IDisposable
{
    private readonly IEmoticonActionFrameService _actionFrameService;

    private readonly MediaPlayer _player;

    private double _frameRate = 30.0; // 假设视频的帧率为30帧每秒

    private List<ElectronBotAction>? _actions;
    public ElectronBotPlayer(IEmoticonActionFrameService actionFrameService, MediaPlayer player)
    {
        _actionFrameService = actionFrameService;
        _player = player;

        _player.MediaEnded += MediaPlayer_MediaEnded;
        _player.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;

        _player.IsVideoFrameServerEnabled = true;
    }

    private async void MediaPlayer_VideoFrameAvailable(MediaPlayer sender, object args)
    {
        var canvasDevice = CanvasDevice.GetSharedDevice();

        using var _canvasRenderTarget = new CanvasRenderTarget(canvasDevice,
            (float)sender.PlaybackSession.NaturalVideoWidth, (float)sender.PlaybackSession.NaturalVideoHeight, 96);

        // 将视频帧复制到CanvasRenderTarget
        sender.CopyFrameToVideoSurface(_canvasRenderTarget);

        // 获取帧数据
        using var ds = _canvasRenderTarget.CreateDrawingSession();
        var pixelBytes = _canvasRenderTarget.GetPixelBytes();

        //使用ImageSharp处理帧数据
        using var image = SixLabors.ImageSharp.Image
            .LoadPixelData<SixLabors.ImageSharp.PixelFormats.Rgba32>(pixelBytes,
            (int)_canvasRenderTarget.SizeInPixels.Width, (int)_canvasRenderTarget.SizeInPixels.Height);

        image.Mutate(x =>
        {
            x.Resize(240, 240);
        });

        // 获取转换后的数据
        var rgbData = new byte[image.Width * image.Height * 3];

        // 遍历每个像素，将Rgba32转换为Bgr24
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var rgbaPixel = image[x, y];
                var rgbIndex = (y * image.Width + x) * 3;
                rgbData[rgbIndex] = rgbaPixel.B;
                rgbData[rgbIndex + 1] = rgbaPixel.G;
                rgbData[rgbIndex + 2] = rgbaPixel.R;
            }
        }

        var currentAction = new ElectronBotAction();

        // 获取当前播放位置
        var position = sender.PlaybackSession.Position;

        // 计算当前帧的帧序
        var currentFrameIndex = (int)(position.TotalSeconds * _frameRate);

        Debug.WriteLine($"当前帧序：{currentFrameIndex}");

        if (_actions != null && _actions.Count > 0)
        {
            var fullTime = sender.NaturalDuration;

            var bili = position / fullTime;

            var actionCount = (int)(_actions.Count * bili);

            if (actionCount >= _actions.Count)
            {
                actionCount = _actions.Count - 1;
            }
            currentAction = _actions[actionCount];
        }

        var frameData = new EmoticonActionFrame(rgbData, true,
            currentAction.J1,
            currentAction.J2,
            currentAction.J3,
            currentAction.J4,
            currentAction.J5,
            currentAction.J6);

        _ = await _actionFrameService.SendToUsbDeviceAsync(frameData);
    }
    private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
    }

    public void Dispose()
    {
        _player?.Dispose();
    }
}
