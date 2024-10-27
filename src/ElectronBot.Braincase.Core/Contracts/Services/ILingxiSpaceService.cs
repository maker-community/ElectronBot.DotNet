using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.Braincase.Core.Models.Lingxi.Filters;

namespace Verdure.Braincase.Core.Contracts.Services;
public interface ILingxiSpaceService
{
    Task<LingxiSpace> AddAsync(LingxiSpace space);
    Task<bool> RemoveAsync(string id);
    Task<LingxiSpace> UpdateAsync(LingxiSpace space);
    Task<LingxiSpace> GetAsync(string id);
    Task<List<LingxiSpace>> GetAllAsync(LingxiSpaceFilter filter);
}
