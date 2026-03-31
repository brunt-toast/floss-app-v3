using App.Enums;

namespace App.Tests.Generators.Enums;

internal class ColorComparisonAlgorithmsGenerator
{
    public static IEnumerable<object[]> GetColorComparisonAlgorithms()
    {
        return Enum
            .GetValues<ColorComparisonAlgorithms>()
            .Select(algorithm => new object[] { algorithm });
    }
}