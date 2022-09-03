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
    public string DiskUsage
    {
        get; set;
    }
    public string MemoryUsage
    {
        get; set;
    }
}
