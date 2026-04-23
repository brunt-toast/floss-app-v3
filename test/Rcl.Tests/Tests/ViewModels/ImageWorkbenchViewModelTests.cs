using Rcl.ViewModels;

namespace Rcl.Tests.Tests.ViewModels;

[TestClass]
public class ImageWorkbenchViewModelTests
{
    [TestMethod]
    [DataRow(1, 1.0, 1.0)]
    [DataRow(3, 1.0, 1.0)]
    [DataRow(5, 0.5, 100.0)]
    [DataRow(10, 0.1, 999.0)]
    public void TotalSpools_IsAtLeast_NumberOfDistinctColors(int distinctColorCount, double stitchLength, double spoolLength)
    {
        var pixelCounts = Enumerable.Repeat(1, distinctColorCount).ToArray();

        var totalSpools = pixelCounts.Sum(pixels =>
            ImageWorkbenchViewModel.CalculateSpoolsForColor(pixels, stitchLength, spoolLength));

        Assert.IsGreaterThanOrEqualTo(totalSpools, distinctColorCount,
            $"Expected total spools ({totalSpools}) >= distinct colors ({distinctColorCount})");
    }

    [TestMethod]
    [DataRow(1, 1.0, 1.0, 1)]
    [DataRow(1, 2.0, 1.0, 2)]
    [DataRow(10, 1.0, 5.0, 2)]
    [DataRow(10, 1.0, 11.0, 1)]
    [DataRow(0, 1.0, 1.0, 1)]
    public void CalculateSpoolsForColor_ReturnsExpected(int pixelCount, double stitchLength, double spoolLength, int expected)
    {
        var result = ImageWorkbenchViewModel.CalculateSpoolsForColor(pixelCount, stitchLength, spoolLength);

        Assert.AreEqual(expected, result);
    }
}
