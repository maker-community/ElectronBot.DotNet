using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verdure.Braincase.Core.Contracts.Services;
using Verdure.Braincase.Core.Models.Lingxi;
using Verdure.Braincase.Core.Models.Lingxi.Filters;

namespace Verdure.Braincase.DataStorage.Services;
public class LiteDBLingxiSpaceService : ILingxiSpaceService
{
    public Task<LingxiSpace> AddAsync(LingxiSpace space) => throw new NotImplementedException();
    public Task<List<LingxiSpace>> GetAllAsync(LingxiSpaceFilter filter) => throw new NotImplementedException();
    public Task<LingxiSpace> GetAsync(string id) => throw new NotImplementedException();
    public Task<bool> RemoveAsync(string id) => throw new NotImplementedException();
    public Task<LingxiSpace> UpdateAsync(LingxiSpace space) => throw new NotImplementedException();
}
