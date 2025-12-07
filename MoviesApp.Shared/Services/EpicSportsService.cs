using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;
using HtmlAgilityPack;
using MoviesApp.Shared.Models;
using Match = MoviesApp.Shared.Models.Match;
using Team = MoviesApp.Shared.Models.Team;
namespace MoviesApp.Shared.Services;

public class EpicSportsService : IEpicSportsService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://live.totalsportek07.com/";

    public EpicSportsService(HttpClient http)
    {
        _http = http;
        _http.BaseAddress ??= new Uri(BaseUrl);
    }

    public async Task<IReadOnlyList<MoviesApp.Shared.Models.Match>> GetLatestMatchesAsync(CancellationToken ct = default)
    {
        var matches = new List<Match>();
        var listPage = await _http.GetStringAsync("/", ct);
        var doc = new HtmlDocument();
        doc.LoadHtml(listPage);
        var mainBox = doc.DocumentNode
            .SelectSingleNode("//div[contains(@class, 'div-main-box')]");
        var matchesUrl = mainBox.Descendants("a").Where(x =>
            {
                var href = x.GetAttributeValue("href", null);
                return href != null && href.Contains(BaseUrl) && !href.Contains("live-stream");
            })
            .Select(x =>
            {
                return x.GetAttributeValue("href", null);
            }).Take(20);
        foreach (var x in matchesUrl)
        {
            var decodedUrl = WebUtility.HtmlDecode(x).Split(BaseUrl)[1];

            var matchContent = await _http.GetStringAsync(decodedUrl, ct);
            var matchDoc = new HtmlDocument();
            matchDoc.LoadHtml(matchContent);
            var svNodes = matchDoc.DocumentNode.SelectNodes(
                "//div[contains(@class,'sv-box')]"
            );

            if (svNodes.Count() == 0)
            {
                throw new Exception("No sv boxes");
            }

            var boxNode = svNodes.First();

            if (boxNode == null)
                throw new Exception("sv-box no-radius container not found");

            var infoNodes = boxNode.SelectNodes(".//div[contains(@class,'w-25')]");

            if (infoNodes == null)
                throw new Exception("Could not find home/away team nodes");

            var homeNode = infoNodes.First();
            var awayNode = infoNodes.Last();

            var homeTeam = ExtractTeam(homeNode);
            var awayTeam = ExtractTeam(awayNode);

            var linkNodes = svNodes.Last();
            var linkUrls = linkNodes.Descendants("a").Take(2);
            var urls = new List<string>();
            foreach (var link in linkUrls)
            {
                var rawLink = link.GetAttributeValue("href", null);
                if(rawLink is null)
                    continue;
                var url = WebUtility.UrlDecode(rawLink);
                urls.Add(url);
            }


            matches.Add(new Match()
            {
                HomeTeam = homeTeam,
                AwayTeam = awayTeam,
                Links = urls
            });
        }

        return matches;
    }

    internal Team ExtractTeam(HtmlNode teamNode)
    {
        // Find the logo and name nodes inside this team box
        var imgNode = teamNode.SelectSingleNode(".//img");
        var nameNode = teamNode.SelectSingleNode(".//h5");

        if (imgNode == null || nameNode == null)
            throw new InvalidOperationException("Could not find team img/h5 in node");

        var logo = WebUtility.UrlDecode(imgNode.GetAttributeValue("src", ""));
        var name = HtmlEntity.DeEntitize(nameNode.InnerText).Trim();

        return new Team
        {
            Name = name,
            Image = logo
        };
    }
}
