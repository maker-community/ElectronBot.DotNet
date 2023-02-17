namespace Contracts.Services;
public interface IChatGPTService
{
    /// <summary>
    /// 获取问题结果
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<string> AskQuestionResultAsync(string message);
}
