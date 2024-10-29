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

    /// <summary>
    /// 获取所有表情文件列表
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<List<EmoticonAction>> GetEmojisFileListAsync(int pageIndex, int pageSize);

    /// <summary>
    /// 保存表情文件
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="fileType"></param>
    /// <returns></returns>

    Task<string> SaveEmojisFileAsync(Stream stream, string fileName, string fileType = ".mp4");

    /// <summary>
    /// 保存表情信息
    /// </summary>
    /// <param name="emoticonAction"></param>
    /// <returns></returns>
    Task<EmoticonAction> SaveEmojisAsync(EmoticonAction emoticonAction);
}
