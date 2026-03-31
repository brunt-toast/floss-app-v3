using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal sealed class EuclideanRgbAlgorithm : IColorDistanceAlgorithm<EuclideanRgbAlgorithm>
{
    public static double CalculateDistance(Color left, Color right)
    {
        var redDifference = left.R - right.R;
        var greenDifference = left.G - right.G;
        var blueDifference = left.B - right.B;

        return Math.Sqrt(
            (redDifference * redDifference) +
            (greenDifference * greenDifference) +
            (blueDifference * blueDifference));
    }
}