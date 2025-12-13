using System.Net.Http.Json;
using System.Text.Json;
using MoviesApp.Shared.Models.Livesports;

namespace MoviesApp.Shared.Services;

public class LiveSportService : ILiveSportService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    public LiveSportService(HttpClient http)
    {
        _http = http;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<T> HandleResponseAsync<T>(HttpResponseMessage message, CancellationToken ct = default)
    {
        var content = await message.Content.ReadAsStringAsync(ct);
        var deserialized = JsonSerializer.Deserialize<LiveSportResponse<T>>(content, _jsonSerializerOptions);
        return deserialized.Data;
    }

    public async Task<IEnumerable<Sport>> GetSportsAsync(CancellationToken ct = default)
    {
        var res = await _http.GetAsync("sports", ct);
        return await HandleResponseAsync<IEnumerable<Sport>>(res, ct);
    }

    public async Task<IList<LiveMatch>> GetMatchesAsync(string sport, CancellationToken ct = default)
    {
        try
        {
            var res = await _http.GetAsync($"matches/{Uri.EscapeDataString(sport)}", ct);
            var matches = await HandleResponseAsync<IList<LiveMatch>>(res, ct);
            return matches;
        }
        catch
        {
            return new List<LiveMatch>();
        }
    }

    public async Task<LiveMatch> GetMatchDetailAsync(string id, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        try
        {
            var res = await _http.GetAsync($"matches/{Uri.EscapeDataString(id)}/detail", ct);
            return await HandleResponseAsync<LiveMatch>(res, ct);
        }
        catch
        {
            return null;
        }
    }
}

