using App.Enums;
using App.Services.ColorProfiles;
using App.Services.ColorReduction;
using App.Services.ImageResizing;
using App.Types;
using CommunityToolkit.Mvvm.ComponentModel;
using MudBlazor;
using PropertyChanged;
using Rcl.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.ComponentModel;
using Rcl.ViewModels.Interfaces;
using App.Extensions.System.Threading;
using DrawingColor = System.Drawing.Color;

namespace Rcl.ViewModels;

public sealed record ImageWorkbenchColorUsageItem(
    string Name,
    string Floss,
    string Hex,
    int PixelCount);

public sealed class ImageWorkbenchViewModel : ObservableObject, IImageWorkbenchViewModel
{
    private readonly IImageResizingService _imageResizingService;
    private readonly IColorReductionService _colorReductionService;
    private readonly IColorProfileService _colorProfileService;
    private readonly IImageFileService _imageFileService;
    private readonly ISnackbar _snackbar;
    private readonly SemaphoreSlim _processingGate = new(1, 1);

    private byte[]? _sourceImageContent;
    private byte[]? _resultImageContent;
    private string _sourceFileName = string.Empty;
    private bool _suppressRealtimeProcessing;
    private CancellationTokenSource? _pipelineCts;

    public IReadOnlyList<ImageSharpKnownResamplers> Resamplers { get; } = Enum.GetValues<ImageSharpKnownResamplers>();
    public IReadOnlyList<ImageSharpKnownDitherings> Ditherings { get; } = Enum.GetValues<ImageSharpKnownDitherings>();
    public IReadOnlyList<ColorSetProfileOption> ColorSets { get; private set; } = [];

    public IReadOnlyList<ColorComparisonAlgorithms> ComparisonAlgorithms { get; } =
        Enum.GetValues<ColorComparisonAlgorithms>();

    public bool CanSave => !IsBusy && _resultImageContent is not null;
    public bool HasImage => _sourceImageContent is not null;

    public int SourceWidth { get; private set; }
    public int SourceHeight { get; private set; }
    public bool IsBusy { get; set; }
    public string ResultPreviewDataUrl { get; private set; } = string.Empty;
    public IReadOnlyList<ImageWorkbenchColorUsageItem> ColorUsage { get; private set; } = [];
    public int MaximumColorFidelity { get; private set; } = 1;
    public double StitchLength { get; set; } = 1;
    public double SpoolLength { get; set; } = 1;

    [OnChangedMethod(nameof(OnDesiredWidthChanged))]
    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public int DesiredWidth { get; set; }

    [OnChangedMethod(nameof(OnDesiredHeightChanged))]
    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public int DesiredHeight { get; set; }

    [OnChangedMethod(nameof(OnScaleChanged))]
    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public double Scale { get; set; } = 1;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public byte TransparencyThreshold { get; set; } = 0;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public ImageSharpKnownResamplers SelectedResampler { get; set; } = ImageSharpKnownResamplers.Bicubic;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public ImageSharpKnownDitherings SelectedDithering { get; set; } = ImageSharpKnownDitherings.Sierra3;

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public string SelectedColorSetKey { get; set; } = "builtin:Dmc";

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public int SelectedColorFidelity { get; set; }

    [OnChangedMethod(nameof(TriggerRealtimeProcessing))]
    public ColorComparisonAlgorithms SelectedComparisonAlgorithm { get; set; } = ColorComparisonAlgorithms.Ciede2000;

    public ImageWorkbenchViewModel(
        IImageResizingService imageResizingService,
        IColorReductionService colorReductionService,
        IColorProfileService colorProfileService,
        IImageFileService imageFileService,
        ISnackbar snackbar)
    {
        _imageResizingService = imageResizingService;
        _colorReductionService = colorReductionService;
        _colorProfileService = colorProfileService;
        _imageFileService = imageFileService;
        _snackbar = snackbar;
    }

    public async Task InitializeAsync()
    {
        ColorSets = await _colorProfileService.GetVisibleProfilesAsync();

        if (ColorSets.Count == 0)
        {
            SelectedColorSetKey = string.Empty;
            return;
        }

        if (ColorSets.All(set => !string.Equals(set.Key, SelectedColorSetKey, StringComparison.Ordinal)))
        {
            SelectedColorSetKey = ColorSets[0].Key;
        }
    }

    private async Task LoadUploadedImageAsync(byte[] content, string fileName)
    {
        ArgumentNullException.ThrowIfNull(content);

        var cts = new CancellationTokenSource();
        var previous = Interlocked.Exchange(ref _pipelineCts, cts);
        previous?.Cancel();
        previous?.Dispose();

        _sourceFileName = fileName;
        _sourceImageContent = content;

        await ProcessPipelineAsync(cts.Token);
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
        if (!HasImage || _suppressRealtimeProcessing)
            return;

        var cts = new CancellationTokenSource();
        var previous = Interlocked.Exchange(ref _pipelineCts, cts);
        previous?.Cancel();
        previous?.Dispose();

        _ = TriggerDebouncedAsync(cts.Token);
    }

    private async Task TriggerDebouncedAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(300, cancellationToken);
            await ProcessPipelineAsync(cancellationToken);
        }
        catch (OperationCanceledException) { }
    }

    private async Task ProcessPipelineAsync(CancellationToken cancellationToken = default)
    {
        if (_sourceImageContent is null)
        {
            return;
        }

        using var _ = await _processingGate.WaitForDisposableAsync(cancellationToken);
        using var _2 = IBusy.UseBusy(this);

        try
        {
            await using var sourceStream = new MemoryStream(_sourceImageContent, writable: false);
            using var source = await Image.LoadAsync<Rgba32>(sourceStream, cancellationToken);

            SourceWidth = source.Width;
            SourceHeight = source.Height;
            SynchronizeResizeInputs(Scale);

            var targetSize = Math.Max(1, (int)Math.Round(source.Width * Scale));
            var previousMaximumColorFidelity = MaximumColorFidelity;
            int? requestedColorFidelity = SelectedColorFidelity > 0 ? SelectedColorFidelity : null;

            using var resized = _imageResizingService.ResizeWidth(source, targetSize, SelectedResampler,
                SelectedDithering, TransparencyThreshold);

            var selectedColors = await _colorProfileService.GetColorsAsync(SelectedColorSetKey);

            cancellationToken.ThrowIfCancellationRequested();

            using var reductionResult =
                _colorReductionService.ReduceColors(
                    resized,
                    selectedColors,
                    SelectedComparisonAlgorithm,
                    requestedColorFidelity);

            var availableColorFidelity = reductionResult.AvailableColorCount;
            var shouldTrackMaximumColorFidelity = SelectedColorFidelity <= 0 ||
                                                  (previousMaximumColorFidelity > 0 && SelectedColorFidelity ==
                                                      previousMaximumColorFidelity);

            var effectiveColorFidelity = shouldTrackMaximumColorFidelity
                ? availableColorFidelity
                : Math.Clamp(SelectedColorFidelity, 1, availableColorFidelity);

            MaximumColorFidelity = availableColorFidelity;
            SetSelectedColorFidelitySilently(effectiveColorFidelity);

            ColorReductionResult? adjustedReductionResult = null;
            try
            {
                if (requestedColorFidelity.HasValue && requestedColorFidelity.Value < effectiveColorFidelity)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    adjustedReductionResult = _colorReductionService.ReduceColors(
                        resized,
                        selectedColors,
                        SelectedComparisonAlgorithm,
                        effectiveColorFidelity);
                }

                var reduced = adjustedReductionResult?.Image ?? reductionResult.Image;

                ColorUsage = BuildColorUsage(reduced, selectedColors);

                cancellationToken.ThrowIfCancellationRequested();

                await using var outputStream = new MemoryStream();
                await reduced.SaveAsync(outputStream, new PngEncoder(), cancellationToken);

                _resultImageContent = outputStream.ToArray();
                ResultPreviewDataUrl = CreateDataUrl(_resultImageContent);
            }
            finally
            {
                adjustedReductionResult?.Dispose();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            _resultImageContent = null;
            ResultPreviewDataUrl = string.Empty;
            ColorUsage = [];
            MaximumColorFidelity = 1;
            _snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private void SetSelectedColorFidelitySilently(int value)
    {
        _suppressRealtimeProcessing = true;
        try
        {
            SelectedColorFidelity = value;
        }
        finally
        {
            _suppressRealtimeProcessing = false;
        }
    }

    private static IReadOnlyList<ImageWorkbenchColorUsageItem> BuildColorUsage(
        Image<Rgba32> reduced,
        IReadOnlyCollection<SetColor> colors)
    {
        var paletteByRgb = colors
            .GroupBy(color => ToRgbKey(color.Color))
            .ToDictionary(group => group.Key, group => group.First());

        var pixelCountsByRgb = new Dictionary<int, int>();

        reduced.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    var pixel = row[x];
                    if (pixel.A == 0)
                    {
                        continue;
                    }

                    var rgbKey = ToRgbKey(pixel.R, pixel.G, pixel.B);
                    pixelCountsByRgb[rgbKey] = pixelCountsByRgb.GetValueOrDefault(rgbKey) + 1;
                }
            }
        });

        return pixelCountsByRgb
            .OrderByDescending(entry => entry.Value)
            .ThenBy(entry => paletteByRgb.TryGetValue(entry.Key, out var setColor)
                ? setColor.Name
                : entry.Key.ToString("X6"))
            .Select(entry =>
            {
                if (paletteByRgb.TryGetValue(entry.Key, out var setColor))
                {
                    return new ImageWorkbenchColorUsageItem(
                        string.IsNullOrWhiteSpace(setColor.Name) ? setColor.Floss : setColor.Name,
                        setColor.Floss,
                        $"#{entry.Key:X6}",
                        entry.Value);
                }

                return new ImageWorkbenchColorUsageItem(
                    $"#{entry.Key:X6}",
                    string.Empty,
                    $"#{entry.Key:X6}",
                    entry.Value);
            })
            .ToArray();
    }

    private static int ToRgbKey(DrawingColor color)
    {
        return ToRgbKey(color.R, color.G, color.B);
    }

    private static int ToRgbKey(byte red, byte green, byte blue)
    {
        return (red << 16) | (green << 8) | blue;
    }

    internal static int CalculateSpoolsForColor(int pixelCount, double stitchLength, double spoolLength)
    {
        return Math.Max(1, (int)Math.Ceiling(pixelCount * stitchLength / spoolLength));
    }

    private static string CreateDataUrl(byte[] content)
    {
        return $"data:image/png;base64,{Convert.ToBase64String(content)}";
    }

    private void OnDesiredWidthChanged()
    {
        SynchronizeResizeInputs((double)DesiredWidth / SourceWidth);
    }

    private void OnDesiredHeightChanged()
    {
        SynchronizeResizeInputs((double)DesiredHeight / SourceHeight);
    }

    private void OnScaleChanged()
    {
        SynchronizeResizeInputs(Scale);
    }

    private readonly LockedFunc _widthSyncContext = new();
    private void SynchronizeResizeInputs(double scale)
    {
        var synchronizedWidth = Math.Max(1, (int)Math.Round(SourceWidth * scale));
        var synchronizedHeight = Math.Max(1, (int)Math.Round(SourceHeight * scale));

        _widthSyncContext.Invoke(() =>
        {
            Scale = scale;
            DesiredWidth = synchronizedWidth;
            DesiredHeight = synchronizedHeight;
        });
    }
}

class LockedFunc
{
    private bool _active;

    public void Invoke(Action action)
    {
        if (_active)
        {
            return;
        }

        _active = true;
        try
        {
            action.Invoke();
        }
        finally
        {
            _active = false;
        }
    }
}

public interface IImageWorkbenchViewModel : INotifyPropertyChanged, IBusy
{
    IReadOnlyList<ImageSharpKnownResamplers> Resamplers { get; }
    IReadOnlyList<ImageSharpKnownDitherings> Ditherings { get; }
    IReadOnlyList<ColorSetProfileOption> ColorSets { get; }
    IReadOnlyList<ColorComparisonAlgorithms> ComparisonAlgorithms { get; }
    bool CanSave { get; }
    bool HasImage { get; }
    IReadOnlyList<ImageWorkbenchColorUsageItem> ColorUsage { get; }
    int MaximumColorFidelity { get; }
    string ResultPreviewDataUrl { get; }
    int SourceWidth { get; }
    int SourceHeight { get; }
    int DesiredWidth { get; set; }
    int DesiredHeight { get; set; }
    double StitchLength { get; set; }
    double SpoolLength { get; set; }
    double Scale { get; set; }
    byte TransparencyThreshold { get; set; }
    ImageSharpKnownResamplers SelectedResampler { get; set; }
    ImageSharpKnownDitherings SelectedDithering { get; set; }
    string SelectedColorSetKey { get; set; }
    int SelectedColorFidelity { get; set; }
    ColorComparisonAlgorithms SelectedComparisonAlgorithm { get; set; }
    Task InitializeAsync();
    Task LoadFromDeviceAsync();
    Task SaveResultAsync();
}
