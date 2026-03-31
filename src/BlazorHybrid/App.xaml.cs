using System.Runtime.Versioning;

namespace BlazorHybrid;

public partial class App : Application
{
	private const double PhoneWindowWidth = 450;
	private const double PhoneWindowAspectRatio = 18d / 9d;

	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
#if WINDOWS
		return CreateWindowsWindow();
#else
		return new Window(new MainPage());
#endif
	}

#if WINDOWS
	[SupportedOSPlatform("windows")]
	private static Window CreateWindowsWindow()
	{
		var window = new Window(new MainPage()) { Title = "BlazorHybrid" };
		window.Width = PhoneWindowWidth;
		window.Height = PhoneWindowWidth * PhoneWindowAspectRatio;
		return window;
	}
#endif
}
