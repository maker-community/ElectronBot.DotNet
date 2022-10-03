using System.Runtime.InteropServices;
using ElectronBot.BraincasePreview.Models;
using Windows.Storage;

namespace ElectronBot.BraincasePreview.Services;
public class ClockDiagnosticService
{


    public ClockDiagnosticInfo GetClockDiagnosticInfoAsync()
    {
        ClockDiagnosticInfo info = new ClockDiagnosticInfo();

        GeneralStatistics currentStats = PerformanceInfo.GetGeneralStatistics();

        PerformanceInfo.GetNativeSystemInfo(out PerformanceInfo.SYSTEM_INFO sysInfo);

        info.SystemArch = ((PerformanceInfo.Arch)sysInfo.CpuInfo.ProcessorArchitecture).ToString();


       
        info.TotalMemory = Math.Round(currentStats.memoryTotal / 1024.0).ToString() + " GB";

        info.UsedMemory = (currentStats.memoryInUse / 1024).ToString() + " GB used";
        info.FreeMemory = ((currentStats.memoryTotal - currentStats.memoryInUse) / 1024).ToString() + " GB free";

        info.CpuThreadCount = CpuUtil.ProcessorCount + " threads";
 

        info.CpuUsage = Convert.ToInt32(CpuUtil.GetPercentage()) + "%";

        ulong freeBytesAvailable;
        ulong totalNumberOfBytes;
        ulong totalNumberOfFreeBytes;

        // You can only specify a folder path that this app can access, but you can
        // get full disk information from any folder path.
        IStorageFolder appFolder = ApplicationData.Current.LocalFolder;
        GetDiskFreeSpaceEx(appFolder.Path, out freeBytesAvailable, out totalNumberOfBytes, out totalNumberOfFreeBytes);
        info.TotalStorage = $"{totalNumberOfBytes / 1073741824} GB";
        info.UsedStorage = $"{(totalNumberOfBytes - freeBytesAvailable) / 1073741824} GB used";
        info.FreeStorage = $"{freeBytesAvailable / 1073741824} GB free";
        return info;
    }
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
}
