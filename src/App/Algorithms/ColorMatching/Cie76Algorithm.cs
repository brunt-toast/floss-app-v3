using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal sealed class Cie76Algorithm : IColorDistanceAlgorithm<Cie76Algorithm>
{
    public static double CalculateDistance(Color left, Color right)
    {
        var leftLab = LabColorConverter.ToLab(left);
        var rightLab = LabColorConverter.ToLab(right);

        var deltaL = leftLab.L - rightLab.L;
        var deltaA = leftLab.A - rightLab.A;
        var deltaB = leftLab.B - rightLab.B;

        return Math.Sqrt((deltaL * deltaL) + (deltaA * deltaA) + (deltaB * deltaB));
    }
}