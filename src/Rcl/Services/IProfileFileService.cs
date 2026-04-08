namespace Rcl.Services;

public interface IProfileFileService
{
    Task<PickedProfileFile?> PickProfileAsync(CancellationToken cancellationToken = default);

    Task SaveProfileAsync(string suggestedFileName, string content, CancellationToken cancellationToken = default);
}

public sealed record PickedProfileFile(string FileName, string Content);
