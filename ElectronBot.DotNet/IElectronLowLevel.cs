using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.DotNet
{
    public interface IElectronLowLevel
    {
        bool Connect();
        bool Disconnect();
        bool Sync();
        void SetImageSrc(byte[] data);
        void SetExtraData(ref byte[] data, int len = 32);

        void SetJointAngles(int j1, int j2, int j3,int j4, int j5, int j6, bool enable = false);
        List<int> GetJointAngles();
        byte[] GetExtraData();
    }
}
