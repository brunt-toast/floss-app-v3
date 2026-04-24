using App.Utils;

namespace App.Extensions.System.Threading;

public static class SemaphoreSlimExtensions
{
    public static async Task<IDisposable> WaitForDisposableAsync(this SemaphoreSlim source, CancellationToken cancellationToken = default)
    {
        await source.WaitAsync(cancellationToken);
        return new DelegateDisposable(() => source.Release());
    }
}