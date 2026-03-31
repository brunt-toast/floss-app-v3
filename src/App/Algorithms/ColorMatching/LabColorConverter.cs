using System.Drawing;

namespace App.Algorithms.ColorMatching;

internal static class LabColorConverter
{
    private const double Epsilon = 216d / 24389d;
    private const double Kappa = 24389d / 27d;
    private const double ReferenceWhiteX = 0.95047d;
    private const double ReferenceWhiteY = 1.00000d;
    private const double ReferenceWhiteZ = 1.08883d;

    public static LabColor ToLab(Color color)
    {
        var red = PivotRgb(color.R / 255d);
        var green = PivotRgb(color.G / 255d);
        var blue = PivotRgb(color.B / 255d);

        var x = (red * 0.4124564d) + (green * 0.3575761d) + (blue * 0.1804375d);
        var y = (red * 0.2126729d) + (green * 0.7151522d) + (blue * 0.0721750d);
        var z = (red * 0.0193339d) + (green * 0.1191920d) + (blue * 0.9503041d);

        var fx = PivotXyz(x / ReferenceWhiteX);
        var fy = PivotXyz(y / ReferenceWhiteY);
        var fz = PivotXyz(z / ReferenceWhiteZ);

        return new LabColor(
            (116d * fy) - 16d,
            500d * (fx - fy),
            200d * (fy - fz));
    }

    private static double PivotRgb(double channel)
    {
        return channel <= 0.04045d
            ? channel / 12.92d
            : Math.Pow((channel + 0.055d) / 1.055d, 2.4d);
    }

    private static double PivotXyz(double channel)
    {
        return channel > Epsilon
            ? Math.Cbrt(channel)
            : ((Kappa * channel) + 16d) / 116d;
    }
}