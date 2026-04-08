using App.Types;

namespace App.Services.ColorProfiles;

public interface IColorProfileService
{
    Task<IReadOnlyList<ColorSetProfileOption>> GetProfilesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ColorSetProfileOption>> GetVisibleProfilesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SetColor>> GetColorsAsync(string profileKey, CancellationToken cancellationToken = default);

    Task SetVisibilityAsync(string profileKey, bool isVisible, CancellationToken cancellationToken = default);

    Task DeleteProfileAsync(string profileKey, CancellationToken cancellationToken = default);

    Task<string> ExportProfileJsonAsync(string profileKey, CancellationToken cancellationToken = default);

    Task ImportProfileJsonAsync(string json, CancellationToken cancellationToken = default);
}
