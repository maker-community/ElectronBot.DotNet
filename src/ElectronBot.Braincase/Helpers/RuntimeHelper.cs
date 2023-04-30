using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace ElectronBot.Braincase.Helpers;

public class RuntimeHelper
{
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder? packageFullName);

    public static bool IsMSIX
    {
        get
        {
            var length = 0;

            return GetCurrentPackageFullName(ref length, null) != 15700L;
        }
    }


    public static bool IsAdminRun()
    {
        var ret = false;
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        if (principal.IsInRole(WindowsBuiltInRole.Administrator))
        {
            // 当前正在以管理员权限运行。
            ret = true;
        }
        return ret;
    }
}
