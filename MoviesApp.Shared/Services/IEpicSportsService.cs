using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MoviesApp.Shared.Models;

namespace MoviesApp.Shared.Services;

public interface IEpicSportsService
{
    Task<IReadOnlyList<MoviesApp.Shared.Models.Match>> GetLatestMatchesAsync(CancellationToken ct = default);
}
