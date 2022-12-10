using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using ElectronBot.BraincasePreview.Models;
using Microsoft.Win32;
using Windows.Storage;
using static ElectronBot.BraincasePreview.Services.PerformanceInfo;

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


    public void RefreshNetworkInfos()
    {
        if (!NetworkInterface.GetIsNetworkAvailable())
            return;

        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface ni in interfaces)
        {
            // Envoyé
            if (ni.GetIPv4Statistics().BytesSent > 0)
            {
            }
                //netMont.Text = ni.GetIPv4Statistics().BytesSent / 1000 + " KB";
            // Reçu
            if (ni.GetIPv4Statistics().BytesReceived > 0)
            {

            }
                //netDes.Text = ni.GetIPv4Statistics().BytesReceived / 1000 + " KB";
        }
    }


    public void RefreshTempInfos()
    {

        Double temperature = 0;
        String instanceName = "";

        try
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            foreach (ManagementObject obj in searcher.Get())
            {
                temperature = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                // Convertir °F en °C
                temperature = (temperature - 2732) / 10.0;
                instanceName = obj["InstanceName"].ToString();
            }
            //temp.Text = temperature + "°C";
        }
        catch (Exception ex)
        {

        }
    }

    //public void RefreshRamInfos()
    //{
    //    ramTotal.Text = "Total : " + FormatSize(GetTotalPhys());
    //    ramUsed.Text = "Used : " + FormatSize(GetUsedPhys());
    //    ramFree.Text = "Free : " + FormatSize(GetAvailPhys());

    //    string[] maxVal = FormatSize(GetTotalPhys()).Split(' ');
    //    barRam.Maximum = float.Parse(maxVal[0]);
    //    string[] memVal = FormatSize(GetUsedPhys()).Split(' ');
    //    barRam.Value = float.Parse(memVal[0]);
    //}

    public string RefreshCpuInfos()
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

        return roundVal + " %";
    }

    // Exemple de fonction pour la RAM
    // ! Pas utilisée dans le cadre de ce cours
    public object getRAMCounter()
    {
        PerformanceCounter ramCounter = new PerformanceCounter();
        ramCounter.CategoryName = "Memory";
        ramCounter.CounterName = "Available MBytes";

        dynamic firstValue = ramCounter.NextValue();
        System.Threading.Thread.Sleep(75);
        dynamic val = ramCounter.NextValue();

        decimal roundVar = Convert.ToDecimal(val);
        roundVar = Math.Round(roundVar, 2);

        return roundVar;
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