using System.Net.Http.Headers;
using System.Text.Json;

namespace EnumerableExamples.Imp;

public static class BekkRepoFetcher
{
    public static async IAsyncEnumerable<BekkRepo> Fetch()
    {
        var perPage = 10;
        var url = $"https://api.github.com/users/bekk/repos?per_page={perPage}&page=1";
        using var client = new HttpClient();
        while (true)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("HttpClient", "1.0"));

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseStream = await response.Content.ReadAsStreamAsync();
            var repositories = await JsonSerializer.DeserializeAsync<BekkRepo[]>(responseStream, JsonSerializerOptions.Web);

            if (repositories == null || repositories.Length == 0)
            {
                yield break;
            }
            foreach (var repo in repositories)
            {
                yield return repo;
            }
            
            var links = ParseLinkHeader(response.Headers.GetValues("Link"));

            if (!links.ContainsKey("next"))
            {
                yield break;
            }
            url = links["next"];
        }
    }
    private static Dictionary<string, string> ParseLinkHeader(IEnumerable<string> linkHeaders)
    {
        var links = new Dictionary<string, string>();
        foreach (var linkHeader in linkHeaders)
        {
            if (string.IsNullOrEmpty(linkHeader))
            {
                continue;
            }
            var linkSections = linkHeader.Split(',');
            foreach (var section in linkSections)
            {
                var parts = section.Split(';', 2);

                if (parts.Length != 2)
                    continue;

                var url = parts[0].Trim().Trim('<', '>');
                var rel = parts[1].Trim();

                if (rel.StartsWith("rel=\"") && rel.EndsWith("\""))
                {
                    var relValue = rel.Substring(5, rel.Length - 6);
                    links[relValue] = url;
                }
            }
        }

        return links;
    }
}


public record BekkRepo(string Name, string Description, Uri Url);