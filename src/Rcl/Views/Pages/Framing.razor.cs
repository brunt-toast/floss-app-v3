using Microsoft.AspNetCore.Components;
using Rcl.ViewModels;
using System.ComponentModel;

namespace Rcl.Views.Pages;

public partial class Framing : IDisposable
{
    [Inject] public IFramingViewModel ViewModel { get; set; } = default!;

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

    private Task OnScaleChanged(double value)
    {
        ViewModel.Scale = value;
        return Task.CompletedTask;
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
