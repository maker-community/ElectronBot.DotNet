namespace HelloWordKeyboard.DotNet.Models;

public class DeviceInfo
{
    public string DeviceName { get; set; } = string.Empty;
    public string Vid { get; set; } = string.Empty;
    public string Pid { get; set; } = string.Empty;
    public int Usage { get; set; }
}