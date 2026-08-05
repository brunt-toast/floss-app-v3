using Microsoft.AspNetCore.Components;
using Rcl.Resources.Languages;
using Rcl.ViewModels;
using System.ComponentModel;

namespace Rcl.Views.Pages;

public partial class Scale : IDisposable
{
    [Inject] public IScaleViewModel ViewModel { get; set; } = default!;

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

    private Task OnStitchCountChanged(int value)
    {
        ViewModel.StitchCount = value;
        return Task.CompletedTask;
    }

    private string GetMinimumHoopSizeText()
    {
        return string.Format(ScaleResources.MinimumHoopSizeFormat, ViewModel.MinimumHoopSizeInches);
    }

    private string GetSampleImageStyle()
    {
        return $"width: {ScaleViewModel.FormatInches(ViewModel.SampleImageWidthInches)}in; height: {ScaleViewModel.FormatInches(ViewModel.SampleImageHeightInches)}in;";
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_renderPending)
        {
            return;
        }

        _renderPending = true;
        _ = InvokeAsync(() =>
        {
            _renderPending = false;
            StateHasChanged();
        });
    }

    private bool _renderPending;
}
