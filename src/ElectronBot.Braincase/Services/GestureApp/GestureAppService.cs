using System;
using System.Collections.Generic;
using System.Text;
using ElectronBot.Braincase.Helpers;
using ElectronBot.Braincase.Models;

namespace ElectronBot.Braincase.Services;

public class GestureAppService
{
    public List<GestureAppConfig>? _gestureAppConfigs = null;
    private bool _inExecuting = false;
    private Task? task_1 = null;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="gestureAppConfigs"></param>
    public void Init(List<GestureAppConfig> gestureAppConfigs)
    {
        _gestureAppConfigs = gestureAppConfigs;
    }

    /// <summary>
    /// 根据传入的手势识别结果执行相关操作
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public async Task Execute(string e)
    {
        if (_gestureAppConfigs != null)
        {
            foreach (var item in _gestureAppConfigs)
            {
                if (e == item.GestureLabel)
                {
                    _inExecuting = true;
                    task_1 ??= Task.Run(async delegate
                        {
                            await Task.Delay(3000);
                            _inExecuting = false;
                            task_1 = null;
                        });
                    await WindowHelper.Instance.StartProcess(item.AppPath);
                    if (!string.IsNullOrEmpty(item.SpeechText))
                    {
                        var appText = $"正在唤醒{item.SpeechText}";
                        ToastHelper.SendToast(appText, TimeSpan.FromSeconds(2));
                        await ElectronBotHelper.Instance.MediaPlayerPlaySoundByTtsAsync(appText, true);
                    }
                }
            }
        }
    }

    public bool GetInExecuting()
    {
        return _inExecuting;
    }
}
