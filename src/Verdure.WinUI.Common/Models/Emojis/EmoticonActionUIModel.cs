using Microsoft.UI.Xaml.Media.Imaging;
using Verdure.Braincase.Core.Models.Emojis.Enums;

namespace Verdure.WinUI.Common.Models;

public class EmoticonActionUIModel
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

    public BitmapImage? Avatar
    {
        get; set;
    }

    public string EmojisVideoPath
    {
        get; set;
    } = string.Empty;
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
}