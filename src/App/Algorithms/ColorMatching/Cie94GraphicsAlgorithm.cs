using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal sealed class Cie94GraphicsAlgorithm : IColorDistanceAlgorithm<Cie94GraphicsAlgorithm>
{
    public static double CalculateDistance(Color left, Color right)
    {
        return Cie94Algorithm.CalculateDistance(left, right, kL: 1d, k1: 0.045d, k2: 0.015d);
    }
}