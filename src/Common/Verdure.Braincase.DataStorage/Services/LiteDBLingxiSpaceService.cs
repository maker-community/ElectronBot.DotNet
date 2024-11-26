using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.Braincase.Core.Models.Lingxi.Filters;
using Verdure.Braincase.DataStorage.Collections;
using Verdure.Braincase.DataStorage.Mappers;

namespace Verdure.Braincase.DataStorage.Services;
public class LiteDBLingxiSpaceService : ILingxiSpaceService
{
    private readonly BraincaseLiteDBContext _db;

    public LiteDBLingxiSpaceService(BraincaseLiteDBContext db)
    {
        _db = db;
    }
    public Task<LingxiSpace> AddAsync(LingxiSpace space)
    {
        var sapceDoc = new LingxiSpaceDocument
        {
            Id = space.Id,
            Name = space.Name,
            Desc = space.Desc,
            Type = space.Type,
            Content = space.Content.ToJsonString(),
            ConversationId = space.ConversationId,
            CreatedTime = space.CreatedTime
        };
        _db.LingxiSpaces.Insert(sapceDoc);
        return Task.FromResult(space);
    }
    public Task<List<LingxiSpace>> GetAllAsync(LingxiSpaceFilter filter)
    {
        var list = new List<LingxiSpace>();

        var spaceDocs = _db.LingxiSpaces.FindAll()
            .Where(l => l.ConversationId == filter.ConversationId).ToList();

        foreach (var item in spaceDocs)
        {
            var model = item.ToModel();
            list.Add(model);
        }
        return Task.FromResult(list);
    }
    public Task<LingxiSpace> GetAsync(string id) => throw new NotImplementedException();
    public Task<bool> RemoveAsync(string id) => throw new NotImplementedException();
    public Task<LingxiSpace> UpdateAsync(LingxiSpace space) => throw new NotImplementedException();
}
