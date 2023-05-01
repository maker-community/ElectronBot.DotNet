using ElectronBot.Braincase.Models;

namespace Models;
public class EmojisFileManifest
{
    public string Name
    {
        get;
        set;
    } = string.Empty;
    public string NameId
    {
        get;
        set;
    } = string.Empty;
    public string Description
    {
        get;
        set;
    } = string.Empty;
    public bool HasAction
    {
        get;
        set;
    }
    public EmojisType EmojisType
    {
        get;
        set;
    }
}
