using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
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
            var _localSettingsService = App.GetService<ILocalSettingsService>();

            var ret2 = await _localSettingsService
                .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);
            var clockTitleConfig = ret2 ?? new CustomClockTitleConfig();

            var urlLast = $"{host}?date={DateTime.Now.ToShortDateString()}&key={clockTitleConfig.Hw75YellowCalendarKey}";

            var yellowCalendarStr = await _localSettingsService.ReadSettingAsync<string>($"{Constants.YellowCalendarKey}-{DateTime.Now.ToShortDateString()}");

            if (noCache == true || string.IsNullOrWhiteSpace(yellowCalendarStr))
            {
                var httpClient = App.GetService<HttpClient>();

                yellowCalendarStr = await httpClient.GetStringAsync(urlLast);

                await _localSettingsService.SaveSettingAsync<string>($"{Constants.YellowCalendarKey}-{DateTime.Now.ToShortDateString()}", yellowCalendarStr);
            }

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<YellowCalendarData>(yellowCalendarStr);

            if (data != null && data.Reason == "successed")
            {
                return data.Result;
            }
        }
        catch
        {
        }

        return new YellowCalendarResult();
    }
}
