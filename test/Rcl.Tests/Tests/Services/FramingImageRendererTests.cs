using Rcl.Services.Framing;
using Rcl.ViewModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Rcl.Tests.Tests.Services;

[TestClass]
public class FramingImageRendererTests
{
    [TestMethod]
    public async Task RenderAsync_CentersImageOnSquareCanvas()
    {
        var source = await CreatePngAsync(2, 4, new Rgba32(100, 120, 140, 255));

        var content = await FramingImageRenderer.RenderAsync(source, FramingViewModel.MaximumScale);

        using var result = Image.Load<Rgba32>(content);

        Assert.AreEqual(4, result.Width);
        Assert.AreEqual(4, result.Height);
        Assert.AreEqual(128, result[0, 0].A);
        Assert.AreEqual(255, result[1, 0].A);
        Assert.AreEqual(255, result[2, 3].A);
        Assert.AreEqual(128, result[3, 3].A);
    }

    [TestMethod]
    public async Task RenderAsync_DarkensPixelsOutsideInscribedCircle()
    {
        var source = await CreatePngAsync(4, 4, new Rgba32(100, 120, 140, 255));

        var content = await FramingImageRenderer.RenderAsync(source, FramingViewModel.MaximumScale);

        using var result = Image.Load<Rgba32>(content);

        Assert.AreEqual(new Rgba32(50, 60, 70, 255), result[0, 0]);
        Assert.AreEqual(new Rgba32(100, 120, 140, 255), result[1, 1]);
        Assert.AreEqual(new Rgba32(100, 120, 140, 255), result[2, 2]);
        Assert.AreEqual(new Rgba32(50, 60, 70, 255), result[3, 3]);
    }

    [TestMethod]
    public async Task RenderAsync_DarkensTransparentCanvasOutsideInscribedCircle()
    {
        var source = await CreatePngAsync(2, 4, new Rgba32(100, 120, 140, 255));

        var content = await FramingImageRenderer.RenderAsync(source, FramingViewModel.MaximumScale);

        using var result = Image.Load<Rgba32>(content);

        Assert.AreEqual(new Rgba32(0, 0, 0, 128), result[0, 0]);
        Assert.AreEqual(new Rgba32(0, 0, 0, 128), result[3, 3]);
    }

    [TestMethod]
    public async Task RenderAsync_ExpandsCanvasAboutImageCenter()
    {
        var source = await CreatePngAsync(10, 10, new Rgba32(100, 120, 140, 255));

        var content = await FramingImageRenderer.RenderAsync(source, FramingViewModel.MinimumScale);

        using var result = Image.Load<Rgba32>(content);

        Assert.AreEqual(15, result.Width);
        Assert.AreEqual(15, result.Height);
        Assert.AreEqual(128, result[0, 0].A);
        Assert.AreEqual(255, result[2, 2].A);
        Assert.AreEqual(255, result[11, 11].A);
        Assert.AreEqual(128, result[14, 14].A);
    }

    [TestMethod]
    public async Task RenderAsync_DoesNotResampleSourceImageWhenScaleChanges()
    {
        var source = await CreateCheckerPngAsync();

        var content = await FramingImageRenderer.RenderAsync(source, FramingViewModel.MinimumScale);

        using var result = Image.Load<Rgba32>(content);

        Assert.AreEqual(new Rgba32(255, 0, 0, 255), result[0, 0]);
        Assert.AreEqual(new Rgba32(0, 255, 0, 255), result[1, 0]);
        Assert.AreEqual(new Rgba32(0, 0, 255, 255), result[0, 1]);
        Assert.AreEqual(new Rgba32(255, 255, 255, 255), result[1, 1]);
    }

    private static async Task<byte[]> CreatePngAsync(int width, int height, Rgba32 color)
    {
        using var image = new Image<Rgba32>(width, height, color);
        await using var stream = new MemoryStream();
        await image.SaveAsync(stream, new PngEncoder());
        return stream.ToArray();
    }

    private static async Task<byte[]> CreateCheckerPngAsync()
    {
        using var image = new Image<Rgba32>(2, 2);
        image[0, 0] = new Rgba32(255, 0, 0, 255);
        image[1, 0] = new Rgba32(0, 255, 0, 255);
        image[0, 1] = new Rgba32(0, 0, 255, 255);
        image[1, 1] = new Rgba32(255, 255, 255, 255);

        await using var stream = new MemoryStream();
        await image.SaveAsync(stream, new PngEncoder());
        return stream.ToArray();
    }
}
