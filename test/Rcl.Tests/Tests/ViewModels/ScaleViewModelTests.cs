using Rcl.ViewModels;

namespace Rcl.Tests.Tests.ViewModels;

[TestClass]
public class ScaleViewModelTests
{
    [TestMethod]
    [DataRow(100, 80, 14, 8)]
    [DataRow(80, 100, 14, 8)]
    [DataRow(28, 20, 14, 2)]
    [DataRow(0, 20, 14, 0)]
    [DataRow(20, 20, 0, 0)]
    public void CalculateMinimumHoopSizeInches_UsesLargestDimensionRoundedUp(
        int width,
        int height,
        int stitchCount,
        int expected)
    {
        var result = ScaleViewModel.CalculateMinimumHoopSizeInches(width, height, stitchCount);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow(100, 14, 7.142857142857143)]
    [DataRow(28, 14, 2.0)]
    [DataRow(0, 14, 0.0)]
    [DataRow(28, 0, 0.0)]
    public void CalculateImageDimensionInches_DoesNotRound(int pixels, int stitchCount, double expected)
    {
        var result = ScaleViewModel.CalculateImageDimensionInches(pixels, stitchCount);

        Assert.AreEqual(expected, result, 0.0000000001);
    }
}
