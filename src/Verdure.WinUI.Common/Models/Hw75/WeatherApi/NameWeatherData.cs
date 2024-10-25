using System.Runtime.Serialization;

namespace ElectronBot.Braincase.Models.Name
{
    #region 序列化
    [DataContract]
    public class NameWeatherData
    {
        [DataMember]
        public int showapi_res_code { get; set; }
        [DataMember]
        public string showapi_res_error { get; set; }
        [DataMember]
        public Showapi_Res_Body showapi_res_body { get; set; }
    }
    [DataContract]
    public class Showapi_Res_Body
    {
        [DataMember]
        public F6 f6 { get; set; }
        [DataMember]
        public F7 f7 { get; set; }
        [DataMember]
        public string time { get; set; }
        [DataMember]
        public int ret_code { get; set; }
        [DataMember]
        public Now now { get; set; }
        [DataMember]
        public Cityinfo cityInfo { get; set; }
        [DataMember]
        public F1 f1 { get; set; }
        [DataMember]
        public F3 f3 { get; set; }
        [DataMember]
        public F2 f2 { get; set; }
        [DataMember]
        public F5 f5 { get; set; }
        [DataMember]
        public bool showapi_treemap { get; set; }
        [DataMember]
        public F4 f4 { get; set; }

    }

    [DataContract]
    public class F6
    {
        [DataMember]
        public string day_weather { get; set; }
        [DataMember]
        public string night_weather { get; set; }
        [DataMember]
        public string night_weather_code { get; set; }
        [DataMember]
        public Index index { get; set; }
        [DataMember]
        public string jiangshui { get; set; }
        [DataMember]
        public string air_press { get; set; }
        [DataMember]
        public string night_wind_power { get; set; }
        [DataMember]
        public string day_wind_power { get; set; }
        [DataMember]
        public string day_weather_code { get; set; }
        [DataMember]
        public string sun_begin_end { get; set; }
        [DataMember]
        public string ziwaixian { get; set; }
        [DataMember]
        public string day_weather_pic { get; set; }
        [DataMember]
        public int weekday { get; set; }
        [DataMember]
        public string night_air_temperature { get; set; }
        [DataMember]
        public string day_air_temperature { get; set; }
        [DataMember]
        public string day_wind_direction { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string night_weather_pic { get; set; }
        [DataMember]
        public string night_wind_direction { get; set; }
    }

    [DataContract]
    public class Index
    {
        [DataMember]
        public Yh yh { get; set; }
        [DataMember]
        public Ls ls { get; set; }
        [DataMember]
        public Clothes clothes { get; set; }
        [DataMember]
        public Dy dy { get; set; }
        [DataMember]
        public Sports sports { get; set; }
        [DataMember]
        public Travel travel { get; set; }
        [DataMember]
        public Beauty beauty { get; set; }
        [DataMember]
        public Xq xq { get; set; }
        [DataMember]
        public Hc hc { get; set; }
        [DataMember]
        public Zs zs { get; set; }
        [DataMember]
        public Cold cold { get; set; }
        [DataMember]
        public Gj gj { get; set; }
        [DataMember]
        public Comfort comfort { get; set; }
        [DataMember]
        public Uv uv { get; set; }
        [DataMember]
        public Cl cl { get; set; }
        [DataMember]
        public Glass glass { get; set; }
        [DataMember]
        public Aqi aqi { get; set; }
        [DataMember]
        public Ac ac { get; set; }
        [DataMember]
        public Wash_Car wash_car { get; set; }
        [DataMember]
        public Mf mf { get; set; }
        [DataMember]
        public Ag ag { get; set; }
        [DataMember]
        public Pj pj { get; set; }
        [DataMember]
        public Nl nl { get; set; }
        [DataMember]
        public Pk pk { get; set; }
    }

    [DataContract]
    public class Yh
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ls
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Clothes
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Dy
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Sports
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Travel
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Beauty
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Xq
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Hc
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Zs
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cold
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Gj
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Comfort
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Uv
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cl
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Glass
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Aqi
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ac
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Wash_Car
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Mf
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ag
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pj
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Nl
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pk
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }

    [DataContract]
    public class F7
    {
        [DataMember]
        public string day_weather { get; set; }
        [DataMember]
        public string night_weather { get; set; }
        [DataMember]
        public string night_weather_code { get; set; }
        [DataMember]
        public Index1 index { get; set; }
        [DataMember]
        public string air_press { get; set; }
        [DataMember]
        public string jiangshui { get; set; }
        [DataMember]
        public string night_wind_power { get; set; }
        [DataMember]
        public string day_wind_power { get; set; }
        [DataMember]
        public string day_weather_code { get; set; }
        [DataMember]
        public string sun_begin_end { get; set; }
        [DataMember]
        public string ziwaixian { get; set; }
        [DataMember]
        public string day_weather_pic { get; set; }
        [DataMember]
        public int weekday { get; set; }
        [DataMember]
        public string night_air_temperature { get; set; }
        [DataMember]
        public string day_air_temperature { get; set; }
        [DataMember]
        public string day_wind_direction { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string night_weather_pic { get; set; }
        [DataMember]
        public string night_wind_direction { get; set; }
    }

    [DataContract]
    public class Index1
    {
        [DataMember]
        public Yh1 yh { get; set; }
        [DataMember]
        public Ls1 ls { get; set; }
        [DataMember]
        public Clothes1 clothes { get; set; }
        [DataMember]
        public Dy1 dy { get; set; }
        [DataMember]
        public Sports1 sports { get; set; }
        [DataMember]
        public Travel1 travel { get; set; }
        [DataMember]
        public Beauty1 beauty { get; set; }
        [DataMember]
        public Xq1 xq { get; set; }
        [DataMember]
        public Hc1 hc { get; set; }
        [DataMember]
        public Zs1 zs { get; set; }
        [DataMember]
        public Cold1 cold { get; set; }
        [DataMember]
        public Gj1 gj { get; set; }
        [DataMember]
        public Comfort1 comfort { get; set; }
        [DataMember]
        public Uv1 uv { get; set; }
        [DataMember]
        public Cl1 cl { get; set; }
        [DataMember]
        public Glass1 glass { get; set; }
        [DataMember]
        public Aqi1 aqi { get; set; }
        [DataMember]
        public Ac1 ac { get; set; }
        [DataMember]
        public Wash_Car1 wash_car { get; set; }
        [DataMember]
        public Mf1 mf { get; set; }
        [DataMember]
        public Ag1 ag { get; set; }
        [DataMember]
        public Pj1 pj { get; set; }
        [DataMember]
        public Nl1 nl { get; set; }
        [DataMember]
        public Pk1 pk { get; set; }
    }
    [DataContract]
    public class Yh1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ls1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Clothes1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Dy1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Sports1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Travel1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Beauty1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Xq1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Hc1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Zs1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cold1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Gj1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Uv1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Comfort1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cl1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Wash_Car1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ac1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Aqi1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Glass1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Mf1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ag1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Nl1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pj1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pk1
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Now
    {
        [DataMember]
        public Aqidetail aqiDetail { get; set; }
        [DataMember]
        public string weather_code { get; set; }
        [DataMember]
        public string temperature_time { get; set; }
        [DataMember]
        public string wind_direction { get; set; }
        [DataMember]
        public string wind_power { get; set; }
        [DataMember]
        public string sd { get; set; }
        [DataMember]
        public string aqi { get; set; }
        [DataMember]
        public string weather { get; set; }
        [DataMember]
        public string weather_pic { get; set; }
        [DataMember]
        public string temperature { get; set; }
    }
    [DataContract]
    public class Aqidetail
    {
        [DataMember]
        public string co { get; set; }
        [DataMember]
        public string num { get; set; }
        [DataMember]
        public string so2 { get; set; }
        [DataMember]
        public string area { get; set; }
        [DataMember]
        public string o3 { get; set; }
        [DataMember]
        public string no2 { get; set; }
        [DataMember]
        public string quality { get; set; }
        [DataMember]
        public string aqi { get; set; }
        [DataMember]
        public string pm10 { get; set; }
        [DataMember]
        public string pm2_5 { get; set; }
        [DataMember]
        public string o3_8h { get; set; }
        [DataMember]
        public string primary_pollutant { get; set; }
    }

    [DataContract]
    public class Cityinfo
    {
        [DataMember]
        public string c6 { get; set; }
        [DataMember]
        public string c5 { get; set; }
        [DataMember]
        public string c4 { get; set; }
        [DataMember]
        public string c3 { get; set; }
        [DataMember]
        public string c9 { get; set; }
        [DataMember]
        public string c8 { get; set; }
        [DataMember]
        public string c7 { get; set; }
        [DataMember]
        public string c17 { get; set; }
        [DataMember]
        public string c16 { get; set; }
        [DataMember]
        public string c1 { get; set; }
        [DataMember]
        public string c2 { get; set; }
        [DataMember]
        public string c11 { get; set; }
        [DataMember]
        public float longitude { get; set; }
        [DataMember]
        public string c10 { get; set; }
        [DataMember]
        public float latitude { get; set; }
        [DataMember]
        public string c12 { get; set; }
        [DataMember]
        public string c15 { get; set; }
    }

    [DataContract]
    public class F1
    {
        [DataMember]
        public string day_weather { get; set; }
        [DataMember]
        public string night_weather { get; set; }
        [DataMember]
        public string night_weather_code { get; set; }
        [DataMember]
        public Index2 index { get; set; }
        [DataMember]
        public string jiangshui { get; set; }
        [DataMember]
        public string air_press { get; set; }
        [DataMember]
        public string night_wind_power { get; set; }
        [DataMember]
        public string day_wind_power { get; set; }
        [DataMember]
        public string day_weather_code { get; set; }
        [DataMember]
        public string sun_begin_end { get; set; }
        [DataMember]
        public string ziwaixian { get; set; }
        [DataMember]
        public string day_weather_pic { get; set; }
        [DataMember]
        public int weekday { get; set; }
        [DataMember]
        public string night_air_temperature { get; set; }
        [DataMember]
        public string day_air_temperature { get; set; }
        [DataMember]
        public string day_wind_direction { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string night_weather_pic { get; set; }
        [DataMember]
        public string night_wind_direction { get; set; }
    }
    [DataContract]
    public class Index2
    {
        [DataMember]
        public Yh2 yh { get; set; }
        [DataMember]
        public Ls2 ls { get; set; }
        [DataMember]
        public Clothes2 clothes { get; set; }
        [DataMember]
        public Dy2 dy { get; set; }
        [DataMember]
        public Sports2 sports { get; set; }
        [DataMember]
        public Travel2 travel { get; set; }
        [DataMember]
        public Beauty2 beauty { get; set; }
        [DataMember]
        public Xq2 xq { get; set; }
        [DataMember]
        public Hc2 hc { get; set; }
        [DataMember]
        public Zs2 zs { get; set; }
        [DataMember]
        public Cold2 cold { get; set; }
        [DataMember]
        public Gj2 gj { get; set; }
        [DataMember]
        public Comfort2 comfort { get; set; }
        [DataMember]
        public Uv2 uv { get; set; }
        [DataMember]
        public Cl2 cl { get; set; }
        [DataMember]
        public Glass2 glass { get; set; }
        [DataMember]
        public Aqi2 aqi { get; set; }
        [DataMember]
        public Ac2 ac { get; set; }
        [DataMember]
        public Wash_Car2 wash_car { get; set; }
        [DataMember]

        public Mf2 mf { get; set; }
        [DataMember]
        public Ag2 ag { get; set; }
        [DataMember]
        public Pj2 pj { get; set; }
        [DataMember]
        public Nl2 nl { get; set; }
        [DataMember]
        public Pk2 pk { get; set; }
    }
    [DataContract]
    public class Yh2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ls2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Clothes2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Dy2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Sports2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Travel2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Beauty2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Xq2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Hc2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Zs2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cold2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Gj2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Comfort2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Uv2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cl2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Glass2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Aqi2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ac2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Wash_Car2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Mf2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ag2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pj2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Nl2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pk2
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }

    [DataContract]
    public class F3
    {
        [DataMember]
        public string day_weather { get; set; }
        [DataMember]
        public string night_weather { get; set; }
        [DataMember]
        public string night_weather_code { get; set; }
        [DataMember]
        public Index3 index { get; set; }
        [DataMember]
        public string air_press { get; set; }
        [DataMember]
        public string jiangshui { get; set; }
        [DataMember]
        public string night_wind_power { get; set; }
        [DataMember]
        public string day_wind_power { get; set; }
        [DataMember]
        public string day_weather_code { get; set; }
        [DataMember]
        public string sun_begin_end { get; set; }
        [DataMember]
        public string ziwaixian { get; set; }
        [DataMember]
        public string day_weather_pic { get; set; }
        [DataMember]
        public int weekday { get; set; }
        [DataMember]
        public string night_air_temperature { get; set; }
        [DataMember]
        public string day_air_temperature { get; set; }
        [DataMember]
        public string day_wind_direction { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string night_weather_pic { get; set; }
        [DataMember]
        public string night_wind_direction { get; set; }
    }
    [DataContract]
    public class Index3
    {
        [DataMember]
        public Yh3 yh { get; set; }
        [DataMember]
        public Ls3 ls { get; set; }
        [DataMember]
        public Clothes3 clothes { get; set; }
        [DataMember]
        public Dy3 dy { get; set; }
        [DataMember]
        public Sports3 sports { get; set; }
        [DataMember]
        public Travel3 travel { get; set; }
        [DataMember]
        public Beauty3 beauty { get; set; }
        [DataMember]
        public Xq3 xq { get; set; }
        [DataMember]
        public Hc3 hc { get; set; }
        [DataMember]
        public Zs3 zs { get; set; }
        [DataMember]
        public Cold3 cold { get; set; }
        [DataMember]
        public Gj3 gj { get; set; }
        [DataMember]
        public Comfort3 comfort { get; set; }
        [DataMember]
        public Uv3 uv { get; set; }
        [DataMember]
        public Cl3 cl { get; set; }
        [DataMember]
        public Glass3 glass { get; set; }
        [DataMember]
        public Aqi3 aqi { get; set; }
        [DataMember]
        public Ac3 ac { get; set; }
        [DataMember]
        public Wash_Car3 wash_car { get; set; }
        [DataMember]
        public Mf3 mf { get; set; }
        [DataMember]
        public Ag3 ag { get; set; }
        [DataMember]
        public Pj3 pj { get; set; }
        [DataMember]
        public Nl3 nl { get; set; }
        [DataMember]
        public Pk3 pk { get; set; }

    }
    [DataContract]
    public class Yh3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ls3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Clothes3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Dy3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Sports3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Travel3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Beauty3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Xq3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Hc3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Zs3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cold3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Gj3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Comfort3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Uv3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cl3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Glass3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Aqi3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ac3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Wash_Car3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Mf3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ag3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pj3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Nl3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pk3
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }

    [DataContract]
    public class F2
    {
        [DataMember]
        public string day_weather { get; set; }
        [DataMember]
        public string night_weather { get; set; }
        [DataMember]
        public string night_weather_code { get; set; }
        [DataMember]
        public Index4 index { get; set; }
        [DataMember]
        public string air_press { get; set; }
        [DataMember]
        public string jiangshui { get; set; }
        [DataMember]
        public string night_wind_power { get; set; }
        [DataMember]
        public string day_wind_power { get; set; }
        [DataMember]
        public string day_weather_code { get; set; }
        [DataMember]
        public string sun_begin_end { get; set; }
        [DataMember]
        public string ziwaixian { get; set; }
        [DataMember]
        public string day_weather_pic { get; set; }
        [DataMember]
        public int weekday { get; set; }
        [DataMember]
        public string night_air_temperature { get; set; }
        [DataMember]
        public string day_air_temperature { get; set; }
        [DataMember]
        public string day_wind_direction { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string night_weather_pic { get; set; }
        [DataMember]
        public string night_wind_direction { get; set; }
    }
    [DataContract]
    public class Index4
    {
        [DataMember]
        public Yh4 yh { get; set; }
        [DataMember]
        public Ls4 ls { get; set; }
        [DataMember]
        public Clothes4 clothes { get; set; }
        [DataMember]
        public Dy4 dy { get; set; }
        [DataMember]
        public Sports4 sports { get; set; }
        [DataMember]
        public Travel4 travel { get; set; }
        [DataMember]
        public Beauty4 beauty { get; set; }
        [DataMember]
        public Xq4 xq { get; set; }
        [DataMember]
        public Hc4 hc { get; set; }
        [DataMember]
        public Zs4 zs { get; set; }
        [DataMember]
        public Cold4 cold { get; set; }
        [DataMember]
        public Gj4 gj { get; set; }
        [DataMember]
        public Comfort4 comfort { get; set; }
        [DataMember]
        public Uv4 uv { get; set; }
        [DataMember]
        public Cl4 cl { get; set; }
        [DataMember]
        public Glass4 glass { get; set; }
        [DataMember]
        public Aqi4 aqi { get; set; }
        [DataMember]
        public Ac4 ac { get; set; }
        [DataMember]
        public Wash_Car4 wash_car { get; set; }
        [DataMember]
        public Mf4 mf { get; set; }
        [DataMember]
        public Ag4 ag { get; set; }
        [DataMember]
        public Pj4 pj { get; set; }
        [DataMember]
        public Nl4 nl { get; set; }
        [DataMember]
        public Pk4 pk { get; set; }
    }
    [DataContract]
    public class Yh4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ls4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Clothes4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Dy4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Sports4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Travel4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Beauty4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Xq4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Hc4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Zs4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cold4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Gj4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Comfort4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Uv4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cl4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Glass4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Aqi4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ac4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Wash_Car4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Mf4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ag4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pj4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Nl4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pk4
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }

    [DataContract]
    public class F5
    {
        [DataMember]
        public string day_weather { get; set; }
        [DataMember]
        public string night_weather { get; set; }
        [DataMember]
        public string night_weather_code { get; set; }
        [DataMember]
        public Index5 index { get; set; }
        [DataMember]
        public string air_press { get; set; }
        [DataMember]
        public string jiangshui { get; set; }
        [DataMember]
        public string night_wind_power { get; set; }
        [DataMember]
        public string day_wind_power { get; set; }
        [DataMember]
        public string day_weather_code { get; set; }
        [DataMember]
        public string sun_begin_end { get; set; }
        [DataMember]
        public string ziwaixian { get; set; }
        [DataMember]
        public string day_weather_pic { get; set; }
        [DataMember]
        public int weekday { get; set; }
        [DataMember]
        public string night_air_temperature { get; set; }
        [DataMember]
        public string day_air_temperature { get; set; }
        [DataMember]
        public string day_wind_direction { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string night_weather_pic { get; set; }
        [DataMember]
        public string night_wind_direction { get; set; }
    }
    [DataContract]
    public class Index5
    {
        [DataMember]
        public Yh5 yh { get; set; }
        [DataMember]
        public Ls5 ls { get; set; }
        [DataMember]
        public Clothes5 clothes { get; set; }
        [DataMember]
        public Dy5 dy { get; set; }
        [DataMember]
        public Sports5 sports { get; set; }
        [DataMember]
        public Travel5 travel { get; set; }
        [DataMember]
        public Beauty5 beauty { get; set; }
        [DataMember]
        public Xq5 xq { get; set; }
        [DataMember]
        public Hc5 hc { get; set; }
        [DataMember]
        public Zs5 zs { get; set; }
        [DataMember]
        public Cold5 cold { get; set; }
        [DataMember]
        public Gj5 gj { get; set; }
        [DataMember]
        public Comfort5 comfort { get; set; }
        [DataMember]
        public Uv5 uv { get; set; }
        [DataMember]
        public Cl5 cl { get; set; }
        [DataMember]
        public Glass5 glass { get; set; }
        [DataMember]
        public Aqi5 aqi { get; set; }
        [DataMember]
        public Ac5 ac { get; set; }
        [DataMember]
        public Wash_Car5 wash_car { get; set; }
        [DataMember]
        public Mf5 mf { get; set; }
        [DataMember]
        public Ag5 ag { get; set; }
        [DataMember]
        public Pj5 pj { get; set; }
        [DataMember]
        public Nl5 nl { get; set; }
        [DataMember]
        public Pk5 pk { get; set; }
    }
    [DataContract]
    public class Yh5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ls5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Clothes5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Dy5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Sports5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Travel5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Beauty5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Xq5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Hc5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Zs5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cold5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Gj5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Comfort5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Uv5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cl5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Glass5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Aqi5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ac5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Wash_Car5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Mf5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ag5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pj5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Nl5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pk5
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }

    [DataContract]
    public class F4
    {
        [DataMember]
        public string day_weather { get; set; }
        [DataMember]
        public string night_weather { get; set; }
        [DataMember]
        public string night_weather_code { get; set; }
        [DataMember]
        public Index6 index { get; set; }
        [DataMember]
        public string air_press { get; set; }
        [DataMember]
        public string jiangshui { get; set; }
        [DataMember]
        public string night_wind_power { get; set; }
        [DataMember]
        public string day_wind_power { get; set; }
        [DataMember]
        public string day_weather_code { get; set; }
        [DataMember]
        public string sun_begin_end { get; set; }
        [DataMember]
        public string ziwaixian { get; set; }
        [DataMember]
        public string day_weather_pic { get; set; }
        [DataMember]
        public int weekday { get; set; }
        [DataMember]
        public string night_air_temperature { get; set; }
        [DataMember]
        public string day_air_temperature { get; set; }
        [DataMember]
        public string day_wind_direction { get; set; }
        [DataMember]
        public string day { get; set; }
        [DataMember]
        public string night_weather_pic { get; set; }
        [DataMember]
        public string night_wind_direction { get; set; }
    }
    [DataContract]
    public class Index6
    {
        [DataMember]
        public Yh6 yh { get; set; }
        [DataMember]
        public Ls6 ls { get; set; }
        [DataMember]
        public Clothes6 clothes { get; set; }
        [DataMember]
        public Dy6 dy { get; set; }
        [DataMember]
        public Sports6 sports { get; set; }
        [DataMember]
        public Travel6 travel { get; set; }
        [DataMember]
        public Beauty6 beauty { get; set; }
        [DataMember]
        public Xq6 xq { get; set; }
        [DataMember]
        public Hc6 hc { get; set; }
        [DataMember]
        public Zs6 zs { get; set; }
        [DataMember]
        public Cold6 cold { get; set; }
        [DataMember]
        public Gj6 gj { get; set; }
        [DataMember]
        public Comfort6 comfort { get; set; }
        [DataMember]
        public Uv6 uv { get; set; }
        [DataMember]
        public Cl6 cl { get; set; }
        [DataMember]
        public Glass6 glass { get; set; }
        [DataMember]
        public Aqi6 aqi { get; set; }
        [DataMember]
        public Ac6 ac { get; set; }
        [DataMember]
        public Wash_Car6 wash_car { get; set; }
        [DataMember]
        public Mf6 mf { get; set; }
        [DataMember]
        public Ag6 ag { get; set; }
        [DataMember]
        public Pj6 pj { get; set; }
        [DataMember]
        public Nl6 nl { get; set; }
        [DataMember]
        public Pk6 pk { get; set; }
    }
    [DataContract]
    public class Yh6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ls6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Clothes6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Dy6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Sports6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Travel6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Beauty6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Xq6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Hc6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Zs6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cold6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Gj6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Comfort6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Uv6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Cl6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Glass6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Aqi6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ac6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Wash_Car6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Mf6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Ag6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pj6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Nl6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    [DataContract]
    public class Pk6
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string desc { get; set; }
    }
    #endregion
}
