namespace EnumerableExamples.Imp.FactorFinder;

public static class FactorIteratorExtensions
{
    public static FactorIterator GetEnumerator(this uint product) => new(product);
}

public class FactorIterator
{
    private readonly IEnumerator<uint> _enumerator;

    public FactorIterator(uint product)
    {
        _enumerator = GetFactors(product).GetEnumerator();
    }
    
    public uint Current => _enumerator.Current;
    public bool MoveNext() => _enumerator.MoveNext();

    private IEnumerable<uint> GetFactors(uint product)
    {
        if(product <= 1) yield break;
        var smallestFactor = GetSmallestFactor(product);
        yield return smallestFactor;
        foreach (var factor in GetFactors(product/smallestFactor))
        {
            yield return factor;
        }
    }
    private uint GetSmallestFactor(uint number)
    {
        // Check divisibility by 2
        if (number % 2 == 0)
            return 2;

        // Check odd factors
        for (var i = 3u; i <= Math.Sqrt(number); i += 2)
        {
            if (number % i == 0)
                return i;
        }

        // If no factor is found, the number itself is prime
        return number;
    }
}