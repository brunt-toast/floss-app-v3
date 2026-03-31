using System.ComponentModel.DataAnnotations;

namespace App.Enums;

public enum ImageSharpKnownDitherings
{
    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.FloydSteinberg" />
    [Display(Name = "Floyd-Steinberg", Description = "Balanced speed and quality.")]
    FloydSteinberg,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Bayer2x2" />
    [Display(Name="Bayer 2X2", Description = "Very fast, but low quality.")]
    Bayer2X2,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Ordered3x3" />
    [Display(Name="Ordered 3X3", Description = "More detailed than Bayer 2X2, and similarly fast, but still blocky.")]
    Ordered3X3,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Bayer4x4" />
    [Display(Name="Bayer 4X4", Description = "Slightly slower and more detailed than lower Bayer ditherings.")]
    Bayer4X4,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Bayer8x8" />
    [Display(Name="Bayer 8X8", Description = "Slightly slower and more detailed than lower Bayer ditherings.")]
    Bayer8X8,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Bayer16x16" />
    [Display(Name="Bayer 16X16", Description = "Slightly slower and more detailed than lower Bayer ditherings.")]
    Bayer16X16,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Atkinson" />
    [Display(Description = "Fast and clean, producing a soft image.")]
    Atkinson,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Burks" />
    [Display(Description = "Slightly slower and smoother than Atkinson.")]
    Burks,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.JarvisJudiceNinke" />
    [Display(Name="Jarvis-Judice-Ninke", Description = "Slightly slower and smoother than Floyd-Steinberg.")]
    JarvisJudiceNinke,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Sierra2" />
    [Display(Name="Sierra-2", Description = "Balanced speed and quality. Faster than Jarvis, smoother than Atkinson.")]
    Sierra2,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Sierra3" />
    [Display(Name="Sierra-3", Description = "High quality, but a bit slow.")]
    Sierra3,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.SierraLite" />
    [Display(Name="Sierra-Lite", Description = "Very fast, but less quality than full Sierra.")]
    SierraLite,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.StevensonArce" />
    [Display(Name="Stevenson-Acre", Description = "Slow, but high quality.")]
    StevensonArce,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownDitherings.Stucki" />
    [Display(Description = "Similar quality to Jarvis, but faster.")]
    Stucki,
}
