using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using HelloWordKeyboard.DotNet;
using Microsoft.UI.Xaml;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Windows.Storage.Streams;

namespace Helpers;
public class Hw75GlobalTimerHelper
{
    private readonly DispatcherTimer timer = new();

    private static Hw75GlobalTimerHelper? _instance;
    public static Hw75GlobalTimerHelper Instance => _instance ??= new Hw75GlobalTimerHelper();

    public Hw75GlobalTimerHelper()
    {
        // Set the interval to 1 second
        timer.Interval = TimeSpan.FromSeconds(60);

        // Set the event handler for the Tick event
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, object e)
    {
        App.MainWindow?.DispatcherQueue.TryEnqueue(async () =>
        {
            try
            {
                var _localSettingsService = App.GetService<ILocalSettingsService>();
                var config = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);
                using var image = SixLabors.ImageSharp.Image.Load<Rgba32>(config?.CustomHw75ImagePath ?? "");

                image.Mutate(x =>
                {
                    x.Resize(128, 296);
                    //x.Grayscale();
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

        // Start the timer
        timer.Start();
    }

    public void StopTimer()
    {
        // Stop the timer when the application closes
        timer.Stop();
    }
}
