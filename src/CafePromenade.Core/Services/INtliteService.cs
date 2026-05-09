using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface INtliteService
{
    Task<WindowsImageInfo?> LoadImageAsync(string imagePath);
    Task<bool> ApplyPresetAsync(string imagePath, NtlitePreset preset);
    Task<bool> RemoveComponentsAsync(string imagePath, List<string> components);
    Task<bool> IntegrateUpdatesAsync(string imagePath, List<string> updatePaths);
    Task<bool> IntegrateDriversAsync(string imagePath, List<string> driverPaths);
    Task<bool> SetFeaturesAsync(string imagePath, Dictionary<string, bool> features);
    Task<bool> ApplyTweaksAsync(string imagePath, Dictionary<string, string> tweaks);
    Task<bool> SetUnattendedAsync(string imagePath, UnattendedSettings settings);
    Task<bool> CreateIsoAsync(string outputPath, string sourcePath);
    Task<NtlitePreset> LoadPresetAsync(string presetPath);
    Task<bool> SavePresetAsync(NtlitePreset preset, string path);
    Task<bool> IsNtliteInstalledAsync();
    Task<string> GetNtlitePathAsync();
}
