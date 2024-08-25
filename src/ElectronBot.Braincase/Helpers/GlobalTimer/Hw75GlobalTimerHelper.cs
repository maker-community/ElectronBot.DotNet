using System.Numerics;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using HelloWordKeyboard.DotNet;
using Microsoft.UI.Xaml;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Windows.Storage;
using HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment;
using VerticalAlignment = SixLabors.Fonts.VerticalAlignment;

namespace Helpers;
public class Hw75GlobalTimerHelper
{
    private readonly DispatcherTimer timer = new();

    private static Hw75GlobalTimerHelper? _instance;
    public static Hw75GlobalTimerHelper Instance => _instance ??= new Hw75GlobalTimerHelper();

    public Hw75GlobalTimerHelper()
    {
        timer.Interval = TimeSpan.FromSeconds(6);
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, object e)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                var _localSettingsService = App.GetService<ILocalSettingsService>();
                var config = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey) ?? new CustomClockTitleConfig();

                using var image = await LoadImageAsync(config.CustomHw75ImagePath);

                var font = await GetFontAsync(config.Hw75CustomContentFontSize);

                // 计算文本尺寸
                TextOptions textOptions = new TextOptions(font)
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                var textSize = TextMeasurer.MeasureSize(config.Hw75CustomContent, textOptions);

                // 计算文本居中位置
                PointF center = new PointF(128 / 2, 296 / 2);


                image.Mutate(x =>
                {
                    x.Resize(128, 296);
                    x.DrawText(config.Hw75CustomContent, font, Color.Black, new Vector2(textSize.X, textSize.Y));
                });
                var byteArray = image.EnCodeImageToBytes();

                _ = Hw75Helper.Instance.Hw75DynamicDevice?.SetEInkImage(byteArray, 0, 0, 128, 296, false);
            }
            catch (Exception ex)
            {
            }
        });
    }

    public void StartTimer()
    {
        timer.Start();
    }

    public void StopTimer()
    {
        timer.Stop();
    }

    private async Task<Font> GetFontAsync(float size, string fontName = "fusion-pixel-12px-monospaced-zh_hans.ttf")
    {
        var collection = new FontCollection();

        using var stream = await GetFileStreamAsync($"ms-appx:///Assets/Font/{fontName}");
        var family = collection.Add(stream);
        var font = family.CreateFont(size, FontStyle.Bold);
        return font;
    }

    private async Task<Image<Rgba32>> LoadImageAsync(string imagePath)
    {
        if (imagePath.StartsWith("ms-appx"))
        {
            using var stream = await GetFileStreamAsync(imagePath);
            return Image.Load<Rgba32>(stream);
        }
        else
        {
            return Image.Load<Rgba32>(imagePath);
        }
    }

    private async Task<Stream> GetFileStreamAsync(string filePath)
    {
        var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(filePath));
        var randomAccessStream = await storageFile.OpenAsync(FileAccessMode.Read);
        return randomAccessStream.AsStreamForRead();
    }
}
