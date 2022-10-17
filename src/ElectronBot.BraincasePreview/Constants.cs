using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ElectronBot.BraincasePreview.Helpers;

namespace ElectronBot.BraincasePreview;
public class Constants
{
    public const string CustomClockTitleKey = "CustomClockTitleKey";

    public const string EmojisFolder = "EmojisAction";

    public const string CustomClockTitleConfigKey = "CustomClockTitleConfigKey";

    public const string DefaultCameraNameKey = "DefaultCameraNameKey";

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
    (new List<string> { "anger", "disdain", "excited", "fear", "sad"});

    public static readonly string TwitterConsumerKey = "";
    public static readonly string TwitterConsumerSecret = "";
    public static readonly string TwitterCallbackURI = "";

    //public static StoreServicesCustomEventLogger LOGGER = StoreServicesCustomEventLogger.GetDefault();
}
