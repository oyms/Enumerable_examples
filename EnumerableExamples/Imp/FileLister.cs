namespace EnumerableExamples.Imp;

public static class FileLister
{
    public static async IAsyncEnumerable<File> FetchFiles(string location, string extension)
    {
        var paths = Directory.GetFiles(location, extension, SearchOption.AllDirectories);
        foreach (var path in paths)
        {
            var fileInfo = new FileInfo(path);
            using var reader = fileInfo.OpenText();
            yield return new File(path, await reader.ReadLineAsync());
        }
    }
}

public record File(string Path, string? FirstLine);