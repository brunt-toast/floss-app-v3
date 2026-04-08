namespace App.Types;

public sealed record ColorSetProfileExport(
    string Id,
    string Name,
    int Version,
    IReadOnlyDictionary<string, ColorSetProfileExportColor> Data);

