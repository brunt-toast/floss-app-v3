using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal sealed class WeightedEuclideanRgbAlgorithm : IColorDistanceAlgorithm<WeightedEuclideanRgbAlgorithm>
{
    public static double CalculateDistance(Color left, Color right)
    {
        var redMean = (left.R + right.R) / 2d;
        var redDifference = left.R - right.R;
        var greenDifference = left.G - right.G;
        var blueDifference = left.B - right.B;

        return Math.Sqrt(
            (((512d + redMean) * redDifference * redDifference) / 256d) +
            (4d * greenDifference * greenDifference) +
            (((767d - redMean) * blueDifference * blueDifference) / 256d));
    }
}