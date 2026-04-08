using Microsoft.Extensions.DependencyInjection;
using Rcl.Services;
using Rcl.Ioc;
using BlazorHybrid.Services;

namespace BlazorHybrid.Ioc;

public static class BlazorHybridServiceRegistrar
{
    public static void RegisterServices(this IServiceCollection services)
    {
        RclServiceRegistrar.RegisterServices(services, FileSystem.AppDataDirectory);
        services.AddSingleton<IImageFileService, MauiImageFileService>();
    }
}