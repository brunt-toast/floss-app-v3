using App.Utils;

namespace Rcl.ViewModels.Interfaces;

public interface IBusy
{
    bool IsBusy { get; protected set; }

    public static IDisposable UseBusy(IBusy source)
    {
        source.IsBusy = true;
        return new DelegateDisposable(() => source.IsBusy = false);
    }
}