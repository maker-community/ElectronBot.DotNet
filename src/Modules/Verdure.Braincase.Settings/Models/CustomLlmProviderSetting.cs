using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotSharp.Abstraction.MLTasks.Settings;

namespace Verdure.Braincase.Settings.Models;
public class CustomLlmProviderSetting
{
    public string Provider { get; set; } = "azure-openai";


    public List<CustomLlmModelSetting> Models { get; set; } = new List<CustomLlmModelSetting>();


    public override string ToString()
    {
        return $"{Provider} with {Models.Count} models";
    }
}
