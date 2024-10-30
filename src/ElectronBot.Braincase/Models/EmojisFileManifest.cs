using Verdure.Braincase.Core.Models.Emojis.Enums;

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
    public int EmojisType
    {
        get;
        set;
    }

    public string Type
    {
        get;
        set;
    } = EmojisFileType.Custom;
}
