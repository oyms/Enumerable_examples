using EnumerableExamples.Imp.FactorFinder;
using System.Linq;

namespace EnumerableExamples.Tests;

public class FactorIteratorTests
{
    [Test]
    public void ListPrimeFactors()
    {
        var @out = TestContext.Current!.OutputWriter;
        foreach (var factor in 100561797u)
        {
            @out.WriteLine(factor);
        }
    }

    [Test]
    public async Task NumberIsProductOfPrimeFactors()
    {
        uint number = 68567788;
        var factors = Factors(number);
        var product = factors.Aggregate((x, y) => x * y);
        await Assert.That(product).IsEqualTo(number);
    }

    private IEnumerable<uint> Factors(uint number)
    {
        foreach (var factor in number)
        {
            yield return factor;
        }
    }
}