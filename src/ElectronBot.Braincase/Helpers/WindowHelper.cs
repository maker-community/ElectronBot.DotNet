using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ElectronBot.Braincase.Models;

namespace ElectronBot.Braincase.Helpers;
public class WindowHelper
{
    [DllImport("User32.dll")]
    public static extern IntPtr GetForegroundWindow();     //获取活动窗口句柄

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);   //获取线程ID

    [DllImport("user32.dll ")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    //根据任务栏应用程序显示的名称找窗口的名称
    [DllImport("User32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    // 引入库
    [DllImport("kernel32.dll")]
    public static extern int WinExec(string programPath, int operType);

    //[DllImport("User32")]
    //public static extern bool OpenClipboard(IntPtr hWndNewOwner);

    //[DllImport("User32")]
    //public static extern bool CloseClipboard();

    //[DllImport("User32")]
    //public static extern bool EmptyClipboard();

    //[DllImport("User32")]
    //public static extern bool IsClipboardFormatAvailable(int format);

    //[DllImport("User32")]
    //public static extern IntPtr GetClipboardData(int uFormat);

    //[DllImport("User32", CharSet = CharSet.Unicode)]
    //public static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);

    //[DllImport("user32.dll", SetLastError = true)]
    //public static extern bool AddClipboardFormatListener(IntPtr hWnd);

    //[DllImport("user32.dll", SetLastError = true)]
    //public static extern bool RemoveClipboardFormatListener(IntPtr hWnd);

    private static WindowHelper? _instance;
    public static WindowHelper Instance => _instance ??= new WindowHelper();

    /// <summary>
    /// 获取当前活动窗口
    /// </summary>
    /// <returns></returns>
    //public static WindowProcessInfo GetActiveWindowInfo()
    //{
    //    IntPtr hWnd = GetForegroundWindow();    //获取活动窗口句柄            
    //    int calcTD = GetWindowThreadProcessId(hWnd, out int calcID); //calcID --> 进程ID  calcTD --> 线程ID
    //    Process process = Process.GetProcessById(calcID);
    //    WindowProcessInfo info = new()
    //    {
    //        ProcessName = process.ProcessName,
    //        CalcID = calcID,
    //        CalcTD = calcTD,
    //        ProcessPath = process.MainModule.FileName
    //    };
    //    return info;
    //}

    private const int SW_RESTORE = 9;

    /// <summary>
    /// 开启应用
    /// </summary>
    /// <param name="appPath"></param>
    public async Task StartProcess(string appPath)
    {
        Process[] exitProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appPath));
        if (exitProcesses.Length > 0)
        {
        }
        else
        {
            try
            {
                Process.Start(appPath);
            }
            catch
            {
                //创建启动对象
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = appPath,
                    //设置启动动作,确保以管理员身份运行
                    Verb = "runas"
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch
                {

                }
            }

        }
    }

    /// <summary>
    /// 将指定程序置顶
    /// </summary>
    /// <param name="appPath"></param>
    /// <returns></returns>
    public async Task ActiveProcess(string appPath)
    {
        Process[] exitProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appPath));
        if (exitProcesses.Length > 0)
        {
            foreach (Process exitProcesse in exitProcesses)
            {
                // 将应用程序置顶
                IntPtr hWnd = exitProcesse.MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, SW_RESTORE); //将窗口还原，如果不用此方法，缩小的窗口不能激活
                    SetForegroundWindow(hWnd);// 将指定的窗口选中(激活)
                }
            }
        }
        else
        {
            _ = Process.Start(appPath);
        }
    }
}
