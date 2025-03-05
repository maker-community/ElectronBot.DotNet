using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verdure.Braincase.Core.Contracts.Services;

public interface IWallpaperService
{
    Task<WallpapersData?> GetWallparperAsync(int index, int number);
}
