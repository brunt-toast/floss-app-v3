using App.Enums;
using App.Services.ColorMatching;
using App.Services.ColorSets;
using App.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using DrawingColor = System.Drawing.Color;

namespace App.Services.ColorReduction;

internal sealed class BuiltinColorReductionService : IColorReductionService
{
    private readonly IColorSetService _colorSetService;
    private readonly IColorMatchingService _colorMatchingService;

    public BuiltinColorReductionService(
        IColorSetService colorSetService,
        IColorMatchingService colorMatchingService)
    {
        _colorSetService = colorSetService;
        _colorMatchingService = colorMatchingService;
    }

    public ColorReductionResult ReduceColors(
        Image<Rgba32> source,
        BuiltinColorSets set,
        ColorComparisonAlgorithms comparisonAlgorithm,
        int? maxColors = null)
    {
        var colors = _colorSetService.GetColors(set).ToArray();
        return ReduceColors(source, colors, comparisonAlgorithm, maxColors);
    }

    public ColorReductionResult ReduceColors(
        Image<Rgba32> source,
        IReadOnlyCollection<SetColor> colors,
        ColorComparisonAlgorithms comparisonAlgorithm,
        int? maxColors = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(colors);

        var palette = colors
            .Select(color => color.Color)
            .Distinct()
            .ToArray();

        if (palette.Length == 0)
        {
            throw new InvalidOperationException("Color set contains no colors.");
        }

        var paletteOrder = palette
            .Select((color, index) => new { RgbKey = ToRgbKey(color), Index = index })
            .ToDictionary(entry => entry.RgbKey, entry => entry.Index);

        var unrestrictedUsage = BuildPaletteUsage(source, palette, comparisonAlgorithm);
        var availableColorCount = Math.Max(1, unrestrictedUsage.Count);

        var selectedPalette = SelectPalette(
            palette,
            unrestrictedUsage,
            paletteOrder,
            maxColors,
            availableColorCount);

        var reduced = source.Clone();
        var nearestByRgb = new Dictionary<int, DrawingColor>();

        reduced.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    var pixel = row[x];
                    var rgbKey = (pixel.R << 16) | (pixel.G << 8) | pixel.B;

                    if (!nearestByRgb.TryGetValue(rgbKey, out var nearest))
                    {
                        var sourceColor = DrawingColor.FromArgb(pixel.R, pixel.G, pixel.B);
                        nearest = FindClosestColor(sourceColor, selectedPalette, comparisonAlgorithm);
                        nearestByRgb[rgbKey] = nearest;
                    }

                    row[x] = new Rgba32(nearest.R, nearest.G, nearest.B, pixel.A);
                }
            }
        });

        return new ColorReductionResult(reduced, availableColorCount);
    }

    private Dictionary<int, int> BuildPaletteUsage(
        Image<Rgba32> source,
        IReadOnlyList<DrawingColor> palette,
        ColorComparisonAlgorithms comparisonAlgorithm)
    {
        var nearestByRgb = new Dictionary<int, DrawingColor>();
        var usageByPaletteRgb = new Dictionary<int, int>();

        source.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    var pixel = row[x];
                    if (pixel.A == 0)
                    {
                        continue;
                    }

                    var sourceRgb = ToRgbKey(pixel.R, pixel.G, pixel.B);
                    if (!nearestByRgb.TryGetValue(sourceRgb, out var nearest))
                    {
                        var sourceColor = DrawingColor.FromArgb(pixel.R, pixel.G, pixel.B);
                        nearest = FindClosestColor(sourceColor, palette, comparisonAlgorithm);
                        nearestByRgb[sourceRgb] = nearest;
                    }

                    var paletteRgb = ToRgbKey(nearest);
                    usageByPaletteRgb[paletteRgb] = usageByPaletteRgb.GetValueOrDefault(paletteRgb) + 1;
                }
            }
        });

        return usageByPaletteRgb;
    }

    private static IReadOnlyList<DrawingColor> SelectPalette(
        IReadOnlyList<DrawingColor> palette,
        IReadOnlyDictionary<int, int> unrestrictedUsage,
        IReadOnlyDictionary<int, int> paletteOrder,
        int? maxColors,
        int availableColorCount)
    {
        if (!maxColors.HasValue || maxColors.Value >= availableColorCount)
        {
            return palette;
        }

        return unrestrictedUsage
            .OrderByDescending(entry => entry.Value)
            .ThenBy(entry => paletteOrder[entry.Key])
            .Take(Math.Max(1, maxColors.Value))
            .Select(entry => palette[paletteOrder[entry.Key]])
            .ToArray();
    }

    private DrawingColor FindClosestColor(
        DrawingColor source,
        IReadOnlyList<DrawingColor> palette,
        ColorComparisonAlgorithms comparisonAlgorithm)
    {
        return _colorMatchingService
            .GetMostSimilarColors(source, palette, comparisonAlgorithm)
            .First();
    }

    private static int ToRgbKey(DrawingColor color)
    {
        return ToRgbKey(color.R, color.G, color.B);
    }

    private static int ToRgbKey(byte red, byte green, byte blue)
    {
        return (red << 16) | (green << 8) | blue;
    }
}