using App.Enums;

namespace App.Tests.Generators.ColorMatching;

internal static class KnownColorMatchingExamplesGenerator
{
    public static IEnumerable<object[]> GetKnownColorMatchingExamples()
    {
        var sourceHex = "#7A5131";
        var optionsHex = new[]
        {
            "#7A5131",
            "#7B5131",
            "#7A5331",
            "#7A5134",
            "#141414",
            "#FAFAFA"
        };

        yield return new object[]
        {
            ColorComparisonAlgorithms.Ciede2000,
            sourceHex,
            optionsHex,
            new[] { "#7A5131", "#7B5131", "#7A5134", "#7A5331", "#141414", "#FAFAFA" }
        };

        yield return new object[]
        {
            ColorComparisonAlgorithms.EuclideanWeightedRgb,
            sourceHex,
            optionsHex,
            new[] { "#7A5131", "#7B5131", "#7A5331", "#7A5134", "#141414", "#FAFAFA" }
        };

        yield return new object[]
        {
            ColorComparisonAlgorithms.EuclideanRgb,
            sourceHex,
            optionsHex,
            new[] { "#7A5131", "#7B5131", "#7A5331", "#7A5134", "#141414", "#FAFAFA" }
        };

        yield return new object[]
        {
            ColorComparisonAlgorithms.ManhattanDistanceRgb,
            sourceHex,
            optionsHex,
            new[] { "#7A5131", "#7B5131", "#7A5331", "#7A5134", "#141414", "#FAFAFA" }
        };

        yield return new object[]
        {
            ColorComparisonAlgorithms.Cie76,
            sourceHex,
            optionsHex,
            new[] { "#7A5131", "#7B5131", "#7A5331", "#7A5134", "#141414", "#FAFAFA" }
        };

        yield return new object[]
        {
            ColorComparisonAlgorithms.Cie94Graphics,
            sourceHex,
            optionsHex,
            new[] { "#7A5131", "#7B5131", "#7A5134", "#7A5331", "#141414", "#FAFAFA" }
        };

        yield return new object[]
        {
            ColorComparisonAlgorithms.Cie94Textiles,
            sourceHex,
            optionsHex,
            new[] { "#7A5131", "#7B5131", "#7A5134", "#7A5331", "#141414", "#FAFAFA" }
        };
    }
}