using App.Enums;
using App.Services.ColorMatching;
using App.Tests.Generators.ColorMatching;
using App.Tests.Generators.Enums;
using System.Drawing;

namespace App.Tests.Tests.Services;

[TestClass]
public class ColorMatchingServiceTests
{
	[TestMethod]
	[DynamicData(
		nameof(ColorComparisonAlgorithmsGenerator.GetColorComparisonAlgorithms),
		typeof(ColorComparisonAlgorithmsGenerator))]
	public void GetMostSimilarColors_Handles_All_Enum_Values(ColorComparisonAlgorithms algorithm)
	{
		var service = new ColorMatchingService();
		var source = Color.FromArgb(255, 110, 140, 170);
		var options = new[]
		{
			Color.FromArgb(255, 110, 140, 170),
			Color.FromArgb(255, 100, 130, 160),
			Color.FromArgb(255, 20, 40, 60),
			Color.FromArgb(255, 210, 220, 230)
		};

		var result = service.GetMostSimilarColors(source, options, algorithm);

		Assert.HasCount(options.Length, result);
		CollectionAssert.AreEquivalent(options, result.ToArray());
		Assert.AreEqual(source, result[0]);
	}

	[TestMethod]
	[DynamicData(
		nameof(KnownColorMatchingExamplesGenerator.GetKnownColorMatchingExamples),
		typeof(KnownColorMatchingExamplesGenerator))]
	public void GetMostSimilarColors_KnownExamples_Regression(
		ColorComparisonAlgorithms algorithm,
		string sourceHex,
		string[] optionHexes,
		string[] expectedHexOrder)
	{
		var service = new ColorMatchingService();
		var source = ParseHexColor(sourceHex);
		var options = optionHexes.Select(ParseHexColor).ToArray();

		var result = service.GetMostSimilarColors(source, options, algorithm);
		var resultHexOrder = result.Select(ToHex).ToArray();

		CollectionAssert.AreEqual(expectedHexOrder, resultHexOrder);
	}

	private static Color ParseHexColor(string value)
	{
		return ColorTranslator.FromHtml(value);
	}

	private static string ToHex(Color color)
	{
		return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
	}
}