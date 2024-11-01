namespace Models;
public class LaunchAppConfig
{
    public string VoiceText
    {
        get; set;
    } = string.Empty;

    public string AppNameText
    {
        get; set;
    } = string.Empty;

    public string? Win32Path
    {
        get; set;
    }

    public bool IsMsix
    {
        get; set;
    }
}
