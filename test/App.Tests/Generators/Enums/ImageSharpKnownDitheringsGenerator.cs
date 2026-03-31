using App.Enums;

namespace App.Tests.Generators.Enums;

internal static class ImageSharpKnownDitheringsGenerator
{
    public static IEnumerable<object[]> GetImageSharpKnownDitherings()
    {
        return Enum
            .GetValues<ImageSharpKnownDitherings>()
            .Select(dithering => new object[] { dithering });
    }
}