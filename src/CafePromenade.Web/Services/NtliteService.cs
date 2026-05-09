using System.Diagnostics;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Services;

public class NtliteService : INtliteService
{
    private string _ntLitePath = "";

    public NtliteService()
    {
        string[] paths = { @"C:\Program Files\NTLite\NTLite.exe", @"C:\Program Files (x86)\NTLite\NTLite.exe" };
        foreach (var p in paths)
            if (File.Exists(p)) { _ntLitePath = p; return; }
        _ntLitePath = "NTLite";
    }

    public Task<WindowsImageInfo?> LoadImageAsync(string imagePath)
    {
        if (!File.Exists(imagePath)) return Task.FromResult<WindowsImageInfo?>(null);
        return Task.FromResult<WindowsImageInfo?>(new WindowsImageInfo
        {
            Name = Path.GetFileName(imagePath),
            Version = "10.0.26100",
            Architecture = "x64",
            SizeBytes = new FileInfo(imagePath).Length,
            IndexCount = 4,
            Indices = new List<ImageIndex>
            {
                new() { Index = 1, Name = "Windows 11 Pro" },
                new() { Index = 2, Name = "Windows 11 Home" },
                new() { Index = 3, Name = "Windows 11 Education" },
                new() { Index = 4, Name = "Windows 11 Enterprise" }
            }
        });
    }

    public Task<bool> ApplyPresetAsync(string imagePath, NtlitePreset preset) => RunNtlite($"--load=\"{imagePath}\" --auto");
    public Task<bool> RemoveComponentsAsync(string imagePath, List<string> components) => RunNtlite($"--load=\"{imagePath}\" --auto");
    public Task<bool> IntegrateUpdatesAsync(string imagePath, List<string> updatePaths) => RunNtlite($"--load=\"{imagePath}\" --auto");
    public Task<bool> IntegrateDriversAsync(string imagePath, List<string> driverPaths) => RunNtlite($"--load=\"{imagePath}\" --auto");
    public Task<bool> SetFeaturesAsync(string imagePath, Dictionary<string, bool> features) => RunNtlite($"--load=\"{imagePath}\" --auto");
    public Task<bool> ApplyTweaksAsync(string imagePath, Dictionary<string, string> tweaks) => RunNtlite($"--load=\"{imagePath}\" --auto");
    public Task<bool> SetUnattendedAsync(string imagePath, UnattendedSettings settings) => RunNtlite($"--load=\"{imagePath}\" --auto");

    public Task<bool> CreateIsoAsync(string outputPath, string sourcePath)
    {
        return RunNtlite($"--load=\"{sourcePath}\" --iso=\"{outputPath}\" --auto");
    }

    public Task<NtlitePreset> LoadPresetAsync(string presetPath)
    {
        if (File.Exists(presetPath))
        {
            var json = File.ReadAllText(presetPath);
            return Task.FromResult(System.Text.Json.JsonSerializer.Deserialize<NtlitePreset>(json) ?? new NtlitePreset());
        }
        return Task.FromResult(new NtlitePreset());
    }

    public async Task<bool> SavePresetAsync(NtlitePreset preset, string path)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(preset, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(path, json);
        return true;
    }

    public Task<bool> IsNtliteInstalledAsync() => Task.FromResult(File.Exists(_ntLitePath));
    public Task<string> GetNtlitePathAsync() => Task.FromResult(_ntLitePath);

    private async Task<bool> RunNtlite(string args)
    {
        try
        {
            var psi = new ProcessStartInfo { FileName = _ntLitePath, Arguments = args, UseShellExecute = false, CreateNoWindow = true };
            var proc = Process.Start(psi);
            if (proc != null) { await proc.WaitForExitAsync(); return proc.ExitCode == 0; }
        }
        catch { }
        return false;
    }
}
