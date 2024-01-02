using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.Models
{
    public class CityInfo : IEquatable<CityInfo>
    {
        public string AREAID { get; set; }
        public string NAMEEN { get; set; }
        public string NAMECN { get; set; }
        public string DISTRICTEN { get; set; }
        public string DISTRICTCN { get; set; }
        public string PROVEN { get; set; }
        public string PROVCN { get; set; }
        public string NATIONEN { get; set; }
        public string NATIONCN { get; set; }
        public CityInfo GetCopy()
        {
            var other = new CityInfo();
            other.AREAID = AREAID;
            other.NAMEEN = NAMEEN;
            other.NAMECN = NAMECN;
            other.DISTRICTEN = DISTRICTEN;
            other.DISTRICTCN = DISTRICTCN;
            other.PROVEN = PROVEN;
            other.PROVCN = PROVCN;
            other.NATIONEN = NATIONEN;
            other.NATIONCN = NATIONCN;
            return other;
        }

        public bool Equals(CityInfo other)
        {
            return NAMECN == other.NAMECN;
        }

        public override int GetHashCode()
        {
            return NAMECN.GetHashCode();
        }
    }
}
