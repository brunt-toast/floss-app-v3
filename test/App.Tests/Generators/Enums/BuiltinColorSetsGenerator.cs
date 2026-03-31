using App.Enums;

namespace App.Tests.Generators.Enums;

internal static class BuiltinColorSetsGenerator
{
    public static IEnumerable<object[]> GetBuiltinColorSets()
    {
        return Enum
            .GetValues<BuiltinColorSets>()
            .Select(set => new object[] { set });
    }
}