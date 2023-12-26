using ElectronBot.Braincase;
using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using Models.Hw75.YellowCalendar;

namespace Services.Hw75Services.YellowCalendar;
public class GetYellowCalendarService
{
    private const string host = "http://v.juhe.cn/laohuangli/d";
    public static async Task<YellowCalendarResult> GetYellowCalendarAsync()
    {
        try
        {
            var _localSettingsService = App.GetService<ILocalSettingsService>();

            var ret2 = await _localSettingsService
                .ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey);
            var clockTitleConfig = ret2 ?? new CustomClockTitleConfig();

            var urlLast = $"{host}?date={DateTime.Now.ToShortDateString().ToString()}&key={clockTitleConfig.Hw75YellowCalendarKey}";

            var httpClient = App.GetService<HttpClient>();
            var resultJson = await httpClient.GetStringAsync(urlLast);
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<YellowCalendarData>(resultJson);

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
