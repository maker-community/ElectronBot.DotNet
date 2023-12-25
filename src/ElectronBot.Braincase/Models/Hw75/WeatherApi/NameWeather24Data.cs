using System.Runtime.Serialization;

namespace ElectronBot.Braincase.Models.Name24
{
    [DataContract]
    public class NameWeather24Data
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
        public int ret_code { get; set; }
        [DataMember]
        public string area { get; set; }
        [DataMember]
        public string areaid { get; set; }
        [DataMember]
        public Hourlist[] hourList { get; set; }
    }
    [DataContract]
    public class Hourlist
    {
        [DataMember]
        public string weather_code { get; set; }
        [DataMember]
        public string time { get; set; }
        [DataMember]
        public string wind_direction { get; set; }
        [DataMember]
        public string wind_power { get; set; }
        [DataMember]
        public string weather { get; set; }
        [DataMember]
        public string temperature { get; set; }
    }

}
