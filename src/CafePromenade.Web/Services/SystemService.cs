using CafePromenade.Core.Models;
using CafePromenade.Core.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CafePromenade.Web.Services;

public class SystemService : ISystemService
{
    public Task<SystemStatus> GetSystemStatusAsync()
    {
        return Task.FromResult(new SystemStatus
        {
            MachineName = Environment.MachineName,
            OsVersion = RuntimeInformation.OSDescription,
            ProcessCount = Process.GetProcesses().Length,
            Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64)
        });
    }

    public Task<List<PowerToyModule>> GetPowerToyModulesAsync()
    {
        return Task.FromResult(new List<PowerToyModule>
        {
            new() { Name = "Always on Top", Description = "Pin windows on top", IsEnabled = true, Shortcut = "Win+Ctrl+T", Category = "Windowing" },
            new() { Name = "Awake", Description = "Keep screen on", IsEnabled = true, Category = "Power" },
            new() { Name = "Color Picker", Description = "Screen color picker", IsEnabled = true, Shortcut = "Win+Shift+C", Category = "Tools" },
            new() { Name = "Crop and Lock", Description = "Crop windows", IsEnabled = true, Category = "Windowing" },
            new() { Name = "Environment Variables", Description = "Manage env vars", IsEnabled = true, Category = "System" },
            new() { Name = "FancyZones", Description = "Window layout manager", IsEnabled = true, Shortcut = "Win+Shift+`", Category = "Windowing" },
            new() { Name = "File Explorer", Description = "Preview pane addons", IsEnabled = true, Category = "Files" },
            new() { Name = "File Locksmith", Description = "Find locked files", IsEnabled = true, Category = "Files" },
            new() { Name = "Hosts File Editor", Description = "Edit hosts file", IsEnabled = true, Category = "Networking" },
            new() { Name = "Image Resizer", Description = "Bulk image resize", IsEnabled = true, Category = "Tools" },
            new() { Name = "Keyboard Manager", Description = "Remap keys", IsEnabled = true, Shortcut = "Win+Ctrl+K", Category = "Input" },
            new() { Name = "Mouse Utilities", Description = "Find my mouse", IsEnabled = true, Shortcut = "Left Ctrl x2", Category = "Input" },
            new() { Name = "Mouse Without Borders", Description = "Multi-PC mouse", IsEnabled = true, Category = "Input" },
            new() { Name = "Paste as Plain Text", Description = "Strip formatting", IsEnabled = true, Shortcut = "Win+Ctrl+V", Category = "Tools" },
            new() { Name = "Peek", Description = "Quick file preview", IsEnabled = true, Shortcut = "Ctrl+Space", Category = "Files" },
            new() { Name = "PowerRename", Description = "Bulk rename", IsEnabled = true, Category = "Files" },
            new() { Name = "PowerToys Run", Description = "Quick launcher", IsEnabled = true, Shortcut = "Alt+Space", Category = "Launcher" },
            new() { Name = "Quick Accent", Description = "Accent characters", IsEnabled = true, Category = "Input" },
            new() { Name = "Registry Preview", Description = "Preview .reg files", IsEnabled = true, Category = "System" },
            new() { Name = "Screen Ruler", Description = "Measure pixels", IsEnabled = true, Shortcut = "Win+Shift+M", Category = "Tools" },
            new() { Name = "Shortcut Guide", Description = "Windows key shortcuts", IsEnabled = true, Shortcut = "Win+Shift+/", Category = "Tools" },
            new() { Name = "Text Extractor", Description = "OCR screen text", IsEnabled = true, Shortcut = "Win+Shift+T", Category = "Tools" },
            new() { Name = "Video Conference", Description = "Mute camera/mic", IsEnabled = true, Shortcut = "Win+Shift+Q", Category = "Tools" },
            new() { Name = "Command Palette", Description = "Command palette", IsEnabled = true, Category = "Launcher" }
        });
    }

    public Task<bool> TogglePowerToyModuleAsync(string moduleName, bool enabled) => Task.FromResult(true);
    public Task<bool> LaunchPowerToyModuleAsync(string moduleName) => Task.FromResult(true);
    public Task<List<ProcessInfo>> GetRunningProcessesAsync() => Task.FromResult(new List<ProcessInfo>());
    public Task<bool> KillProcessAsync(int processId)
    {
        try { Process.GetProcessById(processId).Kill(); return Task.FromResult(true); }
        catch { return Task.FromResult(false); }
    }
}
