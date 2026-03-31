using App.Enums;
using App.Types;
using Newtonsoft.Json;
using System.Drawing;
using System.Globalization;

namespace App.Services.ColorSets;

internal sealed class BuiltinColorSetService : IColorSetService
{
    public IEnumerable<SetColor> GetColors(BuiltinColorSets set)
    {
        return GetColors(set, _ => true);
    }

    public IEnumerable<SetColor> GetColors(BuiltinColorSets set, Func<SetColor, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        using var stream = OpenColorSetStream(set);
        foreach (var color in ReadColors(stream, predicate))
        {
            yield return color;
        }
    }

    private static Stream OpenColorSetStream(BuiltinColorSets set)
    {
        var assembly = typeof(BuiltinColorSetService).Assembly;
        var resourceName = $"App.Resources.ColorSets.{set}.json";
        var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            throw new InvalidOperationException($"Embedded resource not found: {resourceName}.");
        }

        return stream;
    }

    private static IEnumerable<SetColor> ReadColors(Stream stream, Func<SetColor, bool> predicate)
    {
        string? currentHex = null;
        string? currentName = null;
        string? currentFloss = null;
        string? currentInnerProperty = null;

        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader)
        {
            CloseInput = false
        };

        while (jsonReader.Read())
        {
            switch (jsonReader.TokenType)
            {
                case JsonToken.PropertyName when jsonReader.Depth == 1:
                    currentHex = jsonReader.Value?.ToString();
                    currentName = null;
                    currentFloss = null;
                    currentInnerProperty = null;
                    break;

                case JsonToken.PropertyName when jsonReader.Depth == 2:
                    currentInnerProperty = jsonReader.Value?.ToString();
                    break;

                case JsonToken.String when jsonReader.Depth == 2:
                    var value = jsonReader.Value?.ToString() ?? string.Empty;
                    if (string.Equals(currentInnerProperty, "name", StringComparison.Ordinal))
                    {
                        currentName = value;
                    }
                    else if (string.Equals(currentInnerProperty, "floss", StringComparison.Ordinal))
                    {
                        currentFloss = value;
                    }
                    break;

                case JsonToken.EndObject when jsonReader.Depth == 1:
                    if (TryCreateSetColor(currentHex, currentName, currentFloss, out var setColor) && predicate(setColor))
                    {
                        yield return setColor;
                    }

                    currentHex = null;
                    currentName = null;
                    currentFloss = null;
                    currentInnerProperty = null;
                    break;
            }
        }
    }

    private static bool TryCreateSetColor(string? hex, string? name, string? floss, out SetColor result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(hex) ||
            !int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
        {
            return false;
        }

        var red = (rgb >> 16) & 0xFF;
        var green = (rgb >> 8) & 0xFF;
        var blue = rgb & 0xFF;

        var resolvedName = string.IsNullOrWhiteSpace(name) ? (floss ?? string.Empty) : name;
        var resolvedFloss = string.IsNullOrWhiteSpace(floss) ? resolvedName : floss;

        result = new SetColor
        {
            Name = resolvedName,
            Floss = resolvedFloss,
            Color = Color.FromArgb(red, green, blue)
        };

        return true;
    }
}