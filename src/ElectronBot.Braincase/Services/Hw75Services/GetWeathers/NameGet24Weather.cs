using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Models.Name24;
using Microsoft.Extensions.Options;
using Windows.Web.Http;

namespace ElectronBot.Braincase.Services
{
    /// <summary>
    /// 24小时天气
    /// </summary>
    public class NameGet24Weather
    {
        private const string host = "https://ali-weather.showapi.com";
        private const string path = "/hour24";
        /// <summary>
        /// 获取24小时的天气情况
        /// </summary>
        /// <param name="name">地区的名称</param>
        /// <returns>返回24小时json实例</returns>
        public static async Task<NameWeather24Data> NameGet24WeatherIdea(string name)
        {
            var querys = "area=" + name;
            var url = host + path;
            var url2 = url + "?" + querys;
            var resultJson = string.Empty;

            var appCode = App.GetService<IOptions<LocalSettingsOptions>>().Value.Hw75AppCode;

            var _localSettingsService = App.GetService<ILocalSettingsService>();
            var config = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey) ?? new CustomClockTitleConfig();

            if (!string.IsNullOrWhiteSpace(config.Hw75WeatherAppCode))
            {
                appCode = config.Hw75WeatherAppCode;
            }

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                Uri uri = new Uri(url2);
                httpClient.DefaultRequestHeaders.Add("Authorization", "APPCODE " + appCode);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                resultJson = await httpClient.GetStringAsync(uri);
            }
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<NameWeather24Data>(resultJson);
            return data;
        }
    }
}
