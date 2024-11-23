using System.Text.Json;
using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.Braincase.DataStorage.Collections;

namespace Verdure.Braincase.DataStorage.Mappers;
public static class LingxiSpaceMappers
{
    public static LingxiSpace ToModel(this LingxiSpaceDocument doc)
    {
        return new LingxiSpace
        {
            Id = doc.Id,
            Name = doc.Name,
            Desc = doc.Desc,
            Type = doc.Type,
            Content = string.IsNullOrEmpty(doc.Content) ? null : JsonDocument.Parse(doc.Content),
            ConversationId = doc.ConversationId,
            CreatedTime = doc.CreatedTime
        };
    }
}
