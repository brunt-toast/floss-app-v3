using CommunityToolkit.Maui.Storage;
using Rcl.Services;
using System.Text;

namespace BlazorHybrid.Services;

internal sealed class MauiProfileFileService : IProfileFileService
{
    public async Task<PickedProfileFile?> PickProfileAsync(CancellationToken cancellationToken = default)
    {
        var pickedFile = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select profile JSON"
        });

        if (pickedFile is null)
        {
            return null;
        }

        await using var stream = await pickedFile.OpenReadAsync();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var content = await reader.ReadToEndAsync(cancellationToken);

        return new PickedProfileFile(pickedFile.FileName, content);
    }

    public async Task SaveProfileAsync(string suggestedFileName, string content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(suggestedFileName);
        ArgumentNullException.ThrowIfNull(content);

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content), writable: false);
        var saveResult = await FileSaver.Default.SaveAsync(suggestedFileName, stream, cancellationToken);

        if (!saveResult.IsSuccessful)
        {
            throw saveResult.Exception ?? new InvalidOperationException("Could not save the profile.");
        }
    }
}
