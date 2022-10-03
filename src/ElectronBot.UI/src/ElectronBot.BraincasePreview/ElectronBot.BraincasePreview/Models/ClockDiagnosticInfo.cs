using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.BraincasePreview.Models;
public class ClockDiagnosticInfo
{
    public string CpuUsage
    {
        get; set;
    }
    public string TotalMemory
    {
        get; set;
    }
    public string CpuThreadCount
    {
        get; set;
    }
    public string UsedMemory
    {
        get; set;
    }
    public string FreeMemory
    {
        get; set;
    }
    public string SystemArch
    {
        get; set;
    }
    public string TotalStorage
    {
        get; set;
    }
    public string UsedStorage
    {
        get; set;
    }
    public string FreeStorage
    {
        get; set;
    }
}
