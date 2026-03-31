using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal sealed class Cie94TextilesAlgorithm : IColorDistanceAlgorithm<Cie94TextilesAlgorithm>
{
    public static double CalculateDistance(Color left, Color right)
    {
        return Cie94Algorithm.CalculateDistance(left, right, kL: 2d, k1: 0.048d, k2: 0.014d);
    }
}