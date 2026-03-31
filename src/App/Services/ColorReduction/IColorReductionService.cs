using App.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace App.Services.ColorReduction;

public interface IColorReductionService
{
    Image<Rgba32> ReduceColors(
        Image<Rgba32> source,
        BuiltinColorSets set,
        ColorComparisonAlgorithms comparisonAlgorithm);
}