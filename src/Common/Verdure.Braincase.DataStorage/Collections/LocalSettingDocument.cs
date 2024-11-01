namespace Verdure.Braincase.DataStorage.Collections;
public class LocalSettingDocument : LiteDBBase
{
    public string Key
    {
        get; set;
    } = string.Empty;

    public string Value
    {
        get; set;
    } = string.Empty;
}
