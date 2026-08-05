using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Rcl.Services.Framing;

internal static class FramingImageRenderer
{
    public static async Task<byte[]> RenderAsync(
        byte[] sourceImageContent,
        double scale,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceImageContent);

        await using var sourceStream = new MemoryStream(sourceImageContent, writable: false);
        using var source = await Image.LoadAsync<Rgba32>(sourceStream, cancellationToken);

        var maximumImageSide = Math.Max(source.Width, source.Height);
        var canvasSide = Math.Max(maximumImageSide, (int)Math.Ceiling(maximumImageSide / scale));
        var x = (canvasSide - source.Width) / 2;
        var y = (canvasSide - source.Height) / 2;

        using var canvas = new Image<Rgba32>(canvasSide, canvasSide, new Rgba32(0, 0, 0, 0));
        canvas.Mutate(context => context.DrawImage(source, new Point(x, y), 1f));
        DarkenOutsideCircle(canvas);

        await using var outputStream = new MemoryStream();
        await canvas.SaveAsync(outputStream, new PngEncoder(), cancellationToken);

        return outputStream.ToArray();
    }

    private static void DarkenOutsideCircle(Image<Rgba32> image)
    {
        var radius = image.Width / 2.0;
        var center = radius;
        var radiusSquared = radius * radius;

        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                var dy = y + 0.5 - center;

                for (var x = 0; x < row.Length; x++)
                {
                    var dx = x + 0.5 - center;
                    if (dx * dx + dy * dy <= radiusSquared)
                    {
                        continue;
                    }

                    row[x].R = (byte)(row[x].R / 2);
                    row[x].G = (byte)(row[x].G / 2);
                    row[x].B = (byte)(row[x].B / 2);
                    row[x].A = (byte)(row[x].A + ((byte.MaxValue - row[x].A + 1) / 2));
                }
            }
        });
    }
}
