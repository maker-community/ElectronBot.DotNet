using System.Numerics;
using System.Windows.Forms;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using HelloWordKeyboard.DotNet;
using Microsoft.UI.Xaml;
using Models;
using Services.Hw75Services.YellowCalendar;
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

                var byteArray = Array.Empty<byte>();

                if (config.Hw75ViewName == Hw75ViewNameEnum.Hw75CustomViewName)
                {
                    byteArray = await GetHw75CustomImageAsync(config);
                }
                else if (config.Hw75ViewName == Hw75ViewNameEnum.Hw75WeatherViewName)
                {
                    byteArray = await GetHw75WeatherImageAsync(config);
                }
                else if (config.Hw75ViewName == Hw75ViewNameEnum.Hw75YellowCalendarViewName)
                {
                    byteArray = await GetHw75YellowCalendarImageAsync(config);
                }

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

    private async Task<byte[]> GetHw75CustomImageAsync(CustomClockTitleConfig config)
    {
        using var image = await LoadImageAsync(config.CustomHw75ImagePath);

        var font = await GetFontAsync(config.Hw75CustomContentFontSize);

        // 计算文本尺寸
        var textOptions = new TextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        var textSize = TextMeasurer.MeasureSize(config.Hw75CustomContent, textOptions);

        var todayTime = DateTimeOffset.Now.ToString("t");
        var date = $"{DateTimeOffset.Now.Day}{DateTimeOffset.Now:ddd}";

        var dateFont = await GetFontAsync(24);
        var todyTimeFont = await GetFontAsync(36);

        var dateTextSize = TextMeasurer.MeasureSize(date, new TextOptions(dateFont));
        var todayTimeTextSize = TextMeasurer.MeasureSize(todayTime, new TextOptions(todyTimeFont));

        image.Mutate(x =>
        {
            x.Resize(128, 296);

            if (config.Hw75CustomContentIsVisibility)
            {
                var datePosition = new PointF((128 - dateTextSize.Width) / 2, 2);
                x.DrawText(date, dateFont, Color.Black, datePosition);

                var todayTimePosition = new PointF((128 - todayTimeTextSize.Width) / 2, dateTextSize.Height + 2 + 2);
                x.DrawText(todayTime, todyTimeFont, Color.Black, todayTimePosition);

                var textPosition = new PointF((128 - textSize.Width) / 2, 296 - textSize.Height - 4);
                x.DrawText(config.Hw75CustomContent, font, Color.Black, textPosition);
            }
        });

        //var destinationFolder = await KnownFolders.PicturesLibrary
        //    .CreateFolderAsync("ElectronBot\\Hw75View", CreationCollisionOption.OpenIfExists);

        //image.Save($"{destinationFolder.Path}\\" + ".test.jpg");
        var byteArray = image.EnCodeImageToBytes();
        return byteArray;
    }

    private async Task<byte[]> GetHw75WeatherImageAsync(CustomClockTitleConfig config)
    {
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
        return byteArray;
    }

    private async Task<byte[]> GetHw75YellowCalendarImageAsync(CustomClockTitleConfig config)
    {
        var yellowCalendar = await GetYellowCalendarService.GetYellowCalendarAsync();

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
        return byteArray;
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
