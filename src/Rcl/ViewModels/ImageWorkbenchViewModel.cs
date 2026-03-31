using App.Enums;
using App.Services.ColorReduction;
using App.Services.ImageResizing;
using CommunityToolkit.Mvvm.ComponentModel;
using MudBlazor;
using PropertyChanged;
using Rcl.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Rcl.ViewModels.Interfaces;
using App.Extensions.System.Threading;

namespace Rcl.ViewModels;

public sealed class ImageWorkbenchViewModel : ObservableObject, IImageWorkbenchViewModel
{
    private readonly IImageResizingService _imageResizingService;
    private readonly IColorReductionService _colorReductionService;
    private readonly IImageFileService _imageFileService;
    private readonly ISnackbar _snackbar;
    private readonly SemaphoreSlim _processingGate = new(1, 1);

    private byte[]? _sourceImageContent;
    private byte[]? _resultImageContent;
    private string _sourceFileName = string.Empty;

    public IReadOnlyList<ImageSharpKnownResamplers> Resamplers { get; } = Enum.GetValues<ImageSharpKnownResamplers>();
    public IReadOnlyList<ImageSharpKnownDitherings> Ditherings { get; } = Enum.GetValues<ImageSharpKnownDitherings>();
    public IReadOnlyList<BuiltinColorSets> ColorSets { get; } = Enum.GetValues<BuiltinColorSets>();

    public IReadOnlyList<ColorComparisonAlgorithms> ComparisonAlgorithms { get; } =
        Enum.GetValues<ColorComparisonAlgorithms>();

    public bool CanSave => !IsBusy && _resultImageContent is not null;
    public bool HasImage => _sourceImageContent is not null;

    public bool IsBusy { get; set; }
    public string ResultPreviewDataUrl { get; private set; } = string.Empty;
    public int ResultWidth { get; set; }
    public int ResultHeight { get; set; }

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public double Scale { get; set; } = 1;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public byte TransparencyThreshold { get; set; } = 0;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public ImageSharpKnownResamplers SelectedResampler { get; set; } = ImageSharpKnownResamplers.Bicubic;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public ImageSharpKnownDitherings SelectedDithering { get; set; } = ImageSharpKnownDitherings.Sierra3;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public BuiltinColorSets SelectedColorSet { get; set; } = BuiltinColorSets.Dmc;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public ColorComparisonAlgorithms SelectedComparisonAlgorithm { get; set; } = ColorComparisonAlgorithms.Ciede2000;

    public ImageWorkbenchViewModel(
        IImageResizingService imageResizingService,
        IColorReductionService colorReductionService,
        IImageFileService imageFileService,
        ISnackbar snackbar)
    {
        _imageResizingService = imageResizingService;
        _colorReductionService = colorReductionService;
        _imageFileService = imageFileService;
        _snackbar = snackbar;
    }

    private async Task LoadUploadedImageAsync(byte[] content, string fileName)
    {
        ArgumentNullException.ThrowIfNull(content);

        _sourceFileName = fileName;
        _sourceImageContent = content;

        await ProcessPipelineAsync();
    }

    public static string GetDisplayName(Enum value)
    {
        var memberInfo = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var displayAttribute = memberInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>()
            .FirstOrDefault();

        return displayAttribute?.Name ?? value.ToString();
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

            await LoadUploadedImageAsync(picked.Content, picked.FileName);
        }
        catch (Exception ex)
        {
            _snackbar.Add(ex.Message, Severity.Error);
        }
    }

    public async Task SaveResultAsync()
    {
        if (_resultImageContent is null || IsBusy)
        {
            return;
        }

        using var _ = IBusy.UseBusy(this);

        try
        {
            var fileNameStem = string.IsNullOrWhiteSpace(_sourceFileName)
                ? "image"
                : Path.GetFileNameWithoutExtension(_sourceFileName);

            await _imageFileService.SaveImageAsync($"{fileNameStem}-processed.png", _resultImageContent);
        }
        catch (Exception ex)
        {
            _snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private void TriggerRealtimeProcessing()
    {
        if (!HasImage)
        {
            return;
        }

        _ = ProcessPipelineAsync();
    }

    private async Task ProcessPipelineAsync()
    {
        if (_sourceImageContent is null)
        {
            return;
        }

        using var _ = await _processingGate.WaitForDisposableAsync();
        using var _2 = IBusy.UseBusy(this);

        try
        {
            await using var sourceStream = new MemoryStream(_sourceImageContent, writable: false);
            using var source = await Image.LoadAsync<Rgba32>(sourceStream);

            var clampedScale = Math.Clamp(Scale, 0.01, 1.0);
            var targetSize = Math.Max(1, (int)Math.Round(source.Width * clampedScale));

            using var resized = _imageResizingService.ResizeWidth(source, targetSize, SelectedResampler,
                SelectedDithering, TransparencyThreshold);

            using var reduced =
                _colorReductionService.ReduceColors(resized, SelectedColorSet, SelectedComparisonAlgorithm);

            await using var outputStream = new MemoryStream();
            await reduced.SaveAsync(outputStream, new PngEncoder());

            _resultImageContent = outputStream.ToArray();
            ResultPreviewDataUrl = CreateDataUrl(_resultImageContent);
            ResultWidth = reduced.Width;
            ResultHeight = reduced.Height;
        }
        catch (Exception ex)
        {
            _resultImageContent = null;
            ResultPreviewDataUrl = string.Empty;
            ResultWidth = 0;
            ResultHeight = 0;
            _snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private static string CreateDataUrl(byte[] content)
    {
        return $"data:image/png;base64,{Convert.ToBase64String(content)}";
    }
}

public interface IImageWorkbenchViewModel : INotifyPropertyChanged, IBusy
{
    IReadOnlyList<ImageSharpKnownResamplers> Resamplers { get; }
    IReadOnlyList<ImageSharpKnownDitherings> Ditherings { get; }
    IReadOnlyList<BuiltinColorSets> ColorSets { get; }
    IReadOnlyList<ColorComparisonAlgorithms> ComparisonAlgorithms { get; }
    bool CanSave { get; }
    bool HasImage { get; }
    string ResultPreviewDataUrl { get; }
    int ResultWidth { get; set; }
    int ResultHeight { get; set; }
    double Scale { get; set; }
    byte TransparencyThreshold { get; set; }
    ImageSharpKnownResamplers SelectedResampler { get; set; }
    ImageSharpKnownDitherings SelectedDithering { get; set; }
    BuiltinColorSets SelectedColorSet { get; set; }
    ColorComparisonAlgorithms SelectedComparisonAlgorithm { get; set; }
    Task LoadFromDeviceAsync();
    Task SaveResultAsync();
}