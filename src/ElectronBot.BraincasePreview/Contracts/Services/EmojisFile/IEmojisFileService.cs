using ElectronBot.Braincase.Models;

namespace Contracts.Services;
public interface IEmojisFileService
{
    /// <summary>
    /// 导出表情文件到本地
    /// </summary>
    /// <param name="emoticonAction">表情对象</param>
    /// <returns></returns>
    Task ExportEmojisFileToLocalAsync(EmoticonAction emoticonAction);

    /// <summary>
    /// 导出表情文件到临时目录准备上传
    /// </summary>
    /// <param name="emoticonAction">表情对象</param>
    /// <returns>临时目录路径</returns>
    Task<(string path, string name)> ExportEmojisFileToTempAsync(EmoticonAction emoticonAction);
}
