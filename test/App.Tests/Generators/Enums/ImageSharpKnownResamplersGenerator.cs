using App.Enums;

namespace App.Tests.Generators.Enums;

internal static class ImageSharpKnownResamplersGenerator
{
    public static IEnumerable<object[]> GetImageSharpKnownResamplers()
    {
        return Enum
            .GetValues<ImageSharpKnownResamplers>()
            .Select(resampler => new object[] { resampler });
    }
}