using EnumerableExamples.Imp;

namespace EnumerableExamples.Tests;

public class QuoteCollectionTests
{
    [Test]
    public async Task GetQuotes()
    {
        var @out = TestContext.Current!.OutputWriter;
        var max = 5;
        await foreach (var quote in new QuoteCollection())
        {
            if (max-- < 0)
            {
                break;
            }

            await @out.WriteLineAsync($"{quote.Author}: «{quote.Quote}»");
        }
    }
}