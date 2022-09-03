using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronBot.BraincasePreview.Models;
using Windows.System.Diagnostics;

namespace ElectronBot.BraincasePreview.Services;
public class ClockDiagnosticService
{
    

    public ClockDiagnosticInfo GetClockDiagnosticInfoAsync()
    {
        return new ClockDiagnosticInfo
        {
             CpuUsage = SystemDiagnosticInfo.GetForCurrentSystem().CpuUsage.ToString(),
             MemoryUsage = SystemDiagnosticInfo.GetForCurrentSystem().MemoryUsage.ToString(),
        };
    }
}
