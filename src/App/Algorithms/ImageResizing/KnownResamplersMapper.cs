using App.Enums;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace App.Algorithms.ImageResizing;

internal static class KnownResamplersMapper
{
    public static IResampler Map(ImageSharpKnownResamplers resampler)
    {
        return resampler switch
        {
            ImageSharpKnownResamplers.Lanczos8 => KnownResamplers.Lanczos8,
            ImageSharpKnownResamplers.Bicubic => KnownResamplers.Bicubic,
            ImageSharpKnownResamplers.Box => KnownResamplers.Box,
            ImageSharpKnownResamplers.CatmullRom => KnownResamplers.CatmullRom,
            ImageSharpKnownResamplers.Hermite => KnownResamplers.Hermite,
            ImageSharpKnownResamplers.Lanczos2 => KnownResamplers.Lanczos2,
            ImageSharpKnownResamplers.Lanczos3 => KnownResamplers.Lanczos3,
            ImageSharpKnownResamplers.Lanczos5 => KnownResamplers.Lanczos5,
            ImageSharpKnownResamplers.MitchellNetravali => KnownResamplers.MitchellNetravali,
            ImageSharpKnownResamplers.NearestNeighbor => KnownResamplers.NearestNeighbor,
            ImageSharpKnownResamplers.Robidoux => KnownResamplers.Robidoux,
            ImageSharpKnownResamplers.RobidouxSharp => KnownResamplers.RobidouxSharp,
            ImageSharpKnownResamplers.Spline => KnownResamplers.Spline,
            ImageSharpKnownResamplers.Triangle => KnownResamplers.Triangle,
            ImageSharpKnownResamplers.Welch => KnownResamplers.Welch,
            _ => throw new ArgumentOutOfRangeException(nameof(resampler), resampler, "Unsupported ImageSharp resampler.")
        };
    }
}