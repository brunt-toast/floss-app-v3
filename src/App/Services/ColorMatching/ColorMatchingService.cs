using App.Enums;
using App.Algorithms.ColorMatching;
using System.Drawing;

namespace App.Services.ColorMatching;

internal sealed class ColorMatchingService : IColorMatchingService
{
    public IReadOnlyList<Color> GetMostSimilarColors(
        Color source,
        IEnumerable<Color> options,
        ColorComparisonAlgorithms comparisonAlgorithm)
    {
        ArgumentNullException.ThrowIfNull(options);

        var candidates = options
            .Select((color, index) => new RankedColor(color, index, GetDistance(source, color, comparisonAlgorithm)))
            .OrderBy(candidate => candidate.Distance)
            .ThenBy(candidate => candidate.Index)
            .Select(candidate => candidate.Color)
            .ToArray();

        return candidates;
    }

    private static double GetDistance(Color source, Color candidate, ColorComparisonAlgorithms comparisonAlgorithm)
    {
        return comparisonAlgorithm switch
        {
            ColorComparisonAlgorithms.Ciede2000 => Ciede2000Algorithm.CalculateDistance(source, candidate),
            ColorComparisonAlgorithms.EuclideanWeightedRgb => WeightedEuclideanRgbAlgorithm.CalculateDistance(source, candidate),
            ColorComparisonAlgorithms.EuclideanRgb => EuclideanRgbAlgorithm.CalculateDistance(source, candidate),
            ColorComparisonAlgorithms.ManhattanDistanceRgb => ManhattanDistanceRgbAlgorithm.CalculateDistance(source, candidate),
            ColorComparisonAlgorithms.Cie76 => Cie76Algorithm.CalculateDistance(source, candidate),
            ColorComparisonAlgorithms.Cie94Graphics => Cie94GraphicsAlgorithm.CalculateDistance(source, candidate),
            ColorComparisonAlgorithms.Cie94Textiles => Cie94TextilesAlgorithm.CalculateDistance(source, candidate),
            _ => throw new ArgumentOutOfRangeException(nameof(comparisonAlgorithm), comparisonAlgorithm, "Unsupported color comparison algorithm.")
        };
    }

    private sealed record RankedColor(Color Color, int Index, double Distance);
}