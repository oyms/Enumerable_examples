using FileSystemwatcher;

using var cts = new CancellationTokenSource();
Console.WriteLine("Press any key to stop");
_ = Task.Run(() =>
{
    Console.ReadKey(true);
    cts.Cancel();
    Console.WriteLine("Bye! 👋");
});


using var watcher = new Watcher();
await foreach (var fileEvent in watcher.WithCancellation(cts.Token))
{
    Console.WriteLine(fileEvent.Text);
}
