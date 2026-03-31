using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal interface IColorDistanceAlgorithm<TAlgorithm>
    where TAlgorithm : IColorDistanceAlgorithm<TAlgorithm>
{
    static abstract double CalculateDistance(Color left, Color right);
}