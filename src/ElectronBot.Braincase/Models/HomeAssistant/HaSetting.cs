namespace Models;
public class HaSetting
{
    public string BaseUrl
    {
        get; set;
    } = "http://localhost:8123";

    public string HaToken
    {
        get; set;
    } = "";

    public bool IsSessionSwitchEnabled
    {
        get;
        set;
    }
}
