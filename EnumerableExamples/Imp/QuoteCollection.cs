using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnumerableExamples.Imp;

class QuoteCollection : IAsyncEnumerable<InspiringQuote>
{
    public IAsyncEnumerator<InspiringQuote> GetAsyncEnumerator(CancellationToken _) => new QuoteFetcher();
    private class QuoteFetcher : IAsyncEnumerator<InspiringQuote>
    {
        private readonly HttpClient _client;
        public QuoteFetcher()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://qapi.vercel.app"); // Limit 180/min
        }
        public async ValueTask<bool> MoveNextAsync()
        {
            using var response = await _client.GetAsync("/api/random");
            if(!response.IsSuccessStatusCode) {
                return false;
            }
            
            using var responseStream = await response.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<InspiringQuote>(responseStream, JsonSerializerOptions.Web);

            Current = data;
            return true;
        }
        [AllowNull]
        public InspiringQuote Current { get; private set; }
        public ValueTask DisposeAsync()
        {
            _client.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
public record InspiringQuote(string Author, string Quote);