using App.Enums;
using Microsoft.AspNetCore.Components;
using Rcl.Resources.Languages;
using Rcl.ViewModels;
using System.ComponentModel;

namespace Rcl.Views.Pages;

public partial class ImageWorkbench : IDisposable
{
    [Inject] public IImageWorkbenchViewModel ViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public void Dispose()
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private async Task LoadFromDeviceAsync()
    {
        await ViewModel.LoadFromDeviceAsync();
    }

    private async Task SaveResultAsync()
    {
        await ViewModel.SaveResultAsync();
    }

    private Task OnTransparencyChanged(int value)
    {
        ViewModel.TransparencyThreshold = (byte)Math.Clamp(value, byte.MinValue, byte.MaxValue);
        return Task.CompletedTask;
    }

    private Task OnResamplerChanged(ImageSharpKnownResamplers value)
    {
        ViewModel.SelectedResampler = value;
        return Task.CompletedTask;
    }

    private bool CanUseResampler(ImageSharpKnownResamplers value)
    {
        return ViewModel.Resamplers.Contains(value);
    }

    private Task OnDitheringChanged(ImageSharpKnownDitherings value)
    {
        ViewModel.SelectedDithering = value;
        return Task.CompletedTask;
    }

    private bool CanUseDithering(ImageSharpKnownDitherings value)
    {
        return ViewModel.Ditherings.Contains(value);
    }

    private Task OnColorSetChanged(BuiltinColorSets value)
    {
        ViewModel.SelectedColorSet = value;
        return Task.CompletedTask;
    }

    private Task OnColorFidelityChanged(int value)
    {
        ViewModel.SelectedColorFidelity = value;
        return Task.CompletedTask;
    }

    private bool CanUseColorSet(BuiltinColorSets value)
    {
        return ViewModel.ColorSets.Contains(value);
    }

    private Task OnComparisonAlgorithmChanged(ColorComparisonAlgorithms value)
    {
        ViewModel.SelectedComparisonAlgorithm = value;
        return Task.CompletedTask;
    }

    private bool CanUseComparisonAlgorithm(ColorComparisonAlgorithms value)
    {
        return ViewModel.ComparisonAlgorithms.Contains(value);
    }

    private string GetColorUsageSummary()
    {
        return string.Format(
            ImageWorkbenchResources.ColorUsageSummaryFormat,
            ImageWorkbenchViewModel.GetDisplayName(ViewModel.SelectedColorSet),
            ViewModel.ColorUsage.Count,
            ViewModel.ColorUsage.Sum(entry => entry.PixelCount).ToString("N0"));
    }

    private int GetSelectedColorFidelityValue()
    {
        return Math.Max(1, ViewModel.SelectedColorFidelity);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _ = InvokeAsync(StateHasChanged);
    }
}
