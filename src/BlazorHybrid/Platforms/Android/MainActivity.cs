using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace BlazorHybrid;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Keep app content below system bars (status/navigation) on Android.
        WindowCompat.SetDecorFitsSystemWindows(Window, true);
    }
}
