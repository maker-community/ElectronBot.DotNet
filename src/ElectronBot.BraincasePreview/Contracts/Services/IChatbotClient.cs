using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Services;
/// <summary>
/// 聊天机器客户端
/// </summary>
public interface IChatbotClient
{
    public string Name
    {
        get;
    }

    /// <summary>
    /// 获取问题结果
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<string> AskQuestionResultAsync(string message);
}
