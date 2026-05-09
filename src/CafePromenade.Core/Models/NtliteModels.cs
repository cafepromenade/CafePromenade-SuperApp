namespace CafePromenade.Core.Models;

public class NtlitePreset
{
    public string Name { get; set; } = "";
    public string ImagePath { get; set; } = "";
    public int ImageIndex { get; set; } = 1;
    public List<string> RemovedComponents { get; set; } = new();
    public List<string> IntegratedUpdates { get; set; } = new();
    public List<string> IntegratedDrivers { get; set; } = new();
    public Dictionary<string, bool> Features { get; set; } = new();
    public Dictionary<string, string> Tweaks { get; set; } = new();
    public UnattendedSettings? Unattended { get; set; }
    public List<PostSetupCommand> PostSetupCommands { get; set; } = new();
}

public class UnattendedSettings
{
    public string Username { get; set; } = "";
    public string ComputerName { get; set; } = "";
    public string ProductKey { get; set; } = "";
    public string TimeZone { get; set; } = "";
    public string Language { get; set; } = "en-US";
    public bool AutoLogon { get; set; }
    public int AutoLogonCount { get; set; }
    public string NetworkType { get; set; } = "Home";
}

public class PostSetupCommand
{
    public string Command { get; set; } = "";
    public string Arguments { get; set; } = "";
    public int Order { get; set; }
    public bool Enabled { get; set; } = true;
}

public class WindowsImageInfo
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public string Architecture { get; set; } = "";
    public long SizeBytes { get; set; }
    public int IndexCount { get; set; }
    public List<ImageIndex> Indices { get; set; } = new();
}

public class ImageIndex
{
    public int Index { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public long SizeBytes { get; set; }
}
