using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal sealed class Ciede2000Algorithm : IColorDistanceAlgorithm<Ciede2000Algorithm>
{
    public static double CalculateDistance(Color left, Color right)
    {
        var leftLab = LabColorConverter.ToLab(left);
        var rightLab = LabColorConverter.ToLab(right);

        var c1 = Math.Sqrt((leftLab.A * leftLab.A) + (leftLab.B * leftLab.B));
        var c2 = Math.Sqrt((rightLab.A * rightLab.A) + (rightLab.B * rightLab.B));
        var averageC = (c1 + c2) / 2d;
        var averageCToSeventh = Math.Pow(averageC, 7d);
        var compensation = 0.5d * (1d - Math.Sqrt(averageCToSeventh / (averageCToSeventh + Math.Pow(25d, 7d))));

        var adjustedA1 = (1d + compensation) * leftLab.A;
        var adjustedA2 = (1d + compensation) * rightLab.A;
        var adjustedC1 = Math.Sqrt((adjustedA1 * adjustedA1) + (leftLab.B * leftLab.B));
        var adjustedC2 = Math.Sqrt((adjustedA2 * adjustedA2) + (rightLab.B * rightLab.B));
        var adjustedAverageC = (adjustedC1 + adjustedC2) / 2d;

        var h1Prime = CalculateHueAngle(adjustedA1, leftLab.B);
        var h2Prime = CalculateHueAngle(adjustedA2, rightLab.B);
        var deltaLPrime = rightLab.L - leftLab.L;
        var deltaCPrime = adjustedC2 - adjustedC1;

        var deltaHPrime = CalculateDeltaHPrime(adjustedC1, adjustedC2, h1Prime, h2Prime);
        var deltaBigHPrime = 2d * Math.Sqrt(adjustedC1 * adjustedC2) * Math.Sin(DegreesToRadians(deltaHPrime / 2d));

        var averageLPrime = (leftLab.L + rightLab.L) / 2d;
        var averageHPrime = CalculateAverageHuePrime(adjustedC1, adjustedC2, h1Prime, h2Prime);

        var t = 1d -
                (0.17d * Math.Cos(DegreesToRadians(averageHPrime - 30d))) +
                (0.24d * Math.Cos(DegreesToRadians(2d * averageHPrime))) +
                (0.32d * Math.Cos(DegreesToRadians((3d * averageHPrime) + 6d))) -
                (0.20d * Math.Cos(DegreesToRadians((4d * averageHPrime) - 63d)));

        var deltaTheta = 30d * Math.Exp(-Math.Pow((averageHPrime - 275d) / 25d, 2d));
        var adjustedAverageCToSeventh = Math.Pow(adjustedAverageC, 7d);
        var rC = 2d * Math.Sqrt(adjustedAverageCToSeventh / (adjustedAverageCToSeventh + Math.Pow(25d, 7d)));
        var sL = 1d + ((0.015d * Math.Pow(averageLPrime - 50d, 2d)) / Math.Sqrt(20d + Math.Pow(averageLPrime - 50d, 2d)));
        var sC = 1d + (0.045d * adjustedAverageC);
        var sH = 1d + (0.015d * adjustedAverageC * t);
        var rT = -Math.Sin(DegreesToRadians(2d * deltaTheta)) * rC;

        var deltaLTerm = deltaLPrime / sL;
        var deltaCTerm = deltaCPrime / sC;
        var deltaHTerm = deltaBigHPrime / sH;

        return Math.Sqrt(
            (deltaLTerm * deltaLTerm) +
            (deltaCTerm * deltaCTerm) +
            (deltaHTerm * deltaHTerm) +
            (rT * deltaCTerm * deltaHTerm));
    }

    private static double CalculateHueAngle(double adjustedA, double b)
    {
        if (adjustedA == 0d && b == 0d)
        {
            return 0d;
        }

        var hueAngle = RadiansToDegrees(Math.Atan2(b, adjustedA));
        return hueAngle >= 0d ? hueAngle : hueAngle + 360d;
    }

    private static double CalculateDeltaHPrime(double adjustedC1, double adjustedC2, double h1Prime, double h2Prime)
    {
        if (adjustedC1 == 0d || adjustedC2 == 0d)
        {
            return 0d;
        }

        var hueDifference = h2Prime - h1Prime;

        if (Math.Abs(hueDifference) <= 180d)
        {
            return hueDifference;
        }

        return hueDifference > 180d
            ? hueDifference - 360d
            : hueDifference + 360d;
    }

    private static double CalculateAverageHuePrime(double adjustedC1, double adjustedC2, double h1Prime, double h2Prime)
    {
        if (adjustedC1 == 0d || adjustedC2 == 0d)
        {
            return h1Prime + h2Prime;
        }

        if (Math.Abs(h1Prime - h2Prime) <= 180d)
        {
            return (h1Prime + h2Prime) / 2d;
        }

        return (h1Prime + h2Prime) < 360d
            ? (h1Prime + h2Prime + 360d) / 2d
            : (h1Prime + h2Prime - 360d) / 2d;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180d);
    }

    private static double RadiansToDegrees(double radians)
    {
        return radians * (180d / Math.PI);
    }
}