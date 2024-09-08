using System.Windows.Forms;
using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Services;
using HelloWordKeyboard.DotNet;
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
    private readonly System.Timers.Timer timer = new();

    private static Hw75GlobalTimerHelper? _instance;
    public static Hw75GlobalTimerHelper Instance => _instance ??= new Hw75GlobalTimerHelper();

    public Hw75GlobalTimerHelper()
    {
        timer.Interval = TimeSpan.FromSeconds(60).TotalMilliseconds;
        timer.Elapsed += Timer_Tick;
    }

    private async void Timer_Tick(object? sender, object e)
    {
        await UpdateHwViewAsync();
    }

    public async Task UpdateTimerIntervalAsync()
    {
        var _localSettingsService = App.GetService<ILocalSettingsService>();
        var config = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey) ?? new CustomClockTitleConfig();

        if (config.Hw75ViewName == Hw75ViewNameEnum.Hw75CustomViewName)
        {
            timer.Interval = TimeSpan.FromSeconds(60).TotalMilliseconds;
        }
        else if (config.Hw75ViewName == Hw75ViewNameEnum.Hw75WeatherViewName)
        {
            timer.Interval = TimeSpan.FromMinutes(30).TotalMilliseconds;
        }
        else if (config.Hw75ViewName == Hw75ViewNameEnum.Hw75YellowCalendarViewName)
        {
            timer.Interval = TimeSpan.FromMinutes(60).TotalMilliseconds;
        }
    }

    public async Task UpdateHwViewAsync()
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

            var device = App.GetService<IHw75DynamicDevice>();

            _ = device.SetEInkImage(byteArray, 0, 0, 128, 296, false);
        }
        catch (Exception ex)
        {
        }
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

        var font = await GetFontAsync(config.Hw75CustomContentFontSize, "SmileySans-Oblique.ttf");

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
            x.Grayscale();
            x.Resize(128, 296);
            
            if (config.Hw75CustomContentIsVisibility)
            {
                var textPosition = new PointF((128 - textSize.Width) / 2, 296 - textSize.Height - 8);
                x.DrawText(config.Hw75CustomContent, font, Color.Black, textPosition);
            }
            if (config.Hw75TimeIsVisibility)
            {
                var datePosition = new PointF((128 - dateTextSize.Width) / 2, 2);
                x.DrawText(date, dateFont, Color.Black, datePosition);

                var todayTimePosition = new PointF((128 - todayTimeTextSize.Width) / 2, dateTextSize.Height + 2 + 2);
                x.DrawText(todayTime, todyTimeFont, Color.Black, todayTimePosition);

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
        try
        {
            var gpsResult = await GpsGetWeather.GetWeatherIdea();

            var proviceAndCity = $"{gpsResult.Now.Province} {gpsResult.Now.City}";

            var temperature = gpsResult.Now.Temperature;

            var skycon = gpsResult.Now.Skycon;

            var time = gpsResult.Now.Time;

            var wind = gpsResult.Now.Wind;

            var windSd = gpsResult.Now.Wind_sd;

            var sunSet = gpsResult.Now.SunSet;

            var sunRise = gpsResult.Now.SunRise;

            var pressure = gpsResult.Now.Pressure;

            var tempFont = await GetFontAsync(24, "SmileySans-Oblique.ttf");

            var bigFont = await GetFontAsync(24);

            var smallFont = await GetFontAsync(12);

            var tempTextOptions = new TextOptions(tempFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 128
            };

            var bigTextOptions = new TextOptions(bigFont)
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

            var proviceAndCityLines = WrapText(proviceAndCity, bigFont, 128);

            var temperatureLines = WrapText(temperature, tempFont, 128);

            var temperatureTotalWidth = temperatureLines.Sum(line => TextMeasurer.MeasureSize(line, tempTextOptions).Width);

            var width = temperatureTotalWidth + 40 + 8;

            var margin = (int)(128 - width) / 2;

            using var weatherIcon = await LoadImageAsync(gpsResult.Now.Icon);

            weatherIcon.Mutate(x =>
            {
                x.Resize(new Size(50, 50));
            });

            var skyconLines = WrapText(skycon, bigFont, 128);

            var timeLines = WrapText(time, smallFont, 128);

            var windLines = WrapText(wind, smallFont, 128);

            var windSdLines = WrapText(windSd, smallFont, 128);

            var sunSetLines = WrapText(sunSet, smallFont, 128);

            var sunRiseLines = WrapText(sunRise, smallFont, 128);

            var pressureLines = WrapText(pressure, smallFont, 128);

            image.Mutate(ctx =>
            {
                ctx.Grayscale();
                foreach (var proviceAndCityLine in proviceAndCityLines)
                {
                    var size = TextMeasurer.MeasureSize(proviceAndCityLine, bigTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(proviceAndCityLine, bigFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                ctx.DrawImage(weatherIcon, new Point(margin, (int)yOffset), opacity: 1);

                foreach (var temperatureLine in temperatureLines)
                {
                    var size = TextMeasurer.MeasureSize(temperatureLine, tempTextOptions);
                    var position = new PointF(margin + 40 + 8, yOffset);
                    ctx.DrawText(temperatureLine, tempFont, Color.Black, position);
                    yOffset += size.Height + +40 + 8;
                }

                foreach (var skyconLine in skyconLines)
                {
                    var size = TextMeasurer.MeasureSize(skyconLine, bigTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(skyconLine, bigFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var timeLine in timeLines)
                {
                    var size = TextMeasurer.MeasureSize(timeLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(timeLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var windLine in windLines)
                {
                    var size = TextMeasurer.MeasureSize(windLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(windLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var windSdLine in windSdLines)
                {
                    var size = TextMeasurer.MeasureSize(windSd, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(windSd, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var sunSetLine in sunSetLines)
                {
                    var size = TextMeasurer.MeasureSize(sunSetLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(sunSetLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 4;
                }

                foreach (var sunRiseLine in sunRiseLines)
                {
                    var size = TextMeasurer.MeasureSize(sunRiseLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(sunRiseLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var pressureLine in pressureLines)
                {
                    var size = TextMeasurer.MeasureSize(pressureLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(pressureLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }
            });

            //var destinationFolder = await KnownFolders.PicturesLibrary
            //    .CreateFolderAsync("ElectronBot\\Hw75View", CreationCollisionOption.OpenIfExists);

            //image.Save($"{destinationFolder.Path}\\" + ".weather.jpg");

            var byteArray = image.EnCodeImageToBytes();
            return byteArray;



        }
        catch (Exception ex)
        {
            return Array.Empty<byte>();
        }
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

            var bigFont = await GetFontAsync(24);

            var mediumFont = await GetFontAsync(18);

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

            //var yangLiTotalHeight = yangLiLines.Sum(line => TextMeasurer.MeasureSize(line, bigTextOptions).Height);

            var yinLiLines = WrapText(yinLi, smallFont, 128);

            var yinLiTotalHeight = yinLiLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var wuXingTitleLines = WrapText("五行", smallFont, 128);

            //var wuXingTitleTotalHeight = wuXingTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var wuXingLines = WrapText(wuXing, mediumFont, 128);

            var wuXingTotalHeight = wuXingLines.Sum(line => TextMeasurer.MeasureSize(line, mediumTextOptions).Height);

            var chongShaTitleLines = WrapText("冲煞", smallFont, 128);

            //var chongShaTitleTotalHeight = chongShaTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var chongShaLines = WrapText(chongSha, smallFont, 128);

            var chongShaTotalHeight = chongShaLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var xiongShenTitleLines = WrapText("凶神宜忌", smallFont, 128);

            //var xiongShenTitleTotalHeight = xiongShenTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var xiongShenLines = WrapText(xiongShen, mediumFont, 128);

            //var xiongShenTotalHeight = xiongShenLines.Sum(line => TextMeasurer.MeasureSize(line, mediumTextOptions).Height);

            var jiTitleLines = WrapText("忌", smallFont, 128);

            //var jiTitleTotalHeight = jiTitleLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);

            var jiLines = WrapText(ji, smallFont, 128);

            //var jiTotalHeight = jiLines.Sum(line => TextMeasurer.MeasureSize(line, smallTextOptions).Height);


            image.Mutate(ctx =>
            {
                ctx.Grayscale();
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

            //var destinationFolder = await KnownFolders.PicturesLibrary
            //    .CreateFolderAsync("ElectronBot\\Hw75View", CreationCollisionOption.OpenIfExists);

            //image.Save($"{destinationFolder.Path}\\" + ".yellow.jpg");
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
