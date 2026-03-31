using App.Ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rcl.Services;
using Rcl.ViewModels;

namespace Rcl.Ioc;

public static class RclServiceRegistrar
{
    public static void RegisterServices(this IServiceCollection sc)
    {
        AppServiceRegistrar.RegisterServices(sc);
        sc.AddTransient<IImageWorkbenchViewModel, ImageWorkbenchViewModel>();
        sc.TryAddSingleton<IImageFileService, UnsupportedImageFileService>();
    }
}