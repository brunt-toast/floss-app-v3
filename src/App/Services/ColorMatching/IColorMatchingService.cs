using App.Enums;
using System.Drawing;

namespace App.Services.ColorMatching;

public interface IColorMatchingService
{
    IReadOnlyList<Color> GetMostSimilarColors(
        Color source,
        IEnumerable<Color> options,
        ColorComparisonAlgorithms comparisonAlgorithm);
}