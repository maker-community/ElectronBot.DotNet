using System.Collections.ObjectModel;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;

namespace ElectronBot.Braincase;
public class Constants
{
    public const string CustomClockTitleKey = "CustomClockTitleKey";

    public const string EmojisActionListKey = "EmojisActionListKey";

    public const string RandomContentListKey = "RandomContentListKey";

    public const string EmojisFolder = "EmojisAction";

    public const string EmojisTempFileFolder = "EmojisFileTemp";

    public const string CustomClockTitleConfigKey = "CustomClockTitleConfigKey";

    public const string DefaultCameraNameKey = "DefaultCameraNameKey";

    public const string DefaultChatBotNameKey = "DefaultChatBotNameKey";

    public const string DefaultAudioNameKey = "DefaultAudioNameKey";

    public const string DefaultCameraName = "USB";

    public const string MODEL_PATH = "ms-appx:///Assets//model.onnx";

    public const float CLASSIFICATION_CERTAINTY_THRESHOLD = 0.2f;
    public const int CLASSIFICATION_SENSITIVITY_IN_SECONDS = 0;

    public const int EMOJI_DISPLAY_DURATION_IN_SECONDS = 5;
    public const int DIVISIBLE_FACTOR = 25;
    public const int EMOJI_COUNTDOWN_INTERVAL = 1;
    public const int EMOJI_TICK_INTERVAL_IN_MILLISECONDS = 100;
    public const int EMOJI_NUMBER_TO_DISPLAY = 4;
    public const int MESSAGE_BEFORE_RESULTS_DURATION_IN_MILLISECONDS = 4000;

    public static readonly IList<String> POTENTIAL_EMOJI_NAME_LIST = new ReadOnlyCollection<string>
        (new List<string> {
                "EmotionNeutral".GetLocalized(),
                "EmotionHappiness".GetLocalized(),
                "EmotionSurprise".GetLocalized(),
                "EmotionSadness".GetLocalized(),
                "EmotionAnger".GetLocalized(),
                "EmotionDisgust".GetLocalized(),
                "EmotionFear".GetLocalized(),
                "EmotionContempt".GetLocalized()});

    public static readonly IList<String> POTENTIAL_EMOJI_ICON_LIST = new ReadOnlyCollection<string>
        (new List<string> { "😐", "😄", "😮", "😭", "😠", "🤢", "😨", "🙄" });

    public static readonly IList<String> POTENTIAL_EMOJI_LIST = new ReadOnlyCollection<string>
    (new List<string> { "anger", "disdain", "excited", "fear", "sad" });


    public static readonly IList<EmoticonAction> EMOJI_ACTION_LIST = new List<EmoticonAction>()
    {
        new EmoticonAction()
        {
            Name ="AngerName".GetLocalized(),
            NameId="anger",
            Avatar = "ms-appx:///Assets/Emoji/anger.png",
            Desc ="AngerName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        },
        new EmoticonAction()
        {
            Name ="DisdainName".GetLocalized(),
            NameId="disdain",
            Avatar = "ms-appx:///Assets/Emoji/disdain.png",
            Desc ="DisdainName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        },
        new EmoticonAction()
        {
            Name ="ExcitedName".GetLocalized(),
            NameId="excited",
            Avatar = "ms-appx:///Assets/Emoji/excited.png",
            Desc ="ExcitedName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        },
        new EmoticonAction()
        {
            Name ="FearName".GetLocalized(),
            NameId="fear",
            Avatar = "ms-appx:///Assets/Emoji/fear.png",
            Desc ="FearName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        },
        new EmoticonAction()
        {
            Name ="SadName".GetLocalized(),
            NameId="sad",
            Avatar = "ms-appx:///Assets/Emoji/sad.png",
            Desc ="SadName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        }
    };
    public static readonly string TwitterConsumerKey = "";
    public static readonly string TwitterConsumerSecret = "";
    public static readonly string TwitterCallbackURI = "";

    public const string Up = "up";
    public const string Down = "down";
    public const string Back = "back";
    public const string Forward = "forward";
    public const string Land = "land";
    public const string Stop = "stop";
    public const string Left = "left";
    public const string Right = "right";
    public const string FingerHeart = "finger-heart";
    public const string ThirdFinger = "third-finger";

    //public static StoreServicesCustomEventLogger LOGGER = StoreServicesCustomEventLogger.GetDefault();
}
