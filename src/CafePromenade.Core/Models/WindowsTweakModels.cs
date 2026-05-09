namespace CafePromenade.Core.Models;

public class WindowsTweak
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string RegistryPath { get; set; } = "";
    public string RegistryKey { get; set; } = "";
    public string RegistryValue { get; set; } = "";
    public TweakValueType ValueType { get; set; } = TweakValueType.DWord;
    public object? DefaultValue { get; set; }
    public object? TweakedValue { get; set; }
    public bool IsApplied { get; set; }
    public bool RequiresRestart { get; set; }
    public bool RequiresExplorerRestart { get; set; }
    public TweakRisk Risk { get; set; } = TweakRisk.Low;
    public string Icon { get; set; } = "";
}

public enum TweakValueType
{
    DWord,
    String,
    Binary,
    QWord
}

public enum TweakRisk
{
    Low,
    Medium,
    High
}

public class TweakCategory
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "";
    public List<WindowsTweak> Tweaks { get; set; } = new();
}

public class WindowsServiceInfo
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Status { get; set; } = "";
    public string StartType { get; set; } = "";
    public bool CanDisable { get; set; }
    public string Description { get; set; } = "";
}

public class ScheduledTaskInfo
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string Status { get; set; } = "";
    public string LastRun { get; set; } = "";
    public string NextRun { get; set; } = "";
    public bool IsEnabled { get; set; }
}

public class StartupItem
{
    public string Name { get; set; } = "";
    public string Command { get; set; } = "";
    public string Location { get; set; } = "";
    public bool IsEnabled { get; set; }
    public string Publisher { get; set; } = "";
}

public class ContextMenuEntry
{
    public string Name { get; set; } = "";
    public string Command { get; set; } = "";
    public string FileType { get; set; } = "";
    public string MenuType { get; set; } = "";
    public bool IsEnabled { get; set; }
    public string RegistryPath { get; set; } = "";
}

public class WindowsUpdateInfo
{
    public string Title { get; set; } = "";
    public string KB { get; set; } = "";
    public string Size { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime? InstallDate { get; set; }
    public bool IsUninstallable { get; set; }
}

public class FirewallRule
{
    public string Name { get; set; } = "";
    public string Direction { get; set; } = "";
    public string Action { get; set; } = "";
    public string Protocol { get; set; } = "";
    public string LocalPort { get; set; } = "";
    public string RemotePort { get; set; } = "";
    public string Program { get; set; } = "";
    public bool IsEnabled { get; set; }
    public string Profile { get; set; } = "";
}

public class NetworkAdapter
{
    public string Name { get; set; } = "";
    public string MacAddress { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public string SubnetMask { get; set; } = "";
    public string Gateway { get; set; } = "";
    public string Dns { get; set; } = "";
    public string Status { get; set; } = "";
    public long Speed { get; set; }
    public bool IsDhcp { get; set; }
}

public class DriveInfo2
{
    public string Letter { get; set; } = "";
    public string Label { get; set; } = "";
    public string FileSystem { get; set; } = "";
    public long TotalBytes { get; set; }
    public long FreeBytes { get; set; }
    public string DriveType { get; set; } = "";
    public bool IsReady { get; set; }
    public double PercentUsed => TotalBytes > 0 ? (double)(TotalBytes - FreeBytes) / TotalBytes * 100 : 0;
}

public class EnvironmentVar
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public string Scope { get; set; } = "";
    public bool IsPath { get; set; }
}

public class InstalledApp
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public string Publisher { get; set; } = "";
    public string InstallLocation { get; set; } = "";
    public string UninstallString { get; set; } = "";
    public DateTime? InstallDate { get; set; }
    public long SizeBytes { get; set; }
}
