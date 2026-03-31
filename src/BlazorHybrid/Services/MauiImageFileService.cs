using CommunityToolkit.Maui.Storage;
using Rcl.Services;

namespace BlazorHybrid.Services;

internal sealed class MauiImageFileService : IImageFileService
{
    public async Task<PickedImageFile?> PickImageAsync(CancellationToken cancellationToken = default)
    {
        var pickedFile = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select image",
            FileTypes = FilePickerFileType.Images
        });

        if (pickedFile is null)
        {
            return null;
        }

        await using var stream = await pickedFile.OpenReadAsync();
        await using var outputStream = new MemoryStream();
        await stream.CopyToAsync(outputStream, cancellationToken);

        return new PickedImageFile(pickedFile.FileName, outputStream.ToArray());
    }

    public async Task SaveImageAsync(string suggestedFileName, byte[] content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(suggestedFileName);
        ArgumentNullException.ThrowIfNull(content);

        await using var stream = new MemoryStream(content, writable: false);
        var saveResult = await FileSaver.Default.SaveAsync(suggestedFileName, stream, cancellationToken);

        if (!saveResult.IsSuccessful)
        {
            throw saveResult.Exception ?? new InvalidOperationException("Could not save the image.");
        }
    }
}
