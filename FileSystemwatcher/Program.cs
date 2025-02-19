using FileSystemwatcher;

using var cts = new CancellationTokenSource();
Console.WriteLine("Press any key to stop");
_ = Task.Run(() =>
{
    Console.ReadKey(true);
    cts.Cancel();
});


using var watcher = new Watcher();
try
{
    await foreach (var fileEvent in watcher.WithCancellation(cts.Token))
    {
        Console.WriteLine(fileEvent.Text);
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Bye! 👋");
}
