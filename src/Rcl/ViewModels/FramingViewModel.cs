using CommunityToolkit.Mvvm.ComponentModel;
using MudBlazor;
using Rcl.Services;
using Rcl.Services.Framing;
using Rcl.ViewModels.Interfaces;
using System.ComponentModel;

namespace Rcl.ViewModels;

public sealed class FramingViewModel : ObservableObject, IFramingViewModel
{
    public const double MinimumScale = 0.7071067811865475;
    public const double MaximumScale = 1.0;

    private readonly IImageFileService _imageFileService;
    private readonly ISnackbar _snackbar;
    private byte[]? _sourceImageContent;
    private byte[]? _framedImageContent;
    private string _sourceFileName = string.Empty;
    private double _scale = MaximumScale;

    public FramingViewModel(IImageFileService imageFileService, ISnackbar snackbar)
    {
        _imageFileService = imageFileService;
        _snackbar = snackbar;
    }

    public bool IsBusy
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public bool HasImage => _sourceImageContent is not null;
    public bool CanSave => !IsBusy && _framedImageContent is not null;

    public string PreviewDataUrl
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public double Scale
    {
        get => _scale;
        set
        {
            double clamped = Math.Clamp(value, MinimumScale, MaximumScale);
            if (SetProperty(ref _scale, clamped) && HasImage)
            {
                _ = RenderPreviewAsync();
            }
        }
    }

    public async Task LoadFromDeviceAsync()
    {
        if (IsBusy)
        {
            return;
        }

        using var _ = IBusy.UseBusy(this);
        try
        {
            var picked = await _imageFileService.PickImageAsync();
            if (picked is null)
            {
                return;
            }

            _sourceFileName = picked.FileName;
            _sourceImageContent = picked.Content;
            _scale = MaximumScale;
            OnPropertyChanged(nameof(Scale));
            OnPropertyChanged(nameof(HasImage));

            await RenderPreviewAsync();
        }
        catch (Exception ex)
        {
            _snackbar.Add(ex.Message, Severity.Error);
        }
    }

    public async Task SaveResultAsync()
    {
        if (_framedImageContent is null || IsBusy)
        {
            return;
        }

        using var _ = IBusy.UseBusy(this);
        try
        {
            string fileNameStem = string.IsNullOrWhiteSpace(_sourceFileName)
                ? "image"
                : Path.GetFileNameWithoutExtension(_sourceFileName);

            await _imageFileService.SaveImageAsync($"{fileNameStem}-framed.png", _framedImageContent);
        }
        catch (Exception ex)
        {
            _snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task RenderPreviewAsync()
    {
        if (_sourceImageContent is null)
        {
            return;
        }

        using var _ = IBusy.UseBusy(this);
        try
        {
            _framedImageContent = await FramingImageRenderer.RenderAsync(_sourceImageContent, Scale);
            PreviewDataUrl = CreateDataUrl(_framedImageContent);

            OnPropertyChanged(nameof(CanSave));
        }
        catch (Exception ex)
        {
            _framedImageContent = null;
            PreviewDataUrl = string.Empty;
            _snackbar.Add(ex.Message, Severity.Error);
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private static string CreateDataUrl(byte[] content)
    {
        return $"data:image/png;base64,{Convert.ToBase64String(content)}";
    }
}

public interface IFramingViewModel : INotifyPropertyChanged, IBusy
{
    bool HasImage { get; }
    bool CanSave { get; }
    string PreviewDataUrl { get; }
    double Scale { get; set; }
    Task LoadFromDeviceAsync();
    Task SaveResultAsync();
}
