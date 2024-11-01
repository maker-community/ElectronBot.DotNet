using Verdure.Braincase.Core.Models;

namespace Verdure.Braincase.Core.Contracts.Services.EmojisFile;
public interface IEmojisFileService
{
    /// <summary>
    /// 导出表情文件到本地
    /// </summary>
    /// <param name="emoticonAction">表情对象</param>
    /// <param name="targetPath">目标路径</param>
    /// <returns></returns>
    Task<string> ExportEmojisFileToLocalAsync(EmoticonAction emoticonAction, string? targetPath = null);

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
    Task<List<EmoticonActionModel>> GetEmojisFileListAsync(int pageIndex, int pageSize);

    /// <summary>
    /// 保存表情文件
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="fileName"></param>
    /// <param name="fileType"></param>
    /// <returns></returns>

    Task<string> SaveEmojisFileAsync(Stream stream, string fileName, string fileType = ".mp4");

    /// <summary>
    /// 保存表情文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="fileType"></param>
    /// <returns></returns>

    Task<string> SaveEmojisFileAsync(string path, string fileName, string fileType = ".mp4");

    /// <summary>
    /// 保存表情信息
    /// </summary>
    /// <param name="emoticonAction"></param>
    /// <returns></returns>
    Task<EmoticonActionModel> SaveEmojisAsync(EmoticonAction emoticonAction);

    /// <summary>
    /// 获取表情信息
    /// </summary>
    /// <param name="nameId"></param>
    /// <returns></returns>
    Task<EmoticonAction> GetEmojisAsync(string nameId);

    /// <summary>
    /// 获取表情视频流
    /// </summary>
    /// <param name="nameId"></param>
    /// <returns></returns>
    Task<EmoticonActionModel> GetEmojisFileWithVideoStreamAsync(string nameId);

    /// <summary>
    /// 删除表情信息
    /// </summary>
    /// <param name="nameId"></param>
    /// <returns></returns>
    Task<bool> RemoveEmojisAsync(string nameId);

    /// <summary>
    /// 是否存在表情
    /// </summary>
    /// <param name="nameId"></param>
    /// <returns></returns>
    Task<bool> ExistEmojisAsync(string nameId);
}
