using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronBot.Braincase.Models;
public class GestureAppConfig
{
    /// <summary>
    /// 唯一标识
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 手势识别字段
    /// </summary>
    public string GestureLabel { get; set; } = string.Empty;

    /// <summary>
    /// 语音文本
    /// </summary>
    public string SpeechText { get; set; } = string.Empty;

    /// <summary>
    /// App启动路径
    /// </summary>
    public string AppPath { get; set; } = string.Empty;
}

public enum EventKind
{
    /// <summary>
    /// 启动应用
    /// </summary>
    App,

    /// <summary>
    /// 键盘事件
    /// </summary>
    KeyOption,

    /// <summary>
    /// 返回上一层
    /// </summary>
    Back,
}
