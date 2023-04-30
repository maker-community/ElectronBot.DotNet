using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronBot.Braincase.Models;
public class ClockDiagnosticInfo
{
    public double CpuUsage
    {
        get; set;
    }

    public double MemoryUsage
    {
        get; set;
    }

    public string MemoryUsageText
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

    public string Temperature
    {
        get; set;
    }
}
