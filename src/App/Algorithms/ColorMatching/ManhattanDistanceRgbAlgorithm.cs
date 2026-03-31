using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal sealed class ManhattanDistanceRgbAlgorithm : IColorDistanceAlgorithm<ManhattanDistanceRgbAlgorithm>
{
    public static double CalculateDistance(Color left, Color right)
    {
        return Math.Abs(left.R - right.R) +
               Math.Abs(left.G - right.G) +
               Math.Abs(left.B - right.B);
    }
}