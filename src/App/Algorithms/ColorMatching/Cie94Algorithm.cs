using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal static class Cie94Algorithm
{
    public static double CalculateDistance(Color left, Color right, double kL, double k1, double k2)
    {
        var leftLab = LabColorConverter.ToLab(left);
        var rightLab = LabColorConverter.ToLab(right);

        var deltaL = leftLab.L - rightLab.L;
        var c1 = Math.Sqrt((leftLab.A * leftLab.A) + (leftLab.B * leftLab.B));
        var c2 = Math.Sqrt((rightLab.A * rightLab.A) + (rightLab.B * rightLab.B));
        var deltaC = c1 - c2;
        var deltaA = leftLab.A - rightLab.A;
        var deltaB = leftLab.B - rightLab.B;
        var deltaHSquared = Math.Max(0d, (deltaA * deltaA) + (deltaB * deltaB) - (deltaC * deltaC));

        var sL = 1d;
        var sC = 1d + (k1 * c1);
        var sH = 1d + (k2 * c1);

        var deltaLTerm = deltaL / (kL * sL);
        var deltaCTerm = deltaC / sC;
        var deltaHTerm = deltaHSquared / (sH * sH);

        return Math.Sqrt((deltaLTerm * deltaLTerm) + (deltaCTerm * deltaCTerm) + deltaHTerm);
    }
}