using System.Threading.Tasks;
using BotSharp.Abstraction.Crontab.Models;
using BotSharp.Core.Crontab.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.Braincase.Copilot.Hooks;

public class NotifyCrontabHook : ICrontabHook
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NotifyCrontabHook> _logger;

    public NotifyCrontabHook(IServiceProvider services,
        ILogger<NotifyCrontabHook> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task OnCronTriggered(CrontabItem item)
    {
        var imageUrl = "ms-appx:///Assets/Images/HomeImage.jpg";
        //var wallpaperService = _services.GetRequiredService<IWallpaperService>();
        //var wallpaper = await wallpaperService.GetWallparperAsync(0, 1);
        //if (wallpaper != null && wallpaper.images.Length > 0)
        //{
        //    var image = wallpaper.images[0];
        //    imageUrl = "https://www.bing.com" + image.url;
        //}
        if (item.Tasks.Length > 0)
        {
            var task = item.Tasks[0];
            if(task.Script == "call_user()")
            {
                //var builder = new AppNotificationBuilder()
                //    .SetScenario(AppNotificationScenario.IncomingCall)
                //    .AddText("Andrew Bares", new AppNotificationTextProperties()
                //        .SetIncomingCallAlignment())
                //      .AddText("Incoming Call - Mobile", new AppNotificationTextProperties()
                //        .SetIncomingCallAlignment())
                //      .SetInlineImage(new Uri("ms-appx:///Images/Profile.png"),
                //        AppNotificationImageCrop.Circle)
                //    .AddButton(new AppNotificationButton()
                //        .SetToolTip("Answer Video Call")
                //        .SetButtonStyle(AppNotificationButtonStyle.Success)
                //        .SetIcon(new Uri("ms-appx:///Images/Video.png"))
                //        .AddArgument("videoId", "123"))
                //    .AddButton(new AppNotificationButton()
                //        .SetToolTip("Answer Phone Call")
                //        .SetButtonStyle(AppNotificationButtonStyle.Success)
                //        .SetIcon(new Uri("ms-appx:///Images/Call.png"))
                //        .AddArgument("callId", "123"))
                //    .AddButton(new AppNotificationButton()
                //        .SetToolTip("Hang Up")
                //        .SetButtonStyle(AppNotificationButtonStyle.Critical)
                //        .SetIcon(new Uri("ms-appx:///Images/HangUp.png"))
                //        .AddArgument("hangUpId", "123"));
                //AppNotificationManager.Default.Show(builder.BuildNotification());
            }
        }
        else
        {
            var builder = new AppNotificationBuilder()
                .AddText(item.Title)
                .AddText(item.Description)
                .SetHeroImage(new Uri(imageUrl));
            AppNotificationManager.Default.Show(builder.BuildNotification());
        }
    }

    public async Task OnTaskExecuted(CrontabItem item)
    {
        var speech = _services.GetRequiredService<IBotSpeech>();
        await speech.SpeakAsync(item.Title);
    }
}
