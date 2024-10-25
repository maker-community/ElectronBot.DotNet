using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.Models
{
  public  class CityDetail
    {
        public string shengName { get; set; }
        public string difangName { get; set; }
        public string tianqiqingkuang { get; set; }
        public string fengliDengji { get; set; }
        public string fengxiang { get; set; }
        public string wendu { get; set; }
        //public CityDetail GetCopy()
        //{
        //    var other = new CityDetail();
        //    other.name = name;
        //    other.tianqiqingkuang = tianqiqingkuang;
        //    other.wendu = wendu;
        //    other.fengli = fengli;
        //    return other;
        //}
        //public bool Equals(CityDetail other)
        //{
        //    return name == other.name;
        //}

        //public override int GetHashCode()
        //{
        //    return name.GetHashCode();
        //}
    }
}
