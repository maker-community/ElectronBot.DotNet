using System.Net.Http.Json;
using System.Text.Json;
using Verdure.IoT.Net.Models;

namespace Verdure.IoT.Net;

/// <summary>
/// connect ha
/// </summary>
public class HomeAssistantClient
{
    private readonly HttpClient _httpClient;

    public HomeAssistantClient(string baseUrl, string token)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    }

    public async Task<List<HaState>> GetStateAsync()
    {
        var response = await _httpClient.GetAsync("/api/states");
        response.EnsureSuccessStatusCode();
        var resultSting = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<HaState>>(resultSting);

        if (data != null)
        {
            return data;
        }
        return new List<HaState>();
    }

    public async Task<HaState> GetStateByEntityIdAsync(string entityId)
    {
        var response = await _httpClient.GetAsync($"/api/states/{entityId}");
        response.EnsureSuccessStatusCode();
        var resultSting = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<HaState>(resultSting);

        if (data != null)
        {
            return data;
        }
        return new HaState();
    }

    public async Task PostServiceAync(string domain, string service, string entityId)
    {
        var input = new
        {
            entity_id = entityId
        };
        var response = await _httpClient.PostAsJsonAsync($"/api/services/{domain}/{service}", input);
        response.EnsureSuccessStatusCode();
    }
}
