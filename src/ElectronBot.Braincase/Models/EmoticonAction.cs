using CommunityToolkit.Mvvm.ComponentModel;

namespace ElectronBot.Braincase.Models;
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
    public EmojisType EmojisType
    {
        get; set;
    }
    public string EmojisActionPath
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

public enum EmojisType
{
    Default = 1,
    Custom = 2
}