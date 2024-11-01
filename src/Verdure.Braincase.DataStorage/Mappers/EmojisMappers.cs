using Verdure.Braincase.Core.Models;
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
            Type = model.Type,
            EmojisActionJson = model.EmojisActionContent,
            EmojisAuthor = model.EmojisAuthor,
            HasAction = model.HasAction

        };
    }

    public static EmoticonAction ToModel1(this EmojisDocument doc)
    {
        return new EmoticonAction
        {
            Name = doc.Name,
            NameId = doc.NameId,
            Desc = doc.Desc,
            Avatar = doc.Avatar,
            EmojisVideoPath = doc.EmojisVideoPath,
            Type = doc.Type,
            EmojisActionPath = doc.EmojisActionJson,
            EmojisActionContent = doc.EmojisActionJson,
            EmojisAuthor = doc.EmojisAuthor,
            HasAction = doc.HasAction
        };
    }

    public static EmoticonActionModel ToModel(this EmojisDocument doc)
    {
        return new EmoticonActionModel
        {
            Name = doc.Name,
            NameId = doc.NameId,
            Desc = doc.Desc,
            EmojisVideoPath = doc.EmojisVideoPath,
            Type = doc.Type,
            EmojisActionJson = doc.EmojisActionJson,
            EmojisActionPath = doc.EmojisActionJson,
            EmojisAuthor = doc.EmojisAuthor,
            HasAction = doc.HasAction
        };
    }
}
