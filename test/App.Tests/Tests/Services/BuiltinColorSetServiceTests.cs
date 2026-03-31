using App.Enums;
using App.Services.ColorSets;
using App.Tests.Generators.Enums;

namespace App.Tests.Tests.Services;

[TestClass]
public class BuiltinColorSetServiceTests
{
    [TestMethod]
    [DynamicData(
        nameof(BuiltinColorSetsGenerator.GetBuiltinColorSets),
        typeof(BuiltinColorSetsGenerator))]
    public void GetColors_WithoutPredicate_Matches_TruePredicate(object setValue)
    {
        var set = (BuiltinColorSets)setValue;
        var service = new BuiltinColorSetService();

        var withoutPredicate = service.GetColors(set).ToArray();
        var withTruePredicate = service.GetColors(set, _ => true).ToArray();

        CollectionAssert.AreEqual(withTruePredicate, withoutPredicate);
    }

    [TestMethod]
    [DynamicData(
        nameof(BuiltinColorSetsGenerator.GetBuiltinColorSets),
        typeof(BuiltinColorSetsGenerator))]
    public void GetColors_WithPredicate_FiltersResults(object setValue)
    {
        var set = (BuiltinColorSets)setValue;
        var service = new BuiltinColorSetService();

        var filtered = service.GetColors(set, color => color.Color.R == 0).ToArray();

        Assert.IsTrue(filtered.All(color => color.Color.R == 0));
    }

    [TestMethod]
    [DynamicData(
        nameof(BuiltinColorSetsGenerator.GetBuiltinColorSets),
        typeof(BuiltinColorSetsGenerator))]
    public void GetColors_Returns_Data_For_Each_Set(object setValue)
    {
        var set = (BuiltinColorSets)setValue;
        var service = new BuiltinColorSetService();

        var colors = service.GetColors(set).Take(3).ToArray();

        Assert.IsNotEmpty(colors);
        Assert.IsTrue(colors.All(c => !string.IsNullOrWhiteSpace(c.Name)));
        Assert.IsTrue(colors.All(c => !string.IsNullOrWhiteSpace(c.Floss)));
    }
}