using Verdure.Braincase.Core.Models.Emojis.Enums;

namespace Verdure.Braincase.Core.Models;
public class EmoticonActionModel : IDisposable
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

    public Stream? Avatar
    {
        get; set;
    }

    public string EmojisVideoPath
    {
        get; set;
    } = string.Empty;

    public Stream? EmojisVideo
    {
        get; set;
    }
    public string Type
    {
        get; set;
    } = EmojisFileType.Default;

    public string EmojisActionPath
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

    public void Dispose()
    {
        Avatar?.Dispose();
        EmojisVideo?.Dispose();
    }
}