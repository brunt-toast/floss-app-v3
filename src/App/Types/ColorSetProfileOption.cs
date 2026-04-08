namespace App.Types;

public sealed record ColorSetProfileOption(
    string Key,
    string Name,
    bool IsBuiltin,
    bool IsVisible);
