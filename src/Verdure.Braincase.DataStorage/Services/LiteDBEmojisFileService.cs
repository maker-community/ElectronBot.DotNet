using Contracts.Services;
using ElectronBot.Braincase.Models;
using LiteDB;
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
    public Task<List<EmoticonActionModel>> GetEmojisFileListAsync(int pageIndex, int pageSize)
    {
        var list = new List<EmoticonActionModel>();

        var emojisDocs = _db.Emojis.FindAll().Skip(pageIndex * pageSize).Take(pageSize).ToList();

        foreach (var item in emojisDocs)
        {
            var model = item.ToModel();

            var avatarFile = _db.FileStorage.FindById(item.Avatar);

            var stream = new MemoryStream();

            _db.FileStorage.Download(avatarFile.Id, stream);

            stream.Seek(0, SeekOrigin.Begin);

            model.Avatar = stream;

            list.Add(model);
        }
        return Task.FromResult(list);
    }
    public Task<EmoticonActionModel> SaveEmojisAsync(EmoticonAction emoticonAction)
    {
        var doc = emoticonAction.ToDoc();
        var emoticon = _db.Emojis.FindOne(x => x.NameId == emoticonAction.NameId);
        if (emoticon != null)
        {
            doc.Id = emoticon.Id;
            _db.Emojis.Update(doc);
        }
        else
        {
            _db.Emojis.Insert(doc);
        }

        var model = doc.ToModel();

        var avatarFile = _db.FileStorage.FindById(doc.Avatar);

        var stream = new MemoryStream();

        _db.FileStorage.Download(avatarFile.Id, stream);

        stream.Seek(0, SeekOrigin.Begin);

        model.Avatar = stream;

        return Task.FromResult(model);
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

    public Task<string> SaveEmojisFileAsync(string path, string fileName, string fileType = ".mp4")
    {
        var fileStorage = _db.FileStorage;
        string? result;
        if (fileType == ".mp4")
        {
            // 从流上传一个文件
            result = fileStorage.Upload($"$/video/{fileName}", path).Id;
        }
        else
        {
            // 从流上传一个文件
            result = fileStorage.Upload($"$/image/{fileName}", path).Id;
        }
        return Task.FromResult(result);
    }
}
