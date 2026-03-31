using App.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace App.Services.ImageResizing;

public interface IImageResizingService
{
    Image<Rgba32> ResizeWidth(
        Image<Rgba32> image,
        int newWidth,
        ImageSharpKnownResamplers resampler,
        ImageSharpKnownDitherings dithering,
        byte transparencyThreshold);

    Image<Rgba32> ResizeHeight(
        Image<Rgba32> image,
        int newHeight,
        ImageSharpKnownResamplers resampler,
        ImageSharpKnownDitherings dithering,
        byte transparencyThreshold);
}