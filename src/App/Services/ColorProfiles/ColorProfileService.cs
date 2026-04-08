using App.Enums;
using App.Services.ColorSets;
using App.Types;
using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing;
using System.Globalization;

namespace App.Services.ColorProfiles;

internal sealed class ColorProfileService : IColorProfileService
{
    private const string BuiltinPrefix = "builtin:";
    private const string CustomPrefix = "custom:";

    private readonly AppDbContext _dbContext;
    private readonly IColorSetService _colorSetService;

    public ColorProfileService(AppDbContext dbContext, IColorSetService colorSetService)
    {
        _dbContext = dbContext;
        _colorSetService = colorSetService;
    }

    public async Task<IReadOnlyList<ColorSetProfileOption>> GetProfilesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var hiddenBuiltins = await _dbContext.HiddenBuiltinColorSets
            .AsNoTracking()
            .Select(entry => entry.BuiltinKey)
            .ToHashSetAsync(cancellationToken);

        var builtinProfiles = Enum
            .GetValues<BuiltinColorSets>()
            .Select(set => new ColorSetProfileOption(
                BuildBuiltinKey(set),
                set.ToString(),
                true,
                !hiddenBuiltins.Contains(set.ToString())))
            .ToList();

        var customProfiles = await _dbContext.ColorSets
            .AsNoTracking()
            .Where(set => !set.IsDeleted)
            .OrderBy(set => set.Name)
            .Select(set => new ColorSetProfileOption(
                BuildCustomKey(set.Id),
                set.Name,
                false,
                !set.IsHidden))
            .ToListAsync(cancellationToken);

        return builtinProfiles.Concat(customProfiles).ToArray();
    }

    public async Task<IReadOnlyList<ColorSetProfileOption>> GetVisibleProfilesAsync(
        CancellationToken cancellationToken = default)
    {
        return (await GetProfilesAsync(cancellationToken))
            .Where(profile => profile.IsVisible)
            .ToArray();
    }

    public async Task<IReadOnlyList<SetColor>> GetColorsAsync(string profileKey,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (TryParseBuiltinKey(profileKey, out var builtinSet))
        {
            return _colorSetService.GetColors(builtinSet).ToArray();
        }

        if (!TryParseCustomKey(profileKey, out var customId))
        {
            throw new InvalidOperationException($"Unknown profile key '{profileKey}'.");
        }

        var rows = await _dbContext.ColorSetRows
            .AsNoTracking()
            .Where(row => row.ColorSetId == customId)
            .OrderBy(row => row.Floss)
            .ThenBy(row => row.Name)
            .ToListAsync(cancellationToken);

        return rows
            .Select(row => new SetColor
            {
                Name = row.Name,
                Floss = row.Floss,
                Color = Color.FromArgb(row.Red, row.Green, row.Blue)
            })
            .ToArray();
    }

    public async Task SetVisibilityAsync(string profileKey, bool isVisible,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (TryParseBuiltinKey(profileKey, out var builtinSet))
        {
            var builtinKey = builtinSet.ToString();
            var hiddenEntry = await _dbContext.HiddenBuiltinColorSets
                .FirstOrDefaultAsync(entry => entry.BuiltinKey == builtinKey, cancellationToken);

            if (isVisible && hiddenEntry is not null)
            {
                _dbContext.HiddenBuiltinColorSets.Remove(hiddenEntry);
            }

            if (!isVisible && hiddenEntry is null)
            {
                _dbContext.HiddenBuiltinColorSets.Add(new HiddenBuiltinColorSet { BuiltinKey = builtinKey });
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        if (!TryParseCustomKey(profileKey, out var customId))
        {
            throw new InvalidOperationException($"Unknown profile key '{profileKey}'.");
        }

        var customProfile = await _dbContext.ColorSets
                                .FirstOrDefaultAsync(set => set.Id == customId && !set.IsDeleted, cancellationToken)
                            ?? throw new InvalidOperationException($"Custom profile '{profileKey}' was not found.");

        customProfile.IsHidden = !isVisible;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProfileAsync(string profileKey, CancellationToken cancellationToken = default)
    {
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (TryParseBuiltinKey(profileKey, out _))
        {
            throw new InvalidOperationException("Builtin profiles cannot be deleted.");
        }

        if (!TryParseCustomKey(profileKey, out var customId))
        {
            throw new InvalidOperationException($"Unknown profile key '{profileKey}'.");
        }

        var customProfile = await _dbContext.ColorSets
                                .FirstOrDefaultAsync(set => set.Id == customId && !set.IsDeleted, cancellationToken)
                            ?? throw new InvalidOperationException($"Custom profile '{profileKey}' was not found.");

        customProfile.IsDeleted = true;
        customProfile.IsHidden = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> ExportProfileJsonAsync(string profileKey, CancellationToken cancellationToken = default)
    {
        var profile = (await GetProfilesAsync(cancellationToken))
                      .FirstOrDefault(item => string.Equals(item.Key, profileKey, StringComparison.Ordinal))
                      ?? throw new InvalidOperationException($"Profile '{profileKey}' was not found.");

        var colors = await GetColorsAsync(profileKey, cancellationToken);
        var exportModel = new ColorSetProfileExport(
            Guid.CreateVersion7().ToString(),
            profile.Name,
            1,
            colors
                .GroupBy(color => ToHex(color.Color))
                .ToDictionary(
                    group => group.Key,
                    group => new ColorSetProfileExportColor(group.First().Name, group.First().Floss)));

        return JsonConvert.SerializeObject(exportModel, Formatting.Indented);
    }

    public async Task ImportProfileJsonAsync(string json, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var payload = JsonConvert.DeserializeObject<ColorSetImportPayload>(json)
                      ?? throw new InvalidOperationException("Imported profile payload is invalid.");

        if (string.IsNullOrWhiteSpace(payload.Name))
        {
            throw new InvalidOperationException("Imported profile name is required.");
        }

        if (payload.Data is null || payload.Data.Count == 0)
        {
            throw new InvalidOperationException("Imported profile contains no color data.");
        }

        var profileId = Guid.TryParse(payload.Id, out var parsedId) ? parsedId : Guid.CreateVersion7();

        var colorSet = await _dbContext.ColorSets
            .Include(set => set.Colors)
            .FirstOrDefaultAsync(set => set.Id == profileId, cancellationToken);

        if (colorSet is null)
        {
            colorSet = new ColorSet
            {
                Id = profileId,
                Name = payload.Name.Trim(),
                IsDeleted = false,
                IsHidden = false
            };

            _dbContext.ColorSets.Add(colorSet);
        }
        else
        {
            colorSet.Name = payload.Name.Trim();
            colorSet.IsDeleted = false;
            colorSet.IsHidden = false;
            _dbContext.ColorSetRows.RemoveRange(colorSet.Colors);
        }

        var rows = payload.Data
            .Select(entry => CreateColorRow(colorSet.Id, entry.Key, entry.Value))
            .ToArray();

        _dbContext.ColorSetRows.AddRange(rows);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static ColorSetRow CreateColorRow(Guid colorSetId, string hex, ColorSetImportData data)
    {
        var normalizedHex = hex.Trim().TrimStart('#');
        if (!int.TryParse(normalizedHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
        {
            throw new InvalidOperationException($"Invalid color hex '{hex}'.");
        }

        return new ColorSetRow
        {
            Id = Guid.CreateVersion7(),
            ColorSetId = colorSetId,
            Name = data.Name,
            Floss = data.Floss,
            Red = (byte)((rgb >> 16) & 0xFF),
            Green = (byte)((rgb >> 8) & 0xFF),
            Blue = (byte)(rgb & 0xFF)
        };
    }

    private static string ToHex(Color color)
    {
        return $"{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static string BuildBuiltinKey(BuiltinColorSets set)
    {
        return $"{BuiltinPrefix}{set}";
    }

    private static string BuildCustomKey(Guid setId)
    {
        return $"{CustomPrefix}{setId:D}";
    }

    private static bool TryParseBuiltinKey(string profileKey, out BuiltinColorSets result)
    {
        result = default;
        if (!profileKey.StartsWith(BuiltinPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        return Enum.TryParse(profileKey[BuiltinPrefix.Length..], ignoreCase: true, out result);
    }

    private static bool TryParseCustomKey(string profileKey, out Guid result)
    {
        result = default;
        if (!profileKey.StartsWith(CustomPrefix, StringComparison.Ordinal))
        {
            return false;
        }

        return Guid.TryParse(profileKey[CustomPrefix.Length..], out result);
    }

    private sealed class ColorSetImportPayload
    {
        [JsonProperty("id")] public string Id { get; set; } = string.Empty;

        [JsonProperty("name")] public string Name { get; set; } = string.Empty;

        [JsonProperty("version")] public int Version { get; set; }

        [JsonProperty("data")] public Dictionary<string, ColorSetImportData> Data { get; set; } = [];
    }

    private sealed class ColorSetImportData
    {
        [JsonProperty("name")] public string Name { get; set; } = string.Empty;

        [JsonProperty("floss")] public string Floss { get; set; } = string.Empty;
    }
}
