using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.Models
{
   public class Weather_Displayed
    {
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 实时天气
        /// </summary>
        public WeatherUnit Now { get; set; }

        /// <summary>
        /// 8小时级别预报，时间只显示小时
        /// </summary>
        public List<WeatherUnit> HourlyForecates_8 { get; set; }

        /// <summary>
        /// 48小时级别预报，时间显示完整日期
        /// </summary>
        public List<WeatherUnit> HourlyForecates_48 { get; set; }


        /// <summary>
        /// 24小时级别预报，时间显示完整日期
        /// </summary>
        public List<Hour24> HourlyForecates_24 { get; set; }

        /// <summary>
        /// 未来7天预报
        /// </summary>
        public List<WeatherUnit> DailyForecates { get; set; }

        /// <summary>
        /// 当天的生活贴士
        /// </summary>
        public List<Suggestion> Suggestions { get; set; }
    }

    public class WeatherUnit
    {/// <summary>
    /// 地区省会
    /// </summary>
        public string Province{ get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        ///
        public string Temperature { get; set; }

        /// <summary>
        /// 天气概况
        /// </summary>
        public string Skycon { get; set; }
        /// <summary>
        /// 气压
        /// </summary>
        public string Pressure{ get; set; }
        /// <summary>
        /// 空气质量
        /// </summary>
        public string Aqi { get; set; }

        /// <summary>
        /// 相对湿度
        /// </summary>
        public string Humidity { get; set; }

        /// <summary>
        /// PM2.5
        /// </summary>
        public string Pm25 { get; set; }

        /// <summary>
        /// 降水强度
        /// </summary>
        public string Precipitation { get; set; }

        /// <summary>
        /// 风
        /// </summary>
        public string Wind { get; set; }
        /// <summary>
        /// 空气湿度
        /// </summary>
        public string Wind_sd { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// icon图标路径
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 完整的日期和时间
        /// </summary>
        public DateTime FullDateTime { get; set; }

        /// <summary>
        /// 日出时间 hh：mm
        /// </summary>
        public string SunRise { get; set; }

        /// <summary>
        /// 日落时间 hh：mm
        /// </summary>
        public string SunSet { get; set; }
        /// <summary>
        /// 未来七天日期
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// 周几
        /// </summary>
        public string Week { get; set; }
        /// <summary>
        /// 白天图片
        /// </summary>
        public string DayIcon { get; set; }
        /// <summary>
        ///夜晚图片 
        /// </summary>
        public string NightIcon { get; set; }
        /// <summary>
        ///夜晚温度 
        /// </summary>
        public string DayTemperature { get; set; }
        /// <summary>
        /// 夜晚温度
        /// </summary>
        public string NightTemperature { get; set; }
    }

    public class Suggestion
    {
        /// <summary>
        /// 贴士名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 概述
        /// </summary>
        public string Brf { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        public string Txt { get; set; }

        /// <summary>
        /// LightIcon文件名
        /// </summary>
        public string LightIcon { get; set; }
        /// <summary>
        /// DarkIcon文件名
        /// </summary>
        public string DarkIcon { get; set; }
    }

}
