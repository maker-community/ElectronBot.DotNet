using System.Collections.Concurrent;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.Win32;

namespace ElectronBot.Braincase.Services;
public class ClockDiagnosticService
{
    public event EventHandler<ClockDiagnosticInfo>? ClockDiagnosticInfoResult;

    private readonly ConcurrentQueue<(object Data, TaskCompletionSource<bool> Tcs, CancellationToken Ct)> _queue = new();

    private int _isSending;

    public async Task<bool> InvokeClockViewAsync(object data, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tcs = new TaskCompletionSource<bool>();

        _queue.Enqueue((data, tcs, cancellationToken));

        if (Interlocked.CompareExchange(ref _isSending, 1, 0) == 0)
        {
            _ = Task.Run(InvokeClockViewDataAsync, CancellationToken.None);
        }

        return await tcs.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    private Task InvokeClockViewDataAsync()
    {
        while (_queue.TryDequeue(out var item))
        {
            var (data, tcs, cancellationToken) = item;

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.TrySetCanceled(cancellationToken);
                continue;
            }

            try
            {
                if (data != null)
                {
                    var result = GetClockDiagnosticInfo();
                    ClockDiagnosticInfoResult?.Invoke(this, result);
                    tcs.TrySetResult(true);
                }
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        }

        Interlocked.Exchange(ref _isSending, 0);

        return Task.CompletedTask;
    }
    public ClockDiagnosticInfo GetClockDiagnosticInfo()
    {
        ClockDiagnosticInfo info = new ClockDiagnosticInfo();
        var temp = RefreshTempInfos();
        var cpuUsage = RefreshCpuInfos();
        var memoryUsageText = RefreshRamInfos();


        info.Temperature = temp;
        info.CpuUsage = cpuUsage;
        info.MemoryUsage = Math.Round(memoryUsageText.Item2, 2);
        info.MemoryUsageText = memoryUsageText.Item1;


        return info;
    }



    public void GetDrivesInfos()
    {
        DriveInfo[] allDrives = DriveInfo.GetDrives();
        List<Disque> disques = new List<Disque>();

        foreach (DriveInfo info in allDrives)
        {
            if (info.IsReady == true)
            {
                Console.WriteLine("Disque " + info.Name + " prêt !");
            }
            disques.Add(new Disque(info.Name, info.DriveFormat, FormatBytes(info.TotalSize), FormatBytes(info.AvailableFreeSpace)));
        }

        //listeDisques.ItemsSource = disques;
    }

    private static string FormatBytes(long bytes)
    {
        string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
        int i;
        double dblSByte = bytes;
        for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0;
        }

        return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
    }

    public string RefreshTempInfos()
    {

        double temperature = 0;
        var instanceName = "";
        var tempRet = "";
        try
        {
            if (RuntimeHelper.IsAdminRun())
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                foreach (ManagementObject obj in searcher.Get())
                {
                    temperature = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                    // Convertir °F en °C
                    temperature = (temperature - 2732) / 10.0;
                    instanceName = obj["InstanceName"].ToString();
                }
                tempRet = "温度" + temperature + "°C";
            }
        }
        catch (Exception ex)
        {

        }
        return tempRet;
    }

    public (string, double) RefreshRamInfos()
    {
        var totalValue = GetTotalPhys();
        var total = FormatSize(totalValue);
        var usedValue = GetUsedPhys();
        var used = FormatSize(usedValue);
        var totalUsed = usedValue * 1.0 / totalValue;
        var ret = $"ROM：{used}/{total}";

        return (ret, totalUsed * 100);
    }

    public double RefreshCpuInfos()
    {
        PerformanceCounter cpuCounter = new PerformanceCounter();
        cpuCounter.CategoryName = "Processor";
        cpuCounter.CounterName = "% Processor Time";
        cpuCounter.InstanceName = "_Total";

        dynamic firstVal = cpuCounter.NextValue(); // Cette valeur sera toujours 0 !
        System.Threading.Thread.Sleep(75);
        dynamic val = cpuCounter.NextValue(); // Ici c'est la vraie valeur !

        //// Tourner l'image de l'aiguille
        //RotateTransform rotateTransform = new RotateTransform((val * 2.7f) - 90);
        //imgAiguille.RenderTransform = rotateTransform;

        decimal roundVal = Convert.ToDecimal(val);
        roundVal = Math.Round(roundVal, 2);
        return (double)roundVal;
        //return roundVal + " %";
    }

    // Exemple de fonction pour la RAM
    // ! Pas utilisée dans le cadre de ce cours
    public double GetRAMCounter()
    {
        PerformanceCounter ramCounter = new PerformanceCounter();
        ramCounter.CategoryName = "Memory";
        ramCounter.CounterName = "Available MBytes";

        dynamic firstValue = ramCounter.NextValue();
        System.Threading.Thread.Sleep(75);
        dynamic val = ramCounter.NextValue();

        decimal roundVar = Convert.ToDecimal(val);
        roundVar = Math.Round(roundVar, 2);

        return (double)roundVar;
    }

    /* Travailler avec la mémoire (RAM) */
    #region Fonctions spécifiques à la RAM
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);

    // Structure de l'info de la mémoire
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_INFO
    {
        public uint dwLength; // Taille structure
        public uint dwMemoryLoad; // Utilisation mémoire
        public ulong ullTotalPhys; // Mémoire physique totale
        public ulong ullAvailPhys; // Mémoire physique dispo
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual; // Taille mémoire virtuelle
        public ulong ullAvailVirtual; // Mémoire virtuelle dispo
        public ulong ullAvailExtendedVirtual;
    }

    static string FormatSize(double size)
    {
        double d = (double)size;
        int i = 0;
        while ((d > 1024) && (i < 5))
        {
            d /= 1024;
            i++;
        }
        string[] unit = { "B", "KB", "MB", "GB", "TB" };
        return (string.Format("{0} {1}", Math.Round(d, 2), unit[i]));
    }

    public static MEMORY_INFO GetMemoryStatus()
    {
        MEMORY_INFO mi = new MEMORY_INFO();
        mi.dwLength = (uint)Marshal.SizeOf(mi);
        GlobalMemoryStatusEx(ref mi);
        return mi;
    }

    // Récupération mémoire physique totale dispo
    public static ulong GetAvailPhys()
    {
        MEMORY_INFO mi = GetMemoryStatus();
        return mi.ullAvailPhys;
    }

    // Récupération mémoire utilisée
    public static ulong GetUsedPhys()
    {
        MEMORY_INFO mi = GetMemoryStatus();
        return (mi.ullTotalPhys - mi.ullAvailPhys);
    }

    // Récup la mémoire physique totale
    public static ulong GetTotalPhys()
    {
        MEMORY_INFO mi = GetMemoryStatus();
        return mi.ullTotalPhys;
    }
    #endregion

    public void GetAllSystemInfos()
    {
        SystemInfo si = new SystemInfo();
        //osName.Text = si.GetOsInfos("os");
        //osArch.Text = si.GetOsInfos("arch");
        //procName.Text = si.GetCpuInfos();
        //gpuName.Text = si.GetGpuInfos();
    }
}



public class SystemInfo
{
    // Récupération infos OS
    public string GetOsInfos(string param)
    {
        ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
        foreach (ManagementObject mo in mos.Get())
        {
            switch (param)
            {
                case "os":
                    return mo["Caption"].ToString();
                case "arch":
                    return mo["OSArchitecture"].ToString();
                case "osv":
                    return mo["CSDVersion"].ToString();
            }
        }
        return "";
    }

    // Récup infos CPU
    public string GetCpuInfos()
    {
        RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);

        if (processor_name != null)
        {
            return processor_name.GetValue("ProcessorNameString").ToString();
        }

        return "";
    }

    // Récup infos carte graphique
    public string GetGpuInfos()
    {
        using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
        {
            foreach (ManagementObject obj in searcher.Get())
            {
                Console.WriteLine("Name  -  " + obj["Name"]);
                Console.WriteLine("DeviceID  -  " + obj["DeviceID"]);
                Console.WriteLine("AdapterRAM  -  " + obj["AdapterRAM"]);
                Console.WriteLine("AdapterDACType  -  " + obj["AdapterDACType"]);
                Console.WriteLine("Monochrome  -  " + obj["Monochrome"]);
                Console.WriteLine("InstalledDisplayDrivers  -  " + obj["InstalledDisplayDrivers"]);
                Console.WriteLine("DriverVersion  -  " + obj["DriverVersion"]);
                Console.WriteLine("VideoProcessor  -  " + obj["VideoProcessor"]);
                Console.WriteLine("VideoArchitecture  -  " + obj["VideoArchitecture"]);
                Console.WriteLine("VideoMemoryType  -  " + obj["VideoMemoryType"]);

                return obj["Name"].ToString() + " (Version driver : " + obj["DriverVersion"].ToString() + ")";
            }
        }
        return "";
    }

}

public class Disque
{
    private string name;
    private string format;
    private string totalSpace;
    private string freeSpace;

    public Disque(string n, string f, string t, string l)
    {
        name = n;
        format = f;
        totalSpace = t;
        freeSpace = l;
    }

    public override string ToString()
    {
        return name + " (" + format + ") " + freeSpace + " libres / " + totalSpace;
    }
}