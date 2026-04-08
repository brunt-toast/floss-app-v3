using BlazorHybrid.Ioc;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using System.Runtime.Versioning;

namespace BlazorHybrid;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
#if ANDROID
		return CreateAndroidApp();
#elif WINDOWS
		return CreateWindowsApp();
#else
		throw new PlatformNotSupportedException("Unsupported platform for BlazorHybrid.");
#endif
	}

#if ANDROID
	[SupportedOSPlatform("android24.0")]
	private static MauiApp CreateAndroidApp()
	{
		return CreateConfiguredApp(false);
	}
#endif

#if WINDOWS
	[SupportedOSPlatform("windows10.0.17763")]
	private static MauiApp CreateWindowsApp()
	{
		return CreateConfiguredApp(true);
	}
#endif

	private static MauiApp CreateConfiguredApp(bool addWebViewDevTools)
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

		builder.Services.AddMauiBlazorWebView();
		BlazorHybridServiceRegistrar.RegisterServices(builder.Services);
		builder.Services.AddMudServices();

#if DEBUG
		if (addWebViewDevTools)
		{
			builder.Services.AddBlazorWebViewDeveloperTools();
		}

		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
