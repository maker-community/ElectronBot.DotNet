using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Models.Hw75.YellowCalendar;

namespace Services.Hw75Services.YellowCalendar;
public class GetYellowCalendarService
{
    private const string host = "http://v.juhe.cn/laohuangli/d";
    public static async Task<YellowCalendarResult> GetYellowCalendarAsync(bool noCache = false)
    {
        try
        {
            var _localSettingsService = Ioc.Default.GetRequiredService<ILocalSettingsService>();

            var ret2 = await _localSettingsService
                .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);
            var clockTitleConfig = ret2 ?? new CustomClockTitleConfig();

            var urlLast = $"{host}?date={DateTime.Now.ToShortDateString()}&key={clockTitleConfig.Hw75YellowCalendarKey}";

            var yellowCalendarStr = await _localSettingsService.ReadSettingAsync<string>($"{Constants.YellowCalendarKey}-{DateTime.Now.ToShortDateString()}");

            if (noCache == true || string.IsNullOrWhiteSpace(yellowCalendarStr) || (!string.IsNullOrWhiteSpace(yellowCalendarStr) && !yellowCalendarStr.Contains("successed")))
            {
                var httpClient = Ioc.Default.GetRequiredService<HttpClient>();

                yellowCalendarStr = await httpClient.GetStringAsync(urlLast);

                await _localSettingsService.SaveSettingAsync<string>($"{Constants.YellowCalendarKey}-{DateTime.Now.ToShortDateString()}", yellowCalendarStr);
            }

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<YellowCalendarData>(yellowCalendarStr);

            if (data != null && data.Reason == "successed")
            {
                return data.Result;
            }
            else
            {
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    ToastHelper.SendToast($"请检查聚合数据key的配置", TimeSpan.FromSeconds(5));
                });
            }
        }
        catch(Exception ex)
        {

        }

        return new YellowCalendarResult();
    }
}
