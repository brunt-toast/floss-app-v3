namespace Rcl.Services;

internal sealed class UnsupportedProfileFileService : IProfileFileService
{
    public Task<PickedProfileFile?> PickProfileAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromException<PickedProfileFile?>(new NotSupportedException("Profile import is not available on this host."));
    }

    public Task SaveProfileAsync(string suggestedFileName, string content, CancellationToken cancellationToken = default)
    {
        return Task.FromException(new NotSupportedException("Profile export is not available on this host."));
    }
}
