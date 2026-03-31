using App.Enums;
using App.Services.ColorMatching;
using App.Services.ColorSets;
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

    public Image<Rgba32> ReduceColors(
        Image<Rgba32> source,
        BuiltinColorSets set,
        ColorComparisonAlgorithms comparisonAlgorithm)
    {
        ArgumentNullException.ThrowIfNull(source);

        var palette = _colorSetService
            .GetColors(set)
            .Select(color => color.Color)
            .Distinct()
            .ToArray();

        if (palette.Length == 0)
        {
            throw new InvalidOperationException($"Color set '{set}' contains no colors.");
        }

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
                        nearest = FindClosestColor(sourceColor, palette, comparisonAlgorithm);
                        nearestByRgb[rgbKey] = nearest;
                    }

                    row[x] = new Rgba32(nearest.R, nearest.G, nearest.B, pixel.A);
                }
            }
        });

        return reduced;
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
}