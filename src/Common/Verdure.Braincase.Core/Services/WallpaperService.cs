using System.Text.Json;
using Verdure.Braincase.Core.Contracts.Services;

namespace Verdure.Braincase.Core.Services;

public class WallpaperService : IWallpaperService
{

    public async Task<WallpapersData?> GetWallparperAsync(int index, int number)
    {
        try
        {
            var url = string.Format("https://www.bing.com/HPImageArchive.aspx?format=js&idx={0}&n={1}", index, number);
            using var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(url);
            var wallPapersData = JsonSerializer.Deserialize<WallpapersData>(json) ?? new();
            return wallPapersData;
        }
        catch(Exception ex)
        {
            return null;
        }
    }
}
