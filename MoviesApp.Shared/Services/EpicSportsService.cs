using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MoviesApp.Shared.Models;
using Match = MoviesApp.Shared.Models.Match;
using Team = MoviesApp.Shared.Models.Team;
namespace MoviesApp.Shared.Services;

public class EpicSportsService : IEpicSportsService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://epicsports.djsofficial.com/";

    public EpicSportsService(HttpClient http)
    {
        _http = http;
        _http.BaseAddress ??= new Uri(BaseUrl);
    }

    public async Task<IReadOnlyList<MoviesApp.Shared.Models.Match>> GetLatestMatchesAsync(CancellationToken ct = default)
    {
        var listPage = await _http.GetStringAsync("/", ct);
        var posts = ExtractBlogPostLinks(listPage).Take(10).ToList();
        var matches = new List<MoviesApp.Shared.Models.Match>();
        foreach (var link in posts)
        {
            try
            {
                var html = await _http.GetStringAsync(link, ct);
                var match = ParseMatch(html);
                if (match != null)
                {
                    matches.Add(match);
                }
            }
            catch
            {
            }
        }
        return matches;
    }

    private static IEnumerable<string> ExtractBlogPostLinks(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var blocks = doc.DocumentNode.QuerySelectorAll("article.blog-post")
            .Where(d => d.GetAttributeValue("class", string.Empty).Contains("blog-post"));
        foreach (var block in blocks)
        {
            var a = block.Descendants("a").FirstOrDefault(x => x.Attributes.Contains("href"));
            if (a == null) continue;
            var href = a.GetAttributeValue("href", string.Empty);
            if (string.IsNullOrWhiteSpace(href)) continue;
            if (!href.StartsWith("http")) href = new Uri(new Uri(BaseUrl), href).ToString();
            yield return href;
        }
    }

    private static MoviesApp.Shared.Models.Match? ParseMatch(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var comps = doc.DocumentNode.SelectSingleNode("//body").Descendants("div")
            .Where(d =>
            {
                var match = d.GetAttributeValue("id", string.Empty).Contains("h2hWidget");
                if (!match) return false;
                return match ? d.Descendants("div").Any(z=> z.GetAttributeValue("class", string.Empty)
                    .Contains("previous-meetings-widget_header_container")) : false;
            })
            .Take(2)
            .ToList();
        if (comps.Count < 2) return null;
        var home = ExtractTeam(comps[0]);
        var away = ExtractTeam(comps[1]);
        var links = ExtractStreamLinks(doc).ToList();
        return new MoviesApp.Shared.Models.Match { HomeTeam = home, AwayTeam = away, Links = links };
    }

    private static MoviesApp.Shared.Models.Team ExtractTeam(HtmlNode block)
    {
        var img = block.Descendants("img").FirstOrDefault();
        var name = img?.GetAttributeValue("alt", string.Empty) ?? string.Empty;
        var src = img?.GetAttributeValue("src", string.Empty) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(src) && !src.StartsWith("http"))
        {
            src = new Uri(new Uri(BaseUrl), src).ToString();
        }
        return new MoviesApp.Shared.Models.Team { Name = name, Image = src };
    }

    private static IEnumerable<string> ExtractStreamLinks(HtmlDocument doc)
    {
        var urls = new HashSet<string>();
        foreach (var node in doc.DocumentNode.Descendants())
        {
            foreach (var attr in node.Attributes)
            {
                if ((attr.Name == "href" || attr.Name == "src") && attr.Value.Contains(".m3u8", System.StringComparison.OrdinalIgnoreCase))
                {
                    var href = attr.Value;
                    if (!href.StartsWith("http")) href = new Uri(new Uri(BaseUrl), href).ToString();
                    urls.Add(href);
                }
            }
        }
        return urls;
    }
}
