using System.Collections.ObjectModel;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;
using Microsoft.Identity.Client.Extensions.Msal;

namespace ElectronBot.Braincase;
public class Constants
{
    public const string CacheFileName = "braincase_msal_cache.txt";

    public const string AuthDataKey = "AuthDataKey";

    public const string CustomClockTitleKey = "CustomClockTitleKey";

    public const string PlayEmojisLock = "PlayEmojisLock";

    public const string EmojisActionListKey = "EmojisActionListKey";

    public const string RandomContentListKey = "RandomContentListKey";

    public const string LaunchAppListKey = "LaunchAppListKey";

    public const string EmojisFolder = "EmojisAction";

    public const string EmojisTempFileFolder = "EmojisFileTemp";

    public const string CustomClockTitleConfigKey = "CustomClockTitleConfigKey";

    public const string BotSettingKey = "BotSettingKeyKey";

    public const string HaSettingKey = "HaSettingKeyKey";

    public const string DefaultCameraNameKey = "DefaultCameraNameKey";

    public const string DefaultHaSwitchNameKey = "DefaultHaSwitchNameKey";

    public const string DefaultChatBotNameKey = "DefaultChatBotNameKey";

    public const string DefaultChatGPTNameKey = "DefaultChatGPTNameKey";

    public const string DefaultAudioNameKey = "DefaultAudioNameKey";

    public const string CustomGestureAppConfigKey = "CustomGestureAppConfigKey";

    public const string LaunchAppConfigKey = "LaunchAppConfigKey";

    public const string DefaultCameraName = "USB";

    public const string YellowCalendarKey = "YellowCalendarKey";

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
        new()
        {
            Name ="LeftName".GetLocalized(),
            NameId="left",
            Avatar = "ms-appx:///Assets/Emoji/left.png",
            Desc ="LeftName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "left.json",
            HasAction = true
        },
        new ()
        {
            Name ="RightName".GetLocalized(),
            NameId="right",
            Avatar = "ms-appx:///Assets/Emoji/right.png",
            Desc ="RightName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "right.json",
            HasAction = true
        },
        new ()
        {
            Name ="NormalName".GetLocalized(),
            NameId="normal",
            Avatar = "ms-appx:///Assets/Emoji/normal.png",
            Desc ="NormalName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "normal.json",
            HasAction = true
        },
        new ()
        {
            Name ="NormalName".GetLocalized(),
            NameId="normal",
            Avatar = "ms-appx:///Assets/Emoji/normal.png",
            Desc ="NormalName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "normal.json",
            HasAction = true
        },
        new ()
        {
            Name ="AngerName".GetLocalized(),
            NameId="anger",
            Avatar = "ms-appx:///Assets/Emoji/anger.png",
            Desc ="AngerName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "anger.json",
            HasAction = true
        },
        new ()
        {
            Name ="DisdainName".GetLocalized(),
            NameId="disdain",
            Avatar = "ms-appx:///Assets/Emoji/disdain.png",
            Desc ="DisdainName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "disdain.json",
            HasAction = true
        },
        new ()
        {
            Name ="ExcitedName".GetLocalized(),
            NameId="excited",
            Avatar = "ms-appx:///Assets/Emoji/excited.png",
            Desc ="ExcitedName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        },
        new ()
        {
            Name ="FearName".GetLocalized(),
            NameId="fear",
            Avatar = "ms-appx:///Assets/Emoji/fear.png",
            Desc ="FearName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        },
        new ()
        {
            Name ="SadName".GetLocalized(),
            NameId="sad",
            Avatar = "ms-appx:///Assets/Emoji/sad.png",
            Desc ="SadName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "defaultaction.json",
            HasAction = true
        },
         new ()
        {
            Name ="HelloName".GetLocalized(),
            NameId="hello",
            Avatar = "ms-appx:///Assets/Emoji/hello.jpg",
            Desc ="HelloName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "hello.json",
            HasAction = true
        },
          new ()
        {
            Name ="GoodbyeName".GetLocalized(),
            NameId="goodbye",
            Avatar = "ms-appx:///Assets/Emoji/Goodbye.jpg",
            Desc ="GoodbyeName".GetLocalized(),
            EmojisType = EmojisType.Default,
            EmojisActionPath = "goodbye.json",
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
