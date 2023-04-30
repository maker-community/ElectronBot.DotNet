using Microsoft.Graph;

namespace Verdure.ElectronBot.Core.Contracts.Services;
/// <summary>
/// 微软Graph相关的操作
/// </summary>
public interface IMicrosoftGraphService
{
    /// <summary>
    /// 准备Graph
    /// </summary>
    /// <returns></returns>
    Task PrepareGraphAsync();
    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    Task<User> GetUserInfoAsync();
    /// <summary>
    /// 获取用户头像的Base64数据
    /// </summary>
    /// <returns></returns>
    Task<string> GetUserPhotoAsync();
    /// <summary>
    /// 获取Todo列表
    /// </summary>
    /// <returns></returns>
    Task<IList<TodoTaskList>> GetTodoTaskListAsync();
    /// <summary>
    /// 通过任务ID获取todolist
    /// </summary>
    /// <param name="id">TaskId</param>
    /// <returns></returns>
    Task<IList<TodoTask>> GetTodoTaskListByTaskIdAsync(string id);
}
