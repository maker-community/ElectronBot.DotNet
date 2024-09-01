using System.Numerics;
using System.Windows.Forms;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
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

                //_ = Hw75Helper.Instance.Hw75DynamicDevice?.SetEInkImage(byteArray, 0, 0, 128, 296, false);
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
        try
        {
            var yellowCalendar = await GetYellowCalendarService.GetYellowCalendarAsync();

            var yangLi = yellowCalendar.Yangli;

            var yinLi = yellowCalendar.Yinli;

            var wuXing = yellowCalendar.Wuxing;

            var chongSha = yellowCalendar.Chongsha;

            var xiongShen = yellowCalendar.Xiongshen;

            var ji = yellowCalendar.Ji;

            var bigFont = await GetFontAsync(20);

            var mediumFont = await GetFontAsync(16);

            var smallFont = await GetFontAsync(12);

            var bigTextOptions = new TextOptions(bigFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 128
            };

            var mediumTextOptions = new TextOptions(mediumFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 128
            };

            var smallTextOptions = new TextOptions(smallFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 128
            };

            // 创建一个新的图像，背景为白色
            using var image = new Image<Rgba32>(128, 296, Color.White);

            float yOffset = 2;

            var yangLiLines = WrapText(yangLi, bigFont, 128);

            var yangLiTotalHeight = yangLiLines.Sum(line => TextMeasurer.MeasureSize(line, bigTextOptions).Height);

            var yinLiLines = WrapText(yinLi, smallFont, 128);

            var yinLiTotalHeight = yinLiLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var wuXingTitleLines = WrapText("五行", smallFont, 128);

            var wuXingTitleTotalHeight = wuXingTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var wuXingLines = WrapText(wuXing, mediumFont, 128);

            var wuXingTotalHeight = wuXingLines.Sum(line => TextMeasurer.MeasureSize(line, mediumTextOptions).Height);

            var chongShaTitleLines = WrapText("冲煞", smallFont, 128);

            var chongShaTitleTotalHeight = chongShaTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);


            var chongShaLines = WrapText(chongSha, smallFont, 128);

            var chongShaTotalHeight = chongShaLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);


            var xiongShenTitleLines = WrapText("凶神宜忌", smallFont, 128);

            var xiongShenTitleTotalHeight = xiongShenTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var xiongShenLines = WrapText(xiongShen, mediumFont, 128);

            var xiongShenTotalHeight = xiongShenLines.Sum(line => TextMeasurer.MeasureSize(line, mediumTextOptions).Height);

            var jiTitleLines = WrapText("忌", smallFont, 128);

            var jiTitleTotalHeight = jiTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var jiLines = WrapText(ji, smallFont, 128);

            var jiTotalHeight = jiLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);


            image.Mutate(ctx =>
            {
                foreach (var yangLiLine in yangLiLines)
                {
                    var size = TextMeasurer.MeasureSize(yangLiLine, bigTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(yangLiLine, bigFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var yinLiLine in yinLiLines)
                {
                    var size = TextMeasurer.MeasureSize(yinLiLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(yinLiLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var wuXingTitleLine in wuXingTitleLines)
                {
                    var size = TextMeasurer.MeasureSize(wuXingTitleLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(wuXingTitleLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var wuXingLine in wuXingLines)
                {
                    var size = TextMeasurer.MeasureSize(wuXingLine, mediumTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(wuXingLine, mediumFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var chongShaTitleLine in chongShaTitleLines)
                {
                    var size = TextMeasurer.MeasureSize(chongShaTitleLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(chongShaTitleLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var chongShaLine in chongShaLines)
                {
                    var size = TextMeasurer.MeasureSize(chongShaLine, mediumTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(chongShaLine, mediumFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var xiongShenTitleLine in xiongShenTitleLines)
                {
                    var size = TextMeasurer.MeasureSize(xiongShenTitleLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(xiongShenTitleLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 4;
                }

                foreach (var xiongShenLine in xiongShenLines)
                {
                    var size = TextMeasurer.MeasureSize(xiongShenLine, mediumTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(xiongShenLine, mediumFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var jiTitleLine in jiTitleLines)
                {
                    var size = TextMeasurer.MeasureSize(jiTitleLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(jiTitleLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var jiLine in jiLines)
                {
                    var size = TextMeasurer.MeasureSize(jiLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(jiLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }
            });

            //using var image = await LoadImageAsync(config.CustomHw75ImagePath);

            //var font = await GetFontAsync(config.Hw75CustomContentFontSize);

            //// 计算文本尺寸
            //TextOptions textOptions = new TextOptions(font)
            //{
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    VerticalAlignment = VerticalAlignment.Center
            //};
            //var textSize = TextMeasurer.MeasureSize(config.Hw75CustomContent, textOptions);

            //// 计算文本居中位置
            //PointF center = new PointF(128 / 2, 296 / 2);


            //image.Mutate(x =>
            //{
            //    x.Resize(128, 296);
            //    x.DrawText(config.Hw75CustomContent, font, Color.Black, new Vector2(textSize.X, textSize.Y));
            //});

            var destinationFolder = await KnownFolders.PicturesLibrary
                .CreateFolderAsync("ElectronBot\\Hw75View", CreationCollisionOption.OpenIfExists);

            image.Save($"{destinationFolder.Path}\\" + ".yellow.jpg");
            var byteArray = image.EnCodeImageToBytes();
            return byteArray;
        }
        catch (Exception ex)
        {
            return Array.Empty<byte>();
        }
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

    private static List<string> WrapText(string text, Font font, int maxWidth)
    {
        var words = text.Split(' ');
        var lines = new List<string>();
        var currentLine = string.Empty;

        foreach (var word in words)
        {
            var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
            var size = TextMeasurer.MeasureSize(testLine, new TextOptions(font));

            if (size.Width > maxWidth)
            {
                lines.Add(currentLine);
                currentLine = word;
            }
            else
            {
                currentLine = testLine;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
        {
            lines.Add(currentLine);
        }

        return lines;
    }
}
