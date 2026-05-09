using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CafePromenade.Desktop;

public partial class App : Application
{
    [DllImport("Microsoft.WindowsAppRuntime.Bootstrap.dll", CharSet = CharSet.Unicode)]
    private static extern int MddBootstrapInitialize(uint majorMinorVersion, [MarshalAs(UnmanagedType.LPWStr)] string versionTag, ulong packageVersion);

    [DllImport("Microsoft.WindowsAppRuntime.Bootstrap.dll")]
    private static extern void MddBootstrapShutdown();

    private Window? _window;
    private static bool _bootstrapInitialized;

    public App()
    {
        InitializeBootstrap();
        this.InitializeComponent();
    }

    private static void InitializeBootstrap()
    {
        if (_bootstrapInitialized) return;

        try
        {
            // Try Windows App SDK 1.8
            int hr = MddBootstrapInitialize(0x00010008, "1.8.260416003", 0);
            if (hr >= 0) { _bootstrapInitialized = true; return; }

            // Try 1.7
            hr = MddBootstrapInitialize(0x00010007, "1.7.260224002", 0);
            if (hr >= 0) { _bootstrapInitialized = true; return; }

            // Try 1.6
            hr = MddBootstrapInitialize(0x00010006, "1.6.250602001", 0);
            if (hr >= 0) { _bootstrapInitialized = true; return; }

            Debug.WriteLine($"Bootstrap init failed for all versions");
        }
        catch (DllNotFoundException ex)
        {
            Debug.WriteLine($"Bootstrap DLL not found: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Bootstrap init error: {ex.Message}");
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            _window = new MainWindow();
            _window.Activate();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"OnLaunched failed: {ex}");
            throw;
        }
    }

    public static Window? MainWindow => ((App)Current)._window;
}
