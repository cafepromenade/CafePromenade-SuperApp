namespace CafePromenade.Core.Models;

public class PowerToyModule
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsEnabled { get; set; }
    public string Shortcut { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Category { get; set; } = "";
    public Dictionary<string, string> Settings { get; set; } = new();
}

public class SystemStatus
{
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public long TotalMemory { get; set; }
    public long UsedMemory { get; set; }
    public long TotalDisk { get; set; }
    public long UsedDisk { get; set; }
    public int ProcessCount { get; set; }
    public TimeSpan Uptime { get; set; }
    public string OsVersion { get; set; } = "";
    public string MachineName { get; set; } = "";
}
