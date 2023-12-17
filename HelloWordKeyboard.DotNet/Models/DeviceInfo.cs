namespace HelloWordKeyboard.DotNet.Models;

public class DeviceInfo
{
    public string DeviceName { get; set; } = string.Empty;
    public int Vid { get; set; }
    public int Pid { get; set; }
    public int Usage { get; set; }
}