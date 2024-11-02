using System.Net.Http;
using System.Text.Json;
using Microsoft.UI.Dispatching;
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
                .ReadSettingAsync<CustomClockTitleConfig>(CommonConstants.CustomClockTitleConfigKey);
            var clockTitleConfig = ret2 ?? new CustomClockTitleConfig();

            var urlLast = $"{host}?date={DateTime.Now.ToShortDateString()}&key={clockTitleConfig.Hw75YellowCalendarKey}";

            var yellowCalendarStr = await _localSettingsService.ReadSettingAsync<string>($"{CommonConstants.YellowCalendarKey}-{DateTime.Now.ToShortDateString()}");

            if (noCache == true || string.IsNullOrWhiteSpace(yellowCalendarStr) || (!string.IsNullOrWhiteSpace(yellowCalendarStr) && !yellowCalendarStr.Contains("successed")))
            {
                var httpClient = Ioc.Default.GetRequiredService<HttpClient>();

                yellowCalendarStr = await httpClient.GetStringAsync(urlLast);

                await _localSettingsService.SaveSettingAsync<string>($"{CommonConstants.YellowCalendarKey}-{DateTime.Now.ToShortDateString()}", yellowCalendarStr);
            }

            var data = JsonSerializer.Deserialize<YellowCalendarData>(yellowCalendarStr);

            if (data != null && data.Reason == "successed")
            {
                return data.Result;
            }
            else
            {
                var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                dispatcherQueue.TryEnqueue(() =>
                {
                    //ToastHelper.SendToast($"请检查聚合数据key的配置", TimeSpan.FromSeconds(5));
                });
            }
        }
        catch (Exception ex)
        {

        }

        return new YellowCalendarResult();
    }
}
