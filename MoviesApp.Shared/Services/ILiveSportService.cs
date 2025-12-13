using MoviesApp.Shared.Models.Livesports;

namespace MoviesApp.Shared.Services;

public interface ILiveSportService
{
    Task<IEnumerable<Sport>> GetSportsAsync(CancellationToken ct = default);
    Task<IList<LiveMatch>> GetMatchesAsync(string sport, CancellationToken ct = default);
    Task<LiveMatch> GetMatchDetailAsync(string id, CancellationToken ct = default);
}

