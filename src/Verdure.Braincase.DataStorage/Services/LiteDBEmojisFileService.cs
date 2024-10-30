using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Contracts.Services;
using ElectronBot.Braincase.Models;
using LiteDB;
using Verdure.Braincase.Core.Helpers;
using Verdure.Braincase.Core.Models;
using Verdure.Braincase.Core.Models.Emojis;
using Verdure.Braincase.Core.Models.Emojis.Enums;
using Verdure.Braincase.DataStorage.Mappers;

namespace Verdure.Braincase.DataStorage.Services;
public class LiteDBEmojisFileService : IEmojisFileService
{
    private readonly BraincaseLiteDBContext _db;

    private readonly JsonSerializerOptions _options;
    public LiteDBEmojisFileService(BraincaseLiteDBContext db)
    {
        _db = db;
        _options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
    }
    public Task<string> ExportEmojisFileToLocalAsync(EmoticonAction emoticonAction, string? targetPath = null)
    {

        var fileName = $"{emoticonAction.NameId}-{DateTime.Now:yyyyMMddhhmmss}.zip";

        // 假设 destinationFolder 是一个已定义的文件夹路径
        var destinationFile = Path.Combine(targetPath ?? Directory.GetCurrentDirectory(), fileName);

        var fileEntrys = new List<FileEntry>();
        try
        {
            var manifest = new EmojisFileManifest
            {
                Name = emoticonAction.Name,
                NameId = emoticonAction.NameId,
                Description = emoticonAction.Desc,
                HasAction = emoticonAction.HasAction,
                Type = emoticonAction.Type
            };

            var content = System.Text.Json.JsonSerializer.Serialize(manifest, options: _options);

            // 将 JSON 文本转换为文件流
            var jsonStream = new MemoryStream();
            var writer = new StreamWriter(jsonStream);
            writer.Write(content);
            writer.Flush();
            jsonStream.Seek(0, SeekOrigin.Begin);

            // 将文件流添加到 FileEntry 列表中
            fileEntrys.Add(new FileEntry
            {
                FileName = $"{emoticonAction.NameId}-manifest.json",
                FileStream = jsonStream
            });

            if (emoticonAction.HasAction)
            {
                var acitonJsonContent = emoticonAction.EmojisActionPath;

                // 将 JSON 文本转换为文件流
                var actionJsonStream = new MemoryStream();
                var actionWriter = new StreamWriter(actionJsonStream);
                actionWriter.Write(acitonJsonContent);
                actionWriter.Flush();
                actionJsonStream.Seek(0, SeekOrigin.Begin);

                // 将文件流添加到 FileEntry 列表中
                fileEntrys.Add(new FileEntry
                {
                    FileName = $"{emoticonAction.NameId}.json",
                    FileStream = actionJsonStream
                });
            }
  
            if (emoticonAction != null && emoticonAction.Type == EmojisFileType.Custom)
            {
                var avatarFile = _db.FileStorage.FindById(emoticonAction.Avatar);

                var avatarStream = new MemoryStream();

                _db.FileStorage.Download(avatarFile.Id, avatarStream);

                avatarStream.Seek(0, SeekOrigin.Begin);

                fileEntrys.Add(new FileEntry
                {
                    FileName = Path.GetFileName(emoticonAction.Avatar),
                    FileStream = avatarStream
                });

                var videoFile = _db.FileStorage.FindById(emoticonAction.EmojisVideoPath);

                var videoStream = new MemoryStream();

                _db.FileStorage.Download(videoFile.Id, videoStream);

                videoStream.Seek(0, SeekOrigin.Begin);

                fileEntrys.Add(new FileEntry
                {
                    FileName = Path.GetFileName(emoticonAction.EmojisVideoPath),
                    FileStream = videoStream
                });
            }
            ZipFileCreatorHelper.CreateZipFileFromEntries(fileEntrys, destinationFile);
        }
        catch (Exception)
        {
        }
        return Task.FromResult(destinationFile);
    }
    public Task<(string path, string name)> ExportEmojisFileToTempAsync(EmoticonAction emoticonAction) => throw new NotImplementedException();
    public Task<EmoticonAction> GetEmojisAsync(string nameId)
    {
        var emoticon = _db.Emojis.FindOne(x => x.NameId == nameId);
        return Task.FromResult(emoticon.ToModel1());
    }

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
