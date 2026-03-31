namespace Rcl.Services;

public interface IImageFileService
{
    Task<PickedImageFile?> PickImageAsync(CancellationToken cancellationToken = default);

    Task SaveImageAsync(string suggestedFileName, byte[] content, CancellationToken cancellationToken = default);
}

public sealed record PickedImageFile(string FileName, byte[] Content);
