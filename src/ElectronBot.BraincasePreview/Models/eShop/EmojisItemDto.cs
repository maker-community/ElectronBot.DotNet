namespace Models;
public class EmojisItemDto
{
    public string Id
    {
        get; set;
    } = string.Empty;

    public string Name
    {
        get; set;
    } = string.Empty;

    public decimal Price
    {
        get; set;
    }

    public string PictureFileName
    {
        get;
        set;
    } = string.Empty;

    public string PictureFileId
    {
        get; set;
    } = string.Empty;

    public string VideoFileId
    {
        get; set;
    } = string.Empty;
}
