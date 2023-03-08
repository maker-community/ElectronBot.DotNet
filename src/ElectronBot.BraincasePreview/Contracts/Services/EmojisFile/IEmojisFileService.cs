using ElectronBot.BraincasePreview.Models;

namespace Contracts.Services;
public interface IEmojisFileService
{
    /// <summary>
    /// 导出表情文件到本地
    /// </summary>
    /// <param name="emoticonAction">表情对象</param>
    /// <returns></returns>
    Task ExportEmojisFileToLocalAsync(EmoticonAction emoticonAction);
}
