using App.Enums;
using App.Services.ImageResizing;
using App.Tests.Generators.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace App.Tests.Tests.Services;

[TestClass]
public class ImageResizingServiceTests
{
    [TestMethod]
    [DynamicData(
        nameof(ImageSharpKnownResamplersGenerator.GetImageSharpKnownResamplers),
        typeof(ImageSharpKnownResamplersGenerator))]
    public void ResizeWidth_Handles_All_Resamplers(ImageSharpKnownResamplers resampler)
    {
        var service = new ImageResizingService();
        using var source = CreateSampleImage(width: 160, height: 90);

        using var result = service.ResizeWidth(
            image: source,
            newWidth: 80,
            resampler: resampler,
            dithering: ImageSharpKnownDitherings.FloydSteinberg,
            transparencyThreshold: 0);

        Assert.AreEqual(80, result.Width);
        Assert.AreEqual(45, result.Height);
    }

    [TestMethod]
    [DynamicData(
        nameof(ImageSharpKnownDitheringsGenerator.GetImageSharpKnownDitherings),
        typeof(ImageSharpKnownDitheringsGenerator))]
    public void ResizeHeight_Handles_All_Ditherings(ImageSharpKnownDitherings dithering)
    {
        var service = new ImageResizingService();
        using var source = CreateSampleImage(width: 160, height: 90);

        using var result = service.ResizeHeight(
            image: source,
            newHeight: 45,
            resampler: ImageSharpKnownResamplers.Bicubic,
            dithering: dithering,
            transparencyThreshold: 0);

        Assert.AreEqual(80, result.Width);
        Assert.AreEqual(45, result.Height);
    }

    [TestMethod]
    public void ResizeWidth_Preserves_Aspect_Ratio()
    {
        var service = new ImageResizingService();
        using var source = CreateSampleImage(width: 400, height: 200);

        using var result = service.ResizeWidth(
            image: source,
            newWidth: 100,
            resampler: ImageSharpKnownResamplers.Lanczos3,
            dithering: ImageSharpKnownDitherings.Atkinson,
            transparencyThreshold: 0);

        Assert.AreEqual(100, result.Width);
        Assert.AreEqual(50, result.Height);
    }

    [TestMethod]
    public void ResizeHeight_Preserves_Aspect_Ratio()
    {
        var service = new ImageResizingService();
        using var source = CreateSampleImage(width: 400, height: 200);

        using var result = service.ResizeHeight(
            image: source,
            newHeight: 80,
            resampler: ImageSharpKnownResamplers.RobidouxSharp,
            dithering: ImageSharpKnownDitherings.Stucki,
            transparencyThreshold: 0);

        Assert.AreEqual(160, result.Width);
        Assert.AreEqual(80, result.Height);
    }

    [TestMethod]
    public void ResizeWidth_When_Width_Is_Not_Positive_Throws()
    {
        var service = new ImageResizingService();
        var source = CreateSampleImage(width: 100, height: 100);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.ResizeWidth(
            image: source,
            newWidth: 0,
            resampler: ImageSharpKnownResamplers.Bicubic,
            dithering: ImageSharpKnownDitherings.FloydSteinberg,
            transparencyThreshold: 0));
    }

    [TestMethod]
    public void ResizeHeight_When_Height_Is_Not_Positive_Throws()
    {
        var service = new ImageResizingService();
        var source = CreateSampleImage(width: 100, height: 100);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.ResizeHeight(
            image: source,
            newHeight: 0,
            resampler: ImageSharpKnownResamplers.Bicubic,
            dithering: ImageSharpKnownDitherings.FloydSteinberg,
            transparencyThreshold: 0));
    }

    [TestMethod]
    public void ResizeWidth_Applies_Transparency_Threshold()
    {
        var service = new ImageResizingService();

        using var source = new Image<Rgba32>(4, 1);
        source[0, 0] = new Rgba32(10, 10, 10, 49);
        source[1, 0] = new Rgba32(10, 10, 10, 50);
        source[2, 0] = new Rgba32(10, 10, 10, 51);
        source[3, 0] = new Rgba32(10, 10, 10, 200);

        using var result = service.ResizeWidth(
            image: source,
            newWidth: 4,
            resampler: ImageSharpKnownResamplers.NearestNeighbor,
            dithering: ImageSharpKnownDitherings.Atkinson,
            transparencyThreshold: 50);

        Assert.AreEqual((byte)0, result[0, 0].A);
    }

    private static Image<Rgba32> CreateSampleImage(int width, int height)
    {
        var image = new Image<Rgba32>(width, height);
        image.Mutate(context => context.BackgroundColor(new Rgba32(32, 96, 192, 255)));
        return image;
    }
}