using System.Threading;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using SkiaSharp.Skottie;

namespace Verdure.Braincase.WinUI.Common.Players;

public class LottiePlayer : IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isPlaying = false;
    private Animation? _currentAnimation;
    private bool _disposed = false;

    // 事件定义
    public event EventHandler<LottieEventArgs>? PlayStarted;
    public event EventHandler<LottieEventArgs>? PlayCompleted;
    public event EventHandler<LottieEventArgs>? PlayStopped;
    public event EventHandler<LottieFrameRenderedEventArgs>? FrameRendered;

    // 帧处理委托
    public delegate Task FrameProcessorDelegate(byte[] frameData, int width, int height);
    public FrameProcessorDelegate? FrameProcessor
    {
        get; set;
    }

    // 播放配置
    public int Width { get; set; } = 240;
    public int Height { get; set; } = 240;
    public int FrameDelay { get; set; } = 16; // 约60fps
    public bool UseHardwareAcceleration { get; set; } = true;

    public LottiePlayer()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public LottiePlayer(FrameProcessorDelegate frameProcessor) : this()
    {
        FrameProcessor = frameProcessor;
    }

    /// <summary>
    /// 播放Lottie动画
    /// </summary>
    /// <param name="filePath">Lottie JSON文件路径</param>
    /// <param name="loopCount">循环次数，-1表示无限循环</param>
    /// <returns></returns>
    public async Task PlayAsync(string filePath, int loopCount = 1)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_isPlaying)
            {
                await StopAsync();
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _isPlaying = true;

            // 加载动画
            _currentAnimation = Animation.Create(filePath);
            if (_currentAnimation == null)
            {
                throw new InvalidOperationException($"Failed to load Lottie animation from: {filePath}");
            }

            // 通知动画开始
            var eventArgs = new LottieEventArgs { FilePath = filePath };

            PlayStarted?.Invoke(this, eventArgs);

            // 开始播放动画循环
            await Task.Run(async () =>
            {
                try
                {
                    var currentLoop = 0;
                    while ((loopCount < 0 || currentLoop < loopCount) && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _currentAnimation.Seek(0);
                        var frameCount = _currentAnimation.OutPoint;

                        for (var i = 0; i < frameCount; i++)
                        {
                            if (_cancellationTokenSource.Token.IsCancellationRequested)
                                break;

                            // 计算进度
                            var progress = i / frameCount * _currentAnimation.Duration.TotalSeconds;

                            // 渲染当前帧
                            var frameImage = RenderLottieFrame(_currentAnimation, progress, Width, Height);

                            // 处理帧数据
                            var rgbData = ConvertImageToRgbData(frameImage);

                            // 调用帧处理委托
                            if (FrameProcessor != null)
                            {
                                await FrameProcessor(rgbData, Width, Height);
                            }

                            // 触发帧渲染事件
                            FrameRendered?.Invoke(this, new LottieFrameRenderedEventArgs
                            {
                                FilePath = filePath,
                                FrameIndex = i,
                                TotalFrames = (int)frameCount,
                                Progress = progress,
                                LoopIndex = currentLoop,
                                FrameData = rgbData
                            });

                            // 控制帧率 由于针对设备写入已经有延时这里延时取消
                            //await Task.Delay(FrameDelay, _cancellationTokenSource.Token);
                        }

                        currentLoop++;
                    }

                    if (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        PlayCompleted?.Invoke(this, eventArgs);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 预期的取消操作，不需要处理
                }
                catch (Exception ex)
                {
                    // 记录异常但不抛出
                    Console.WriteLine($"Error during Lottie playback: {ex}");
                }
                finally
                {
                    _isPlaying = false;
                }
            });
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 停止播放动画
    /// </summary>
    public async Task StopAsync()
    {
        if (!_isPlaying) return;

        try
        {
            _cancellationTokenSource.Cancel();

            // 等待一段时间确保动画线程已停止
            await Task.Delay(100);

            PlayStopped?.Invoke(this, new LottieEventArgs
            {
                FilePath = _currentAnimation?.ToString() ?? string.Empty
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping Lottie playback: {ex}");
        }
        finally
        {
            _isPlaying = false;
        }
    }

    /// <summary>
    /// 渲染Lottie动画帧
    /// </summary>
    private static Image<Bgra32> RenderLottieFrame(Animation animation, double progress, int width, int height)
    {
        // 创建SKSurface用于渲染
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        // 清除背景
        canvas.Clear(SKColors.Transparent);

        // 渲染动画帧
        animation.SeekFrameTime(progress);
        animation.Render(canvas, new SKRect(0, 0, width, height));

        // 将SKBitmap转换为图像
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var bytes = data.ToArray();

        // 转换为ImageSharp格式
        using var memStream = new MemoryStream(bytes);
        return Image.Load<Bgra32>(memStream);
    }

    /// <summary>
    /// 将图像转换为RGB字节数组
    /// </summary>
    private static byte[] ConvertImageToRgbData(Image<Bgra32> image)
    {
        var rgbData = new byte[image.Width * image.Height * 3];

        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var pixel = image[x, y];
                var rgbIndex = (y * image.Width + x) * 3;
                rgbData[rgbIndex] = pixel.B;
                rgbData[rgbIndex + 1] = pixel.G;
                rgbData[rgbIndex + 2] = pixel.R;
            }
        }

        return rgbData;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _semaphore?.Dispose();
            _currentAnimation?.Dispose();
        }

        _disposed = true;
    }
}

// 事件参数类
public class LottieEventArgs : EventArgs
{
    public string FilePath
    {
        get; set;
    }
}

public class LottieFrameRenderedEventArgs : LottieEventArgs
{
    public int FrameIndex
    {
        get; set;
    }
    public int TotalFrames
    {
        get; set;
    }
    public double Progress
    {
        get; set;
    }
    public int LoopIndex
    {
        get; set;
    }
    public byte[] FrameData
    {
        get; set;
    }
}