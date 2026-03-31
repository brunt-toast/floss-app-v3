using App.Enums;
using App.Services.ColorMatching;
using App.Services.ColorReduction;
using App.Services.ColorSets;
using App.Tests.Generators.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace App.Tests.Tests.Services;

[TestClass]
public class BuiltinColorReductionServiceTests
{
    [TestMethod]
    [DynamicData(
        nameof(BuiltinColorSetsGenerator.GetBuiltinColorSets),
        typeof(BuiltinColorSetsGenerator))]
    public void ReduceColors_Maps_All_Pixels_To_Target_Set(object setValue)
    {
        var set = (BuiltinColorSets)setValue;
        var colorSetService = new BuiltinColorSetService();
        var service = new BuiltinColorReductionService(colorSetService, new ColorMatchingService());

        using var source = new Image<Rgba32>(3, 1);
        source[0, 0] = new Rgba32(12, 34, 56, 255);
        source[1, 0] = new Rgba32(120, 200, 14, 200);
        source[2, 0] = new Rgba32(250, 10, 220, 128);

        using var reduced = service.ReduceColors(source, set, ColorComparisonAlgorithms.Ciede2000);

        var allowed = colorSetService
            .GetColors(set)
            .Select(c => (c.Color.R, c.Color.G, c.Color.B))
            .ToHashSet();

        Assert.Contains((reduced[0, 0].R, reduced[0, 0].G, reduced[0, 0].B), allowed);
        Assert.Contains((reduced[1, 0].R, reduced[1, 0].G, reduced[1, 0].B), allowed);
        Assert.Contains((reduced[2, 0].R, reduced[2, 0].G, reduced[2, 0].B), allowed);
    }

    [TestMethod]
    public void ReduceColors_Preserves_Alpha_Channel()
    {
        var service = new BuiltinColorReductionService(new BuiltinColorSetService(), new ColorMatchingService());

        using var source = new Image<Rgba32>(2, 1);
        source[0, 0] = new Rgba32(10, 20, 30, 7);
        source[1, 0] = new Rgba32(40, 50, 60, 201);

        using var reduced = service.ReduceColors(source, BuiltinColorSets.Html, ColorComparisonAlgorithms.Ciede2000);

        Assert.AreEqual((byte)7, reduced[0, 0].A);
        Assert.AreEqual((byte)201, reduced[1, 0].A);
    }

    [TestMethod]
    public void ReduceColors_Does_Not_Mutate_Source_Image()
    {
        var service = new BuiltinColorReductionService(new BuiltinColorSetService(), new ColorMatchingService());

        using var source = new Image<Rgba32>(1, 1);
        source[0, 0] = new Rgba32(1, 2, 3, 4);

        using var reduced = service.ReduceColors(source, BuiltinColorSets.Html, ColorComparisonAlgorithms.Ciede2000);

        Assert.AreEqual(new Rgba32(1, 2, 3, 4), source[0, 0]);
        Assert.AreNotEqual(source[0, 0], reduced[0, 0]);
    }
}