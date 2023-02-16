namespace ElectronBot.BraincasePreview.Models;
public class CustomClockTitleConfig
{
    public string CustomClockTitle
    {
        get; set;
    } = "你好世界☺️";
    public int CustomClockTitleFontsize
    {
        get; set;
    } = 16;

    public string ChatGPTSessionKey
    {
        get; set;
    } = "";

    public string TuringAppkey
    {
        get; set;
    } = string.Empty;

    public string TuringUserId
    {
        get; set;
    } = string.Empty;

    /// <summary>
    /// 手势识别回复文本
    /// </summary>
    public string AnswerText
    {
        get; set;
    } = string.Empty;

}
