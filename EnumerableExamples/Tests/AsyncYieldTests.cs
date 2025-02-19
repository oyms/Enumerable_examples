using EnumerableExamples.Imp;

namespace EnumerableExamples.Tests;

public class AsyncYieldTests
{   
    [Test]
    public async Task ListRepos()
    {
        var @out = TestContext.Current!.OutputWriter;
        await foreach (var repo in BekkRepoFetcher.Fetch())
        {
            await @out.WriteLineAsync(repo.ToString());
        }
    }

    [Test]
    public async Task ListFiles()
    {
        var @out = TestContext.Current!.OutputWriter;
        var path = Directory.GetParent(TestContext.Current.TestDetails.TestFilePath)!.Parent!.FullName;
        await foreach (var (filePath, firstLine) in FileLister.FetchFiles(path, "*.*"))
        {
            await @out.WriteLineAsync($"{filePath}:\t{firstLine}");
        }
    }
}
