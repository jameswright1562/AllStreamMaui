using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoviesApp.Shared.Models.Livesports;

public class LiveSportResponse<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
}

public class LiveMatch
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public long Date { get; set; }
    public DateTime MatchDate => DateTimeOffset.FromUnixTimeMilliseconds(Date).LocalDateTime;
    [JsonIgnore]
    public bool IsLive { get; set; }
    public bool Popular { get; set; }

    public Teams? Teams { get; set; }
    public IEnumerable<LiveStream>? Sources { get; set; }
}

public class Teams
{
    public Team Home { get; set; }
    public Team Away { get; set; }
}

public class Team
{
    public string Name { get; set; }
    public string Badge { get; set; }
}

public class LiveStream
{
    public string Id { get; set; }
    public int Viewers { get; set; }
    public string Source { get; set; }
    public bool Hd { get; set; }
    public string Language { get; set; } = string.Empty;
    public string EmbedUrl { get; set; } = string.Empty;
}
