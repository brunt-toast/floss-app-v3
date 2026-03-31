using App.Algorithms.ImageResizing;
using App.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace App.Services.ImageResizing;

internal sealed class ImageResizingService : IImageResizingService
{
    public Image<Rgba32> ResizeWidth(
        Image<Rgba32> image,
        int newWidth,
        ImageSharpKnownResamplers resampler,
        ImageSharpKnownDitherings dithering,
        byte transparencyThreshold)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (newWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newWidth), newWidth, "Width must be greater than zero.");
        }

        var computedHeight = (int)Math.Round(image.Height * (newWidth / (double)image.Width));
        return ResizeInternal(image, newWidth, Math.Max(1, computedHeight), resampler, dithering, transparencyThreshold);
    }

    public Image<Rgba32> ResizeHeight(
        Image<Rgba32> image,
        int newHeight,
        ImageSharpKnownResamplers resampler,
        ImageSharpKnownDitherings dithering,
        byte transparencyThreshold)
    {
        ArgumentNullException.ThrowIfNull(image);

        if (newHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newHeight), newHeight, "Height must be greater than zero.");
        }

        var computedWidth = (int)Math.Round(image.Width * (newHeight / (double)image.Height));
        return ResizeInternal(image, Math.Max(1, computedWidth), newHeight, resampler, dithering, transparencyThreshold);
    }

    private static Image<Rgba32> ResizeInternal(
        Image<Rgba32> image,
        int targetWidth,
        int targetHeight,
        ImageSharpKnownResamplers resampler,
        ImageSharpKnownDitherings dithering,
        byte transparencyThreshold)
    {
        var output = image.Clone();
        var sampler = KnownResamplersMapper.Map(resampler);
        var dither = KnownDitheringsMapper.Map(dithering);

        output.Mutate(context =>
        {
            context.Resize(new ResizeOptions
            {
                Sampler = sampler,
                Size = new Size(targetWidth, targetHeight),
                Mode = ResizeMode.Stretch
            });

            context.Dither(dither);
        });

        if (transparencyThreshold > 0)
        {
            output.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    for (var x = 0; x < row.Length; x++)
                    {
                        if (row[x].A < transparencyThreshold)
                        {
                            row[x].A = 0;
                        }
                    }
                }
            });
        }

        return output;
    }
}