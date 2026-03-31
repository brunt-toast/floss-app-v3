using App.Services.ColorMatching;
using App.Services.ColorReduction;
using App.Services.ColorSets;
using App.Services.ImageResizing;
using Microsoft.Extensions.DependencyInjection;

namespace App.Ioc;

public static class AppServiceRegistrar
{
    public static void RegisterServices(this IServiceCollection sc)
    {
        sc.AddSingleton<IColorMatchingService, ColorMatchingService>();
        sc.AddSingleton<IImageResizingService, ImageResizingService>();
        sc.AddSingleton<IColorSetService, BuiltinColorSetService>();
        sc.AddSingleton<IColorReductionService, BuiltinColorReductionService>();
    }
}