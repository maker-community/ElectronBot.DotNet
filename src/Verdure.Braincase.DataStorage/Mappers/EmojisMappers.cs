using ElectronBot.Braincase.Models;
using Verdure.Braincase.DataStorage.Collections;

namespace Verdure.Braincase.DataStorage.Mappers;

public static class EmojisMappers
{
    public static EmojisDocument ToDoc(this EmoticonAction model)
    {
        return new EmojisDocument
        {
            NameId = model.NameId,
            Name = model.Name,
            Desc = model.Desc,
            Avatar = model.Avatar,
            EmojisVideoPath = model.EmojisVideoPath,
            Type = model.EmojisType.ToString(),
            EmojisActionJson = model.EmojisActionPath,
            EmojisAuthor = model.EmojisAuthor,
            HasAction = model.HasAction

        };
    }

    public static EmoticonAction ToModel(this EmojisDocument doc)
    {
        return new EmoticonAction
        {
            Name = doc.Name,
            NameId = doc.NameId,
            Desc = doc.Desc,
            Avatar = doc.Avatar,
            EmojisVideoPath = doc.EmojisVideoPath,
            EmojisType = Enum.Parse<EmojisType>(doc.Type),
            EmojisActionPath = doc.EmojisActionJson,
            EmojisAuthor = doc.EmojisAuthor,
            HasAction = doc.HasAction
        };
    }
}
