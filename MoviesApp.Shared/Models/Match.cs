using System.Collections.Generic;

namespace MoviesApp.Shared.Models;

public class Match
{
    public Team HomeTeam { get; set; } = new Team();
    public Team AwayTeam { get; set; } = new Team();
    public IList<string> Links { get; set; } = new List<string>();
}

public class Team
{
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}
