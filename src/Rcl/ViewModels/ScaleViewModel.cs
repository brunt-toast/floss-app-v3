using CommunityToolkit.Mvvm.ComponentModel;
using MudBlazor;
using Rcl.Services;
using Rcl.ViewModels.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.ComponentModel;
using System.Globalization;

namespace Rcl.ViewModels;

public sealed class ScaleViewModel : ObservableObject, IScaleViewModel
{
    private readonly IImageFileService _imageFileService;
    private readonly ISnackbar _snackbar;

    public ScaleViewModel(IImageFileService imageFileService, ISnackbar snackbar)
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
                OnPropertyChanged(nameof(CanLoad));
            }
        }
    }

    public bool HasImage => SourceWidth > 0 && SourceHeight > 0;
    public bool CanLoad => !IsBusy;
    public int MinimumHoopSizeInches => CalculateMinimumHoopSizeInches(SourceWidth, SourceHeight, StitchCount);
    public double SampleImageWidthInches => CalculateImageDimensionInches(SourceWidth, StitchCount);
    public double SampleImageHeightInches => CalculateImageDimensionInches(SourceHeight, StitchCount);

    public string PreviewDataUrl
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public int SourceWidth
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                NotifyCalculatedPropertiesChanged();
            }
        }
    }

    public int SourceHeight
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                NotifyCalculatedPropertiesChanged();
            }
        }
    }

    public int StitchCount
    {
        get;
        set
        {
            var clamped = Math.Max(1, value);
            if (SetProperty(ref field, clamped))
            {
                NotifyCalculatedPropertiesChanged();
            }
        }
    } = 14;

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

            await using var sourceStream = new MemoryStream(picked.Content, writable: false);
            using var image = await Image.LoadAsync<Rgba32>(sourceStream);

            SourceWidth = image.Width;
            SourceHeight = image.Height;
            PreviewDataUrl = CreateDataUrl(picked.FileName, picked.Content);
            OnPropertyChanged(nameof(HasImage));
        }
        catch (Exception ex)
        {
            SourceWidth = 0;
            SourceHeight = 0;
            PreviewDataUrl = string.Empty;
            _snackbar.Add(ex.Message, Severity.Error);
            OnPropertyChanged(nameof(HasImage));
        }
    }

    internal static int CalculateMinimumHoopSizeInches(int width, int height, int stitchCount)
    {
        if (width <= 0 || height <= 0 || stitchCount <= 0)
        {
            return 0;
        }

        return (int)Math.Ceiling(Math.Max(width, height) / (double)stitchCount);
    }

    internal static double CalculateImageDimensionInches(int pixels, int stitchCount)
    {
        if (pixels <= 0 || stitchCount <= 0)
        {
            return 0;
        }

        return pixels / (double)stitchCount;
    }

    internal static string FormatInches(double inches)
    {
        return inches.ToString("0.###", CultureInfo.InvariantCulture);
    }

    private void NotifyCalculatedPropertiesChanged()
    {
        OnPropertyChanged(nameof(MinimumHoopSizeInches));
        OnPropertyChanged(nameof(SampleImageWidthInches));
        OnPropertyChanged(nameof(SampleImageHeightInches));
    }

    private static string CreateDataUrl(string fileName, byte[] content)
    {
        return $"data:{GetMimeType(fileName)};base64,{Convert.ToBase64String(content)}";
    }

    private static string GetMimeType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".png" => "image/png",
            _ => "image/png"
        };
    }
}

public interface IScaleViewModel : INotifyPropertyChanged, IBusy
{
    bool HasImage { get; }
    bool CanLoad { get; }
    int SourceWidth { get; }
    int SourceHeight { get; }
    int StitchCount { get; set; }
    int MinimumHoopSizeInches { get; }
    double SampleImageWidthInches { get; }
    double SampleImageHeightInches { get; }
    string PreviewDataUrl { get; }
    Task LoadFromDeviceAsync();
}
