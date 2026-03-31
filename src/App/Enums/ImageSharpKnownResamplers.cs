using System.ComponentModel.DataAnnotations;

namespace App.Enums;

public enum ImageSharpKnownResamplers
{
    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Lanczos8" />
    [Display(Name = "Lanczos 8px", Description = "Slightly slower and higher quality than lower Lanczos options.")]
    Lanczos8,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Bicubic" />
    [Display(Description = "Slow but high quality. Best for retaining quality in photos.")]
    Bicubic,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Box" />
    [Display(Description = "One of the fastest options, but produces blurry results.")]
    Box,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.CatmullRom" />
    [Display(Name="Catmull-Rom", Description = "Medium speed, producing sharp results. Best for images with clean lines.")]
    CatmullRom,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Hermite" />
    [Display(Description = "Slightly slower but smoother than Triangle. Produces softer images.")]
    Hermite,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Lanczos2" />
    [Display(Name="Lanczos 2px", Description = "Slightly slower and higher quality than Bicubic. Struggles with high-contrast edges.")]
    Lanczos2,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Lanczos3" />
    [Display(Name="Lanczos 3px", Description = "Slightly slower and higher quality than lower Lanczos options.")]
    Lanczos3,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Lanczos5" />
    [Display(Name="Lanczos 5px", Description = "Slightly slower and higher quality than lower Lanczos options.")]
    Lanczos5,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.MitchellNetravali" />
    [Display(Name = "Mitchell-Netravali", Description = "Balanced speed and quality.")]
    MitchellNetravali,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.NearestNeighbor" />
    [Display(Name="Nearest neighbor", Description = "The fastest option, but lowest quality. Good for pixel art.")]
    NearestNeighbor,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Robidoux" />
    [Display(Description = "Slightly softer than Robidoux Sharp, with fewer artifacts.")]
    Robidoux,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.RobidouxSharp" />
    [Display(Name="Robidoux Sharp", Description = "Slightly slower than Mitchell, but higher quality. Handles unclean lines better.")]
    RobidouxSharp,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Spline" />
    [Display(Description = "Slower but higher quality than Mitchell. Best for photorealistic results.")]
    Spline,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Triangle" />
    [Display(Description = "Slightly slower but smoother than Box. Produces softer images. A.K.A. Linear or bi-linear.")]
    Triangle,

    /// <inheritdoc cref="SixLabors.ImageSharp.Processing.KnownResamplers.Welch" />
    [Display(Description = "Slightly worse than Lanczos. Reduces sharp artifacts.")]
    Welch
}
