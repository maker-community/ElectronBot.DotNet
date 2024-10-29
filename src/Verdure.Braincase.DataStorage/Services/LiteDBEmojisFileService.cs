using Contracts.Services;
using ElectronBot.Braincase.Models;
using Verdure.Braincase.DataStorage.Mappers;

namespace Verdure.Braincase.DataStorage.Services;
public class LiteDBEmojisFileService : IEmojisFileService
{
    private readonly BraincaseLiteDBContext _db;
    public LiteDBEmojisFileService(BraincaseLiteDBContext db)
    {
        _db = db;
    }
    public Task ExportEmojisFileToLocalAsync(EmoticonAction emoticonAction)
    {
        throw new NotImplementedException();
    }
    public Task<(string path, string name)> ExportEmojisFileToTempAsync(EmoticonAction emoticonAction) => throw new NotImplementedException();
    public Task<List<EmoticonAction>> GetEmojisFileListAsync(int pageIndex, int pageSize)
    {
        var emojis = _db.Emojis.FindAll().Skip(pageIndex * pageSize).Take(pageSize).Select(x => x.ToModel()).ToList();
        return Task.FromResult(emojis);
    }
    public Task<EmoticonAction> SaveEmojisAsync(EmoticonAction emoticonAction)
    {
        var emoticon = _db.Emojis.FindOne(x => x.NameId == emoticonAction.NameId);
        if (emoticon != null)
        {
            var doc = emoticonAction.ToDoc();
            doc.Id = emoticon.Id;
            _db.Emojis.Update(doc);
            return Task.FromResult(emoticonAction);
        }
        _db.Emojis.Insert(emoticonAction.ToDoc());
        return Task.FromResult(emoticonAction);
    }
    public Task<string> SaveEmojisFileAsync(Stream stream, string fileName, string fileType = ".mp4")
    {
        var fileStorage = _db.FileStorage;
        string? result;
        if (fileType == ".mp4")
        {
            // 从流上传一个文件
            result = fileStorage.Upload($"$/video/{fileName}", fileName, stream).Id;
        }
        else
        {
            // 从流上传一个文件
            result = fileStorage.Upload($"$/image/{fileName}", fileName, stream).Id;
        }
        return Task.FromResult(result);
    }
}
