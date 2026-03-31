namespace Rcl.Services;

internal sealed class UnsupportedImageFileService : IImageFileService
{
    public Task<PickedImageFile?> PickImageAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromException<PickedImageFile?>(new NotSupportedException("File picking is not available on this host."));
    }

    public Task SaveImageAsync(string suggestedFileName, byte[] content, CancellationToken cancellationToken = default)
    {
        return Task.FromException(new NotSupportedException("File saving is not available on this host."));
    }
}
