using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface ISystemService
{
    Task<SystemStatus> GetSystemStatusAsync();
    Task<List<PowerToyModule>> GetPowerToyModulesAsync();
    Task<bool> TogglePowerToyModuleAsync(string moduleName, bool enabled);
    Task<bool> LaunchPowerToyModuleAsync(string moduleName);
    Task<List<ProcessInfo>> GetRunningProcessesAsync();
    Task<bool> KillProcessAsync(int processId);
}

public class ProcessInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double CpuUsage { get; set; }
    public long MemoryUsage { get; set; }
    public string Status { get; set; } = "";
}
