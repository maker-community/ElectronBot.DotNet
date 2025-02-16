using BotSharp.Abstraction.MLTasks.Settings;

namespace Verdure.Braincase.Settings.Models;
public class CustomLlmModelSetting : LlmModelSetting
{
    public string Provider
    {
        get; set;
    } = string.Empty;
}
