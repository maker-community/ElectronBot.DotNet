using ElectronBot.Braincase.Contracts.Services;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Models.Gps;
using ElectronBot.Braincase.Models.Name24;
using Microsoft.Extensions.Options;
using Windows.Devices.Geolocation;

namespace ElectronBot.Braincase.Services;

public class GpsGetWeather
{
    private const string host = "https://ali-weather.showapi.com";
    private const string path = "/gps-to-weather";
    private const string areapath = "/area-to-weather";
    private const string method = "GET";


    /// <summary>
    /// 通过经纬度获取天气情况
    /// </summary>
    /// <param name="lat">纬度</param>
    /// <param name="lon">经度</param>
    /// <returns>结果封装的类的实例</returns>
    public static async Task<Weather_Displayed> GetWeatherIdea()
    {

        var weatherDisplayed = new Weather_Displayed();
        try
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            var geolocator = new Geolocator();
            if (accessStatus != GeolocationAccessStatus.Allowed)
            {
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    ToastHelper.SendToast($"请检查系统设置是否开启系统定位权限。", TimeSpan.FromSeconds(5));
                });
                return weatherDisplayed;
            };
            var pos = await geolocator.GetGeopositionAsync();
            var lat = pos.Coordinate.Point.Position.Latitude;
            var lon = pos.Coordinate.Point.Position.Longitude;
            var querys = "from=5&lat=" + lat.ToString() + "&lng=" + lon.ToString() + "&need3HourForcast=0&needAlarm=0&needHourData=0&needIndex=1&needMoreDay=1";
            var url = host + path;
            var urlLast = url + "?" + querys;
            var uri = new Uri(urlLast);
            var resultJson = string.Empty;

            var appCode = App.GetService<IOptions<LocalSettingsOptions>>().Value.Hw75AppCode;

            var _localSettingsService = App.GetService<ILocalSettingsService>();
            var config = await _localSettingsService.ReadSettingAsync<CustomClockTitleConfig>(Constants.CustomClockTitleConfigKey) ?? new CustomClockTitleConfig();

            if (!string.IsNullOrWhiteSpace(config.Hw75WeatherAppCode))
            {
                appCode = config.Hw75WeatherAppCode;
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "APPCODE " + appCode);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                resultJson = await httpClient.GetStringAsync(uri);
            }
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<GpsWeatherData>(resultJson);

            var hour24 = await NameGet24Weather.NameGet24WeatherIdea(System.Web.HttpUtility.UrlEncode(data.showapi_res_body.cityInfo.c3, System.Text.Encoding.UTF8));
            //var hour24 = await NameGet24Weather.NameGet24WeatherIdea(TransCoding.UrlCode(data.showapi_res_body.cityInfo.c3, "utf-8"));
            OrganizeWeatherData(weatherDisplayed, data, hour24);
        }
        catch(Exception ex)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ToastHelper.SendToast($"天气获取错误。{ex.Message}", TimeSpan.FromSeconds(5));
            });
        }
      
        return weatherDisplayed;
    }

    #region 这是数据处理部分
    private static void OrganizeWeatherData(Weather_Displayed weatherDisplayed, GpsWeatherData data, NameWeather24Data hour24)
    {
        weatherDisplayed.Now = new WeatherUnit();
        var unit = weatherDisplayed.Now;
        unit.Skycon = data.showapi_res_body.now.weather;
        unit.Aqi = data.showapi_res_body.now.aqi;
        unit.Province = data.showapi_res_body.cityInfo.c7;
        unit.City = data.showapi_res_body.cityInfo.c3;
        unit.Humidity = data.showapi_res_body.now.sd;
        unit.Temperature = data.showapi_res_body.now.temperature + "℃";
        unit.SunSet = data.showapi_res_body.f1.sun_begin_end.Substring(0, 5) + " 日出";
        unit.SunRise = data.showapi_res_body.f1.sun_begin_end.Substring(6, 5) + " 日落";
        unit.Time = "最后更新时间" + data.showapi_res_body.now.temperature_time;
        unit.Wind_sd = "湿度  " + data.showapi_res_body.now.sd;
        unit.Wind = "风力  " + data.showapi_res_body.now.wind_direction + data.showapi_res_body.now.wind_power;
        unit.Pressure = "气压 " + data.showapi_res_body.f1.air_press;
        unit.Icon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.now.weather_pic.Substring(32));
        string[] week = new string[7] { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };
        weatherDisplayed.HourlyForecates_24 = new List<Hour24>();
        var hourly24 = weatherDisplayed.HourlyForecates_24;
        for (int i = 0; i < 24; i++)
        {
            hourly24.Add(new Hour24()
            {
                Temperature = hour24.showapi_res_body.hourList[i].temperature + "℃",
                Data = hour24.showapi_res_body.hourList[i].time.Substring(8, 2) + ":" + hour24.showapi_res_body.hourList[i].time.Substring(10, 2),
                Icon = string.Format("ms-appx:///Assets/icon/day/{0}.png", hour24.showapi_res_body.hourList[i].weather_code),
            });
        }

        weatherDisplayed.DailyForecates = new List<WeatherUnit>();

        var dailyForecates = weatherDisplayed.DailyForecates;

        dailyForecates.Add(new WeatherUnit
        {
            Week = week[data.showapi_res_body.f1.weekday - 1],
            Date = data.showapi_res_body.f1.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f1.weekday - 1] + ")",
            DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f1.day_weather_pic.Substring(32)),
            DayTemperature = data.showapi_res_body.f1.day_air_temperature,
            NightTemperature = data.showapi_res_body.f1.night_air_temperature,
            Skycon = data.showapi_res_body.f1.day_weather

        });

        dailyForecates.Add(new WeatherUnit
        {
            Week = week[data.showapi_res_body.f2.weekday - 1],
            Date = data.showapi_res_body.f2.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f2.weekday - 1] + ")",
            DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f2.day_weather_pic.Substring(32)),
            DayTemperature = data.showapi_res_body.f2.day_air_temperature,
            NightTemperature = data.showapi_res_body.f2.night_air_temperature,
            Skycon = data.showapi_res_body.f2.day_weather

        });
        dailyForecates.Add(new WeatherUnit
        {
            Week = week[data.showapi_res_body.f3.weekday - 1],
            Date = data.showapi_res_body.f3.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f3.weekday - 1] + ")",
            DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f3.day_weather_pic.Substring(32)),
            DayTemperature = data.showapi_res_body.f3.day_air_temperature,
            NightTemperature = data.showapi_res_body.f3.night_air_temperature,
            Skycon = data.showapi_res_body.f3.day_weather

        });

        dailyForecates.Add(new WeatherUnit
        {
            Week = week[data.showapi_res_body.f4.weekday - 1],
            Date = data.showapi_res_body.f4.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f4.weekday - 1] + ")",
            DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f4.day_weather_pic.Substring(32)),
            DayTemperature = data.showapi_res_body.f4.day_air_temperature,
            NightTemperature = data.showapi_res_body.f4.night_air_temperature,
            Skycon = data.showapi_res_body.f4.day_weather



        });
        dailyForecates.Add(new WeatherUnit
        {
            Week = week[data.showapi_res_body.f5.weekday - 1],
            Date = data.showapi_res_body.f5.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f5.weekday - 1] + ")",
            DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f5.day_weather_pic.Substring(32)),
            DayTemperature = data.showapi_res_body.f5.day_air_temperature,
            NightTemperature = data.showapi_res_body.f5.night_air_temperature,
            Skycon = data.showapi_res_body.f5.day_weather

        });
        dailyForecates.Add(new WeatherUnit
        {
            Week = week[data.showapi_res_body.f6.weekday - 1],
            Date = data.showapi_res_body.f6.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f6.weekday - 1] + ")",
            DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f6.day_weather_pic.Substring(32)),
            DayTemperature = data.showapi_res_body.f6.day_air_temperature,
            NightTemperature = data.showapi_res_body.f6.night_air_temperature,
            Skycon = data.showapi_res_body.f6.day_weather

        });

        dailyForecates.Add(new WeatherUnit
        {
            Week = week[data.showapi_res_body.f7.weekday - 1],
            Date = data.showapi_res_body.f7.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f7.weekday - 1] + ")",
            DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f7.day_weather_pic.Substring(32)),
            DayTemperature = data.showapi_res_body.f7.day_air_temperature,
            NightTemperature = data.showapi_res_body.f7.night_air_temperature,
            Skycon = data.showapi_res_body.f7.day_weather

        });







        //建议
        weatherDisplayed.Suggestions = new List<Suggestion>();
        var suggestions = weatherDisplayed.Suggestions;
        suggestions.Add(new Suggestion
        {
            Name = "空气质量",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Air.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Air.png",
            Brf = data.showapi_res_body.f1.index.aqi.title,
            Txt = data.showapi_res_body.f1.index.aqi.desc,
        });


        suggestions.Add(new Suggestion
        {
            Name = "舒服指数",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Comf.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Comf.png",
            Brf = data.showapi_res_body.f1.index.comfort.title,
            Txt = data.showapi_res_body.f1.index.comfort.desc,
        });
        suggestions.Add(new Suggestion
        {
            Name = "洗车指数",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Cw.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Cw.png",
            Brf = data.showapi_res_body.f1.index.wash_car.title,
            Txt = data.showapi_res_body.f1.index.wash_car.desc,
        });
        suggestions.Add(new Suggestion
        {
            Name = "穿衣指数",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Drsg.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Drsg.png",
            Brf = data.showapi_res_body.f1.index.clothes.title,
            Txt = data.showapi_res_body.f1.index.clothes.desc,



        });


        suggestions.Add(new Suggestion
        {
            Name = "感冒指数",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Flu.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Flu.png",

            Brf = data.showapi_res_body.f1.index.cold.title,
            Txt = data.showapi_res_body.f1.index.cold.desc,
        });


        suggestions.Add(new Suggestion
        {
            Name = "运动指数",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Sport.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Sport.png",
            Brf = data.showapi_res_body.f1.index.sports.title,
            Txt = data.showapi_res_body.f1.index.sports.desc,
        });
        suggestions.Add(new Suggestion
        {
            Name = "旅游指数",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Trav.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Trav.png",
            Brf = data.showapi_res_body.f1.index.travel.title,
            Txt = data.showapi_res_body.f1.index.travel.desc,
        });
        suggestions.Add(new Suggestion
        {
            Name = "防晒指数",
            LightIcon = "ms-appx:///Assets/SuggestionIcon/Light/Uv.png",
            DarkIcon = "ms-appx:///Assets/SuggestionIcon/Dark/Uv.png",
            // LightIcon = ThemeSelectorService.GetHomeImageSource(),
            Brf = data.showapi_res_body.f1.index.uv.title,
            Txt = data.showapi_res_body.f1.index.uv.desc,
        });








    }

    #endregion
}
