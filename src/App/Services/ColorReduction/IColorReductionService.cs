using App.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace App.Services.ColorReduction;

public sealed class ColorReductionResult(Image<Rgba32> image, int availableColorCount) : IDisposable
{
    public Image<Rgba32> Image { get; } = image;
    public int AvailableColorCount { get; } = availableColorCount;

    public void Dispose()
    {
        Image.Dispose();
    }
}

public interface IColorReductionService
{
    ColorReductionResult ReduceColors(
        Image<Rgba32> source,
        BuiltinColorSets set,
        ColorComparisonAlgorithms comparisonAlgorithm,
        int? maxColors = null);
}