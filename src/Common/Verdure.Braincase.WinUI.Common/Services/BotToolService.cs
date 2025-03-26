using System.Linq;
using System.Text;
using System.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.WinUI.Common.Helpers;
using Verdure.ElectronBot.Core.Contracts.Services;
using Verdure.ElectronBot.Core.Models;
using Windows.Storage;

namespace Verdure.Braincase.WinUI.Common.Services;
public class BotToolService : IBotToolService
{
    private readonly IBotIotService _botIotService;
    public BotToolService(IBotIotService botIotService)
    {
        _botIotService = botIotService;
    }
    public Task SendBliFansToBotAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public async Task<string> SendWeatherToBotAsync(CancellationToken cancellationToken = default)
    {
        var strBuilder = new StringBuilder();
        try
        {
            var gpsResult = await GpsGetWeather.GetWeatherIdea();

            var proviceAndCity = $"{gpsResult.Now.Province} {gpsResult.Now.City}";

            var temperature = gpsResult.Now.Temperature;

            var skycon = gpsResult.Now.Skycon;

            var time = gpsResult.Now.Time;

            var wind = gpsResult.Now.Wind;

            var windSd = gpsResult.Now.Wind_sd;

            strBuilder.AppendLine($"省份和城市：{proviceAndCity}");
            strBuilder.AppendLine($"温度：{temperature}");
            strBuilder.AppendLine($"天气概况：{skycon}");
            strBuilder.AppendLine($"时间：{time}");
            strBuilder.AppendLine($"风：{wind}");
            strBuilder.AppendLine($"空气湿度：{windSd}");

            var tempFont = await GetFontAsync(24, "SmileySans-Oblique.ttf");

            var bigFont = await GetFontAsync(24);

            var smallFont = await GetFontAsync(12);

            var tempTextOptions = new TextOptions(tempFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 240
            };

            var bigTextOptions = new TextOptions(bigFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 240
            };

            var smallTextOptions = new TextOptions(smallFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 240
            };

            // 创建一个新的图像，背景为白色
            using var image = new Image<Rgba32>(240, 240, Color.White);

            float yOffset = 20;

            var proviceAndCityLines = WrapText(proviceAndCity, bigFont, 240);

            var temperatureLines = WrapText(temperature, tempFont, 240);

            var temperatureTotalWidth = temperatureLines.Sum(line => TextMeasurer.MeasureSize(line, tempTextOptions).Width);

            var width = temperatureTotalWidth + 40 + 8;

            var margin = (int)(240 - width) / 2;

            var iconPath = gpsResult.Now.Icon;
            iconPath = iconPath.Replace("Assets", "Assets/Copilot");
            using var weatherIcon = await LoadImageAsync(iconPath);

            weatherIcon.Mutate(x =>
            {
                x.Resize(new Size(100, 100));
            });

            var skyconLines = WrapText(skycon, bigFont, 240);

            var timeLines = WrapText(time, smallFont, 240);

            var windLines = WrapText(wind, smallFont, 240);

            var windSdLines = WrapText(windSd, smallFont, 240);

            image.Mutate(ctx =>
            {
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
            });

            //var destinationFolder = await KnownFolders.PicturesLibrary
            //    .CreateFolderAsync("ElectronBot\\Hw75View", CreationCollisionOption.OpenIfExists);

            //image.Save($"{destinationFolder.Path}\\" + ".weather.jpg");

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

            var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

            var frameData = new EmoticonActionFrame(rgbData);

            _ = await service.SendToUsbDeviceAsync(frameData);
        }
        catch (Exception ex)
        {
        }
        return strBuilder.ToString();
    }

    public async Task SendWordsToBotAsync(LearnWordsContent content, CancellationToken cancellationToken = default)
    {
        try
        {
            var bigFont = await GetFontAsync(24, "SmileySans-Oblique.ttf");

            var smallFont = await GetFontAsync(20, "SmileySans-Oblique.ttf");

            var iconPath = "ms-appx:///Assets/StoreLogo.backup.png";

            using var botIcon = await LoadImageAsync(iconPath);

            botIcon.Mutate(x =>
            {
                x.Resize(new Size(40, 40));
            });

            var bigTextOptions = new TextOptions(bigFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 240
            };

            var smallTextOptions = new TextOptions(smallFont)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                WrappingLength = 240
            };

            // 创建一个新的图像，背景为白色
            using var image = new Image<Rgba32>(240, 240, Color.White);

            float yOffset = 20;

            var wordLines = WrapText(content.Word, bigFont, 240);

            var descLines = WrapText(content.WordDescription, smallFont, 200);

            image.Mutate(ctx =>
            {
                ctx.DrawImage(botIcon, new Point((image.Width - 40) / 2, (int)yOffset), opacity: 1);

                yOffset += 40 + 8;
                foreach (var wordLine in wordLines)
                {
                    var size = TextMeasurer.MeasureSize(wordLine, bigTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(wordLine, bigFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }

                foreach (var descLine in descLines)
                {
                    var size = TextMeasurer.MeasureSize(descLine, smallTextOptions);
                    var position = new PointF((image.Width - size.Width) / 2, yOffset);
                    ctx.DrawText(descLine, smallFont, Color.Black, position);
                    yOffset += size.Height + 8;
                }
            });

            //var destinationFolder = await KnownFolders.PicturesLibrary
            //    .CreateFolderAsync("ElectronBot\\Hw75View", CreationCollisionOption.OpenIfExists);

            //image.Save($"{destinationFolder.Path}\\" + "word.jpg");

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

            var service = Ioc.Default.GetRequiredService<IEmoticonActionFrameService>();

            var frameData = new EmoticonActionFrame(rgbData);

            _ = await service.SendToUsbDeviceAsync(frameData);
        }
        catch (Exception ex)
        {
        }
    }

    public async Task SendImageDataToBotSettingAsync(string imageData, CancellationToken cancellationToken = default)
    {
        var folder = ApplicationData.Current.LocalFolder;

        var storageFolder = await folder.CreateFolderAsync(Constants.EmojisFolder, CreationCollisionOption.OpenIfExists);

        var storageFile = await storageFolder
            .CreateFileAsync($"CustomViewPicture-{DateTime.Now.Second}.png", CreationCollisionOption.ReplaceExisting);

        var localSettingsService = Ioc.Default.GetRequiredService<ILocalSettingsService>();

        var botSetting = await localSettingsService.ReadSettingAsync<BotSetting>(Constants.BotSettingKey);
        if (botSetting != null && !string.IsNullOrEmpty(imageData))
        {
            var writeableBitmapImage = await ImageHelper.WriteableBitmapFromBase64StringAsync(imageData);

            if (await ImageHelper.SaveWriteableBitmapImageFileAsync(writeableBitmapImage, storageFile))
            {
                botSetting.CustomViewPicturePath = storageFile.Path;
                await localSettingsService.SaveSettingAsync(Constants.BotSettingKey, botSetting);

                var clockView = new ChangeClockView
                {
                    ClockViewName = "CustomView"
                };
                WeakReferenceMessenger.Default.Send(clockView);
            }
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
        var lines = new List<string>();
        var currentLine = new StringBuilder();
        var isChinese = text.Any(ch => ch >= 0x4E00 && ch <= 0x9FFF);

        foreach (var ch in text)
        {
            var testLine = currentLine.Append(ch).ToString();
            var size = TextMeasurer.MeasureSize(testLine, new TextOptions(font));

            if (size.Width > maxWidth || (!isChinese && ch == ' '))
            {
                lines.Add(currentLine.ToString(0, currentLine.Length - 1));
                currentLine.Clear();
                if (!isChinese && ch != ' ')
                {
                    currentLine.Append(ch);
                }
            }
        }

        if (currentLine.Length > 0)
        {
            lines.Add(currentLine.ToString());
        }

        return lines;
    }
}
