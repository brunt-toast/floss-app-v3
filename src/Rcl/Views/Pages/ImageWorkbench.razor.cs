using App.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rcl.Resources.Languages;
using Rcl.Views.Components;
using Rcl.ViewModels;
using System.ComponentModel;

namespace Rcl.Views.Pages;

public partial class ImageWorkbench : IDisposable
{
    [Inject] public IImageWorkbenchViewModel ViewModel { get; set; } = default!;
    [Inject] public IDialogService DialogService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitializeAsync();
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

    private Task OnScaleChanged(double value)
    {
        ViewModel.Scale = Math.Clamp(value, 0.01, 1.0);
        return Task.CompletedTask;
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

    private Task OnColorSetChanged(string? value)
    {
        ViewModel.SelectedColorSetKey = value ?? string.Empty;
        return Task.CompletedTask;
    }

    private Task OnColorFidelityChanged(int value)
    {
        var max = Math.Max(1, ViewModel.MaximumColorFidelity);
        ViewModel.SelectedColorFidelity = Math.Clamp(value, 1, max);
        return Task.CompletedTask;
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
        var totalSpools = ViewModel.StitchLength > 0 && ViewModel.SpoolLength > 0
            ? ViewModel.ColorUsage.Sum(entry => ImageWorkbenchViewModel.CalculateSpoolsForColor(entry.PixelCount, ViewModel.StitchLength, ViewModel.SpoolLength)).ToString("N0")
            : ImageWorkbenchResources.EmptyValue;

        return string.Format(
            ImageWorkbenchResources.ColorUsageSummaryFormat,
            GetSelectedProfileName(),
            ViewModel.ColorUsage.Count,
            totalSpools);
    }

    private string GetSelectedProfileName()
    {
        return ViewModel.ColorSets
            .FirstOrDefault(profile => string.Equals(profile.Key, ViewModel.SelectedColorSetKey, StringComparison.Ordinal))
            ?.Name
            ?? ViewModel.SelectedColorSetKey;
    }

    private int GetSelectedColorFidelityValue()
    {
        return Math.Max(1, ViewModel.SelectedColorFidelity);
    }

    private string GetSpoolsToBuyText(int pixelCount)
    {
        if (ViewModel.StitchLength <= 0 || ViewModel.SpoolLength <= 0)
            return ImageWorkbenchResources.EmptyValue;

        return ImageWorkbenchViewModel.CalculateSpoolsForColor(pixelCount, ViewModel.StitchLength, ViewModel.SpoolLength).ToString("N0");
    }

    private async Task ShowColorDetailsAsync(ImageWorkbenchColorUsageItem colorUsage)
    {
        var parameters = new DialogParameters
        {
            { nameof(ColorDetailDialog.ColorUsage), colorUsage },
            { nameof(ColorDetailDialog.SchemeName), GetSelectedProfileName() }
        };
        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.ExtraSmall,
            FullWidth = true
        };

        await DialogService.ShowAsync<ColorDetailDialog>(ColorDetailDialogResources.Title, parameters, options);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_renderPending)
            return;
        _renderPending = true;
        _ = InvokeAsync(() =>
        {
            _renderPending = false;
            StateHasChanged();
        });
    }

    private bool _renderPending;
}
