namespace Verdure.Braincase.DataStorage.Collections;
public class EmojisDocument : LiteDBBase
{
    public string NameId
    {
        get; set;
    } = string.Empty;

    public string Name
    {
        get; set;
    } = string.Empty;

    public string Desc
    {
        get; set;
    } = string.Empty;

    public string Avatar
    {
        get; set;
    } = string.Empty;

    public string EmojisVideoPath
    {
        get; set;
    } = string.Empty;

    public string Type
    {
        get; set;
    } = string.Empty;

    public string EmojisActionJson
    {
        get; set;
    } = string.Empty;

    public string EmojisAuthor
    {
        get;
        set;
    } = string.Empty;

    public bool HasAction
    {
        get;
        set;
    }
}
