namespace Verdure.ElectronBot.Core.Models;
public class EmoticonAction
{
    public string NameId
    {
        get; set;
    } = "";

    public string Name
    {
        get; set;
    } = "";

    public string Desc
    {
        get; set;
    } = "";

    public string Avatar
    {
        get; set;
    } = "";

    public string EmojisVideoPath
    {
        get; set;
    } = "";
    public EmojisType EmojisType
    {
        get; set;
    }
}

public enum EmojisType
{
    Default = 1,
    Custom = 2
}