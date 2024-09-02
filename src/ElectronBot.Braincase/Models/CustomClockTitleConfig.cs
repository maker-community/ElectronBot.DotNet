namespace ElectronBot.Braincase.Models;
public class CustomClockTitleConfig
{
    public string CustomClockTitle
    {
        get; set;
    } = "你好世界☺️";

    public string Hw75CustomContent
    {
        get; set;
    } = "你好世界☺️";

    public string Hw75YellowCalendarKey
    {
        get; set;
    } = string.Empty;

    public string Hw75WeatherAppCode
    {
        get; set;
    } = string.Empty;

    public int CustomClockTitleFontSize
    {
        get; set;
    } = 16;

    public int Hw75CustomContentFontSize
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

    public string OpenAIBaseUrl
    {
        get; set;
    } = "https://api.openai-proxy.com/";

    /// <summary>
    /// 手势识别回复文本
    /// </summary>
    public string AnswerText
    {
        get; set;
    } = "你想做什么,你需要帮忙吗,我能帮你做些什么,你需要帮助吗";

    public string CustomViewPicturePath
    {
        get;
        set;
    }= string.Empty;

    public string CustomHw75ImagePath
    {
        get;
        set;
    } = "ms-appx:///Assets/Images/Hw75CustomViewDefault.png";

    public float GaussianBlurValue
    {
        get;
        set;
    } = 4.0f;

    public bool CustomViewContentIsVisibility
    {
        get;
        set;
    } = true;

    public bool Hw75CustomContentIsVisibility
    {
        get;
        set;
    } = true;

    public bool Hw75TimeIsVisibility
    {
        get;
        set;
    } = true;

    public bool Hw75IsOpen
    {
        get;
        set;
    }

    public string Hw75ViewName
    {
        get; set;
    } = "Hw75CustomView";

    public string ChatGPTVersion
    {
        get; set;
    } = "gpt-3.5-turbo";
}
