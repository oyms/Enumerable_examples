using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace FileSystemwatcher;

public class Watcher: IAsyncEnumerable<FileSystemEvent>, IDisposable
{
    private readonly Channel<FileSystemEvent> _channel = Channel.CreateUnbounded<FileSystemEvent>();
    private FileSystemWatcher? _watcher;
    
    public IAsyncEnumerator<FileSystemEvent> GetAsyncEnumerator(CancellationToken cancellationToken = new()) => 
        GetEventsAsync(cancellationToken).GetAsyncEnumerator(cancellationToken);

    private void Init()
    {
        _watcher = new FileSystemWatcher(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                Filter = "*.*"
            };
        _watcher.Created += (_, e) => _channel.Writer.TryWrite(new FileSystemEvent($"File created: {e.FullPath}"));
        _watcher.Deleted += (_, e) => _channel.Writer.TryWrite(new FileSystemEvent($"File deleted: {e.FullPath}"));
        _watcher.Changed += (_, e) => _channel.Writer.TryWrite(new FileSystemEvent($"File changed: {e.FullPath} {e.ChangeType}"));
        _watcher.Renamed += (_, e) => _channel.Writer.TryWrite(new FileSystemEvent($"File renamed: {e.FullPath} from {e.OldFullPath}"));
        _watcher.EnableRaisingEvents = true;
    }

    private async IAsyncEnumerable<FileSystemEvent> GetEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_watcher is null) Init();
        
        while (await _channel.Reader.WaitToReadAsync(CancellationToken.None))
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            yield return await _channel.Reader.ReadAsync(CancellationToken.None);
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        _channel.Writer.TryComplete();
    }
}

public record FileSystemEvent(string Text);