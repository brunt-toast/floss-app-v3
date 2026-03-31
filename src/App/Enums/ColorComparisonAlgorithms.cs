using System.ComponentModel.DataAnnotations;

namespace App.Enums;

public enum ColorComparisonAlgorithms
{
    [Display(Name = "CIEDE2000", Description = "The slowest but best option. A.K.A. \u0394E\u2080\u2080.")]
    Ciede2000,

    [Display(Name="Weighted Euclidean RGB", Description = "A strong balance between speed and accuracy.")]
    EuclideanWeightedRgb,

    [Display(Name="Euclidean RGB", Description = "Slower than Manhattan Distance with slightly better results.")]
    EuclideanRgb,

    [Display(Name = "Manhattan Distance RGB", Description = "The fastest option, but with the least accurate results. A.K.A. L1 or city-block distance. ")]
    ManhattanDistanceRgb,

    [Display(Name="CIE76", Description = "Slower and better than weighted Euclidean, but struggles in blue, gray, and low-chroma regions.")]
    Cie76,

    [Display(Name="CIE94 for Graphics", Description = "Slower and better than CIE76. Weights optimised for graphic arts, but struggles in blue, gray, and low-chroma regions.")]
    Cie94Graphics,

    [Display(Name = "CIE94 for Textiles", Description = "Slower and better than CIE76. Weights optimised for textile arts, but struggles in blue, gray, and low-chroma regions.")]
    Cie94Textiles
}
