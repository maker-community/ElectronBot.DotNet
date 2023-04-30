using System;
using Windows.System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Diagnostics;
using System.Diagnostics;

namespace ElectronBot.Braincase.Services;

public static class PerformanceInfo
{
    private static readonly Stopwatch stopwatch = new Stopwatch();
    public static SYSTEM_INFO sysInfo = new SYSTEM_INFO();
    private static MEMORYSTATUSEX mem = new MEMORYSTATUSEX();

     static GeneralStatistics generalStatistics = new GeneralStatistics();

    [DllImport("kernel32.dll", SetLastError = false)]
    public static extern void GetNativeSystemInfo(out SYSTEM_INFO Info);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    static PerformanceInfo()
    {
        //mem.dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>();
        //GlobalMemoryStatusEx(mem);
        stopwatch.Start();
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SYSTEM_INFO_UNION
    {
        [FieldOffset(0)]
        public UInt32 OemId;
        [FieldOffset(0)]
        public UInt16 ProcessorArchitecture;
        [FieldOffset(2)]
        public UInt16 Reserved;
    }

    public struct SYSTEM_INFO
    {
        public SYSTEM_INFO_UNION CpuInfo;
        public UInt32 PageSize;
        public UInt32 MinimumApplicationAddress;
        public UInt32 MaximumApplicationAddress;
        public UInt32 ActiveProcessorMask;
        public UInt32 NumberOfProcessors;
        public UInt32 ProcessorType;
        public UInt32 AllocationGranularity;
        public UInt16 ProcessorLevel;
        public UInt16 ProcessorRevision;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        public MEMORYSTATUSEX()
        {
            this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }

    public enum Arch : ushort
    {
        X86 = 0,
        Arm = 5,
        X64 = 9,
        Neutral = 11,
        Arm64 = 12,
        X86OnArm64 = 14,
        Unknown = 65535
    }

    public static GeneralStatistics GetGeneralStatistics()
    {
        GlobalMemoryStatusEx(mem);
        generalStatistics.memoryTotal = mem.ullTotalPhys / 1048576;
        generalStatistics.memoryInUse = (mem.ullTotalPhys - mem.ullAvailPhys) / 1048576;
        return generalStatistics;
    }
}

public class GeneralStatistics
{
    public string processorName;
    public ProcessorArchitecture processorArch;

    public string memoryType;
    public ulong memoryTotal;
    public ulong memoryInUse;
}