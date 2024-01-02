using ElectronBot.Braincase.Models;
using ElectronBot.Braincase.Models.Name;
using ElectronBot.Braincase.Models.Name24;
using Microsoft.Extensions.Options;

namespace ElectronBot.Braincase.Services
{
    public class AreaNameGetWeather
    {
        private const string host = "https://ali-weather.showapi.com";
        private const string path = "/area-to-weather";

        /// <summary>
        /// 通过地区的名字获取天气情况
        /// </summary>
        /// <param name="name">地区名字的16进制码</param>
        /// <returns>返回json实例</returns>
        ///
        public static async Task<Weather_Displayed> NameGetWeatherIdea(string name)
        {
            var querys = "area=" + name + "&need3HourForcast=0&needAlarm=0&needHourData=0&needIndex=1&needMoreDay=1";
            var url = host + path;
            var urlLast = url + "?" + querys;
            var resultJson = string.Empty;

            var appCode = App.GetService<IOptions<LocalSettingsOptions>>().Value.Hw75AppCode;
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                Uri uri = new Uri(urlLast);
                httpClient.DefaultRequestHeaders.Add("Authorization", "APPCODE " + appCode);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                resultJson = await httpClient.GetStringAsync(uri);
            }
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<NameWeatherData>(resultJson);
            var weatherDisplayed = new Weather_Displayed();
            var hour24 = await NameGet24Weather.NameGet24WeatherIdea(name);
            OrganizeWeatherData(weatherDisplayed, data, hour24);
            return weatherDisplayed;

        }


        #region 这是数据处理部分
        private static void OrganizeWeatherData(Weather_Displayed weatherDisplayed, NameWeatherData data, NameWeather24Data hour24)
        {
            weatherDisplayed.Now = new WeatherUnit();
            var unit = weatherDisplayed.Now;
            unit.Skycon = data.showapi_res_body.now.weather;
            unit.Aqi = data.showapi_res_body.now.aqi;
            unit.Province = data.showapi_res_body.cityInfo.c7;
            unit.City = data.showapi_res_body.cityInfo.c3;
            unit.Humidity = data.showapi_res_body.now.sd;
            unit.Temperature = data.showapi_res_body.now.temperature + "℃";
            unit.SunSet = data.showapi_res_body.f1.sun_begin_end.Substring(0, 5);
            unit.SunRise = data.showapi_res_body.f1.sun_begin_end.Substring(6, 5);
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
                DayTemperature = data.showapi_res_body.f1.day_air_temperature + "℃",
                NightTemperature = data.showapi_res_body.f1.night_air_temperature + "℃",
                Skycon = data.showapi_res_body.f1.day_weather

            });

            dailyForecates.Add(new WeatherUnit
            {
                Week = week[data.showapi_res_body.f2.weekday - 1],
                Date = data.showapi_res_body.f2.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f2.weekday - 1] + ")",
                DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f2.day_weather_pic.Substring(32)),
                DayTemperature = data.showapi_res_body.f2.day_air_temperature + "℃",
                NightTemperature = data.showapi_res_body.f2.night_air_temperature + "℃",
                Skycon = data.showapi_res_body.f2.day_weather

            });
            dailyForecates.Add(new WeatherUnit
            {
                Week = week[data.showapi_res_body.f3.weekday - 1],
                Date = data.showapi_res_body.f3.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f3.weekday - 1] + ")",
                DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f3.day_weather_pic.Substring(32)),
                DayTemperature = data.showapi_res_body.f3.day_air_temperature + "℃",
                NightTemperature = data.showapi_res_body.f3.night_air_temperature + "℃",
                Skycon = data.showapi_res_body.f3.day_weather

            });

            dailyForecates.Add(new WeatherUnit
            {
                Week = week[data.showapi_res_body.f4.weekday - 1],
                Date = data.showapi_res_body.f4.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f4.weekday - 1] + ")",
                DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f4.day_weather_pic.Substring(32)),
                DayTemperature = data.showapi_res_body.f4.day_air_temperature + "℃",
                NightTemperature = data.showapi_res_body.f4.night_air_temperature + "℃",
                Skycon = data.showapi_res_body.f4.day_weather



            });
            dailyForecates.Add(new WeatherUnit
            {
                Week = week[data.showapi_res_body.f5.weekday - 1],
                Date = data.showapi_res_body.f5.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f5.weekday - 1] + ")",
                DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f5.day_weather_pic.Substring(32)),
                DayTemperature = data.showapi_res_body.f5.day_air_temperature + "℃",
                NightTemperature = data.showapi_res_body.f5.night_air_temperature + "℃",
                Skycon = data.showapi_res_body.f5.day_weather

            });
            dailyForecates.Add(new WeatherUnit
            {
                Week = week[data.showapi_res_body.f6.weekday - 1],
                Date = data.showapi_res_body.f6.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f6.weekday - 1] + ")",
                DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f6.day_weather_pic.Substring(32)),
                DayTemperature = data.showapi_res_body.f6.day_air_temperature + "℃",
                NightTemperature = data.showapi_res_body.f6.night_air_temperature + "℃",
                Skycon = data.showapi_res_body.f6.day_weather

            });

            dailyForecates.Add(new WeatherUnit
            {
                Week = week[data.showapi_res_body.f7.weekday - 1],
                Date = data.showapi_res_body.f7.day.Substring(6, 2) + "日" + "(" + week[data.showapi_res_body.f7.weekday - 1] + ")",
                DayIcon = string.Format("ms-appx:///Assets/{0}", data.showapi_res_body.f7.day_weather_pic.Substring(32)),
                DayTemperature = data.showapi_res_body.f7.day_air_temperature + "℃",
                NightTemperature = data.showapi_res_body.f7.night_air_temperature + "℃",
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
}

