using Verdure.ElectronBot.Core.Models;

namespace Verdure.ElectronBot.Core.Contracts.Services;
/// <summary>
/// 网络表情文件管理器
/// </summary>
public interface IEmojisFileNetworkManager
{
    /// <summary>
    /// 上传表情文件
    /// </summary>
    /// <param name="emojisKey">表情key</param>
    /// <param name="catalogItemRequest">上传配置参数</param>
    /// <returns></returns>
    Task<bool> UploadEmojisFileAsync(string emojisKey, CatalogItemRequest catalogItemRequest = null);
    /// <summary>
    /// 下载并解压缩表情文件
    /// </summary>
    /// <param name="emojisFileId">表情文件Id</param>
    /// <returns></returns>
    Task<bool> DownloadAndUnzipEmojisFileAsync(string emojisFileId);
}
