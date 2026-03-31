using App.Enums;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace App.Algorithms.ImageResizing;

internal static class KnownDitheringsMapper
{
    public static IDither Map(ImageSharpKnownDitherings dithering)
    {
        return dithering switch
        {
            ImageSharpKnownDitherings.FloydSteinberg => KnownDitherings.FloydSteinberg,
            ImageSharpKnownDitherings.Bayer2X2 => KnownDitherings.Bayer2x2,
            ImageSharpKnownDitherings.Ordered3X3 => KnownDitherings.Ordered3x3,
            ImageSharpKnownDitherings.Bayer4X4 => KnownDitherings.Bayer4x4,
            ImageSharpKnownDitherings.Bayer8X8 => KnownDitherings.Bayer8x8,
            ImageSharpKnownDitherings.Bayer16X16 => KnownDitherings.Bayer16x16,
            ImageSharpKnownDitherings.Atkinson => KnownDitherings.Atkinson,
            ImageSharpKnownDitherings.Burks => KnownDitherings.Burks,
            ImageSharpKnownDitherings.JarvisJudiceNinke => KnownDitherings.JarvisJudiceNinke,
            ImageSharpKnownDitherings.Sierra2 => KnownDitherings.Sierra2,
            ImageSharpKnownDitherings.Sierra3 => KnownDitherings.Sierra3,
            ImageSharpKnownDitherings.SierraLite => KnownDitherings.SierraLite,
            ImageSharpKnownDitherings.StevensonArce => KnownDitherings.StevensonArce,
            ImageSharpKnownDitherings.Stucki => KnownDitherings.Stucki,
            _ => throw new ArgumentOutOfRangeException(nameof(dithering), dithering, "Unsupported ImageSharp dithering algorithm.")
        };
    }
}