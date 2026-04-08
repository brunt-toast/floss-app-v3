using App.Services.ColorMatching;
using App.Services.ColorProfiles;
using App.Services.ColorReduction;
using App.Services.ColorSets;
using App.Services.ImageResizing;
using Database.Ioc;
using Microsoft.Extensions.DependencyInjection;

namespace App.Ioc;

public static class AppServiceRegistrar
{
    public static void RegisterServices(this IServiceCollection services, string appDataDirectory)
    {
        var dbPath = Path.Combine(appDataDirectory, "app.db");
        var connectionString = $"Data Source={dbPath}";

        services.AddSingleton<IColorMatchingService, ColorMatchingService>();
        services.AddSingleton<IImageResizingService, ImageResizingService>();
        services.AddSingleton<IColorSetService, BuiltinColorSetService>();
        services.AddSingleton<IColorReductionService, BuiltinColorReductionService>();
        services.AddScoped<IColorProfileService, ColorProfileService>();

        services.RegisterDatabaseServices(connectionString);
    }
}