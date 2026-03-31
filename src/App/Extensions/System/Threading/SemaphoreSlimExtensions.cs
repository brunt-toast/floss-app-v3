using App.Utils;

namespace App.Extensions.System.Threading;

public static class SemaphoreSlimExtensions
{
    public static async Task<IDisposable> WaitForDisposableAsync(this SemaphoreSlim source)
    {
        await source.WaitAsync();
        return new DelegateDisposable(() => source.Release());
    }
}