using CommunityToolkit.Mvvm.ComponentModel;
using Verdure.Braincase.Core.Models.Emojis.Enums;

namespace Verdure.Braincase.Core.Models;
public class EmoticonAction : ObservableRecipient
{
    private bool _hasAction;
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
    public string Type
    {
        get; set;
    } = EmojisFileType.Default;
    public string EmojisActionPath
    {
        get; set;
    } = "";

    public string EmojisActionContent
    {
        get; set;
    } = "";

    public string EmojisAuthor
    {
        get;
        set;
    } = "";
    public bool HasAction
    {
        get => _hasAction;
        set => SetProperty(ref _hasAction, value);
    }
}