using App.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rcl.Services;
using Rcl.ViewModels;

namespace Rcl.Ioc;

public static class RclServiceRegistrar
{
    public static void RegisterServices(this IServiceCollection services, string appDataDirectory)
    {
        AppServiceRegistrar.RegisterServices(services, appDataDirectory);
        services.AddTransient<IImageWorkbenchViewModel, ImageWorkbenchViewModel>();
        services.AddTransient<IFramingViewModel, FramingViewModel>();
        services.AddTransient<IScaleViewModel, ScaleViewModel>();
        services.TryAddSingleton<IImageFileService, UnsupportedImageFileService>();
        services.TryAddSingleton<IProfileFileService, UnsupportedProfileFileService>();
    }
}
