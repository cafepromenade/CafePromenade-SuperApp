using System.Diagnostics;
using System.Runtime.InteropServices;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;
using Microsoft.Win32;

namespace CafePromenade.Web.Services;

public class WindowsTweakService : IWindowsTweakService
{
    private readonly List<TweakCategory> _categories;

    public WindowsTweakService()
    {
        _categories = BuildTweakCategories();
    }

    private List<TweakCategory> BuildTweakCategories()
    {
        return new List<TweakCategory>
        {
            new()
            {
                Name = "Privacy", Description = "Privacy and telemetry tweaks", Icon = "Shield",
                Tweaks = new()
                {
                    T("disable-telemetry", "Disable Telemetry", "Disable Windows telemetry and data collection", "Privacy", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, 1, TweakRisk.Medium),
                    T("disable-advertising-id", "Disable Advertising ID", "Disable advertising ID tracking", "Privacy", @"HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0, 1),
                    T("disable-activity-history", "Disable Activity History", "Disable activity history collection", "Privacy", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\System", "EnableActivityFeed", 0, 1),
                    T("disable-location", "Disable Location Tracking", "Disable location services", "Privacy", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors", "DisableLocation", 1, 0),
                    T("disable-input-personalization", "Disable Input Personalization", "Disable typing and inking personalization", "Privacy", @"HKCU\Software\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", 1, 0),
                    T("disable-autologger", "Disable AutoLogger", "Disable automatic logger", "Privacy", @"HKLM\SYSTEM\CurrentControlSet\Control\WMI\Autologger\AutoLogger-Diagtrack-Listener", "Start", 0, 1, TweakRisk.Medium),
                    T("disable-sync", "Disable Sync", "Disable settings sync across devices", "Privacy", @"HKCU\Software\Microsoft\Windows\CurrentVersion\SettingSync", "SyncPolicy", 5, 0),
                    T("disable-clipboard-history", "Disable Clipboard History", "Disable Windows clipboard history", "Privacy", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\System", "AllowClipboardHistory", 0, 1),
                    T("disable-wifi-sense", "Disable Wi-Fi Sense", "Disable Wi-Fi Sense auto-connect", "Privacy", @"HKLM\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\config", "AutoConnectAllowedOEM", 0, 1),
                }
            },
            new()
            {
                Name = "Windows Apps", Description = "Remove and disable built-in Windows apps", Icon = "Delete",
                Tweaks = new()
                {
                    T("disable-cortana", "Disable Cortana", "Disable Cortana digital assistant", "Windows Apps", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search", "AllowCortana", 0, 1, TweakRisk.Medium),
                    T("disable-copilot", "Disable Copilot", "Disable Windows Copilot AI", "Windows Apps", @"HKCU\Software\Policies\Microsoft\Windows\WindowsCopilot", "TurnOffWindowsCopilot", 1, 0, TweakRisk.Medium),
                    T("disable-onedrive", "Disable OneDrive", "Disable OneDrive integration", "Windows Apps", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\OneDrive", "DisableFileSyncNGSC", 1, 0, TweakRisk.Medium),
                    T("disable-news-widget", "Disable News Widget", "Disable Widgets and news feed", "Windows Apps", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarDa", 0, 1),
                    T("disable-taskbar-search", "Disable Taskbar Search", "Remove search from taskbar", "Windows Apps", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Search", "SearchboxTaskbarMode", 0, 1),
                    T("disable-task-view", "Disable Task View", "Remove Task View button from taskbar", "Windows Apps", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowTaskViewButton", 0, 1),
                    T("disable-edge-tabs", "Disable Edge Tab Previews", "Disable Edge tab previews in Alt+Tab", "Windows Apps", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\AltTabSettings", "VirtualDesktopAltTabFilter", 1, 0),
                    T("disable-start-menu-ads", "Disable Start Menu Ads", "Remove suggestions from Start menu", "Windows Apps", @"HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled", 0, 1),
                    T("disable-game-bar", "Disable Game Bar", "Disable Xbox Game Bar overlay", "Windows Apps", @"HKCU\Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0, 1),
                    T("disable-xbox", "Disable Xbox Services", "Disable Xbox Live services", "Windows Apps", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 0, 1, TweakRisk.Medium),
                }
            },
            new()
            {
                Name = "System", Description = "System performance and behavior tweaks", Icon = "Settings",
                Tweaks = new()
                {
                    T("disable-uac", "Disable UAC", "Disable User Account Control prompts", "System", @"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", 0, 1, TweakRisk.High, true),
                    T("disable-windows-update", "Disable Windows Update", "Disable automatic Windows updates", "System", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoUpdate", 1, 0, TweakRisk.High, true),
                    T("disable-windows-defender", "Disable Windows Defender", "Disable Windows Defender antivirus", "System", @"HKLM\SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", 1, 0, TweakRisk.High, true),
                    T("disable-search-indexing", "Disable Search Indexing", "Disable Windows Search indexing", "System", @"HKLM\SYSTEM\CurrentControlSet\Services\WSearch", "Start", 4, 2, TweakRisk.Medium, true),
                    T("disable-hibernation", "Disable Hibernation", "Disable hibernation and delete hiberfil.sys", "System", @"HKLM\SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled", 0, 1),
                    T("disable-fast-startup", "Disable Fast Startup", "Disable Windows fast startup", "System", @"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", 0, 1, TweakRisk.Medium),
                    T("disable-superfetch", "Disable Superfetch", "Disable Superfetch/SysMain service", "System", @"HKLM\SYSTEM\CurrentControlSet\Services\SysMain", "Start", 4, 2, TweakRisk.Medium, true),
                    T("disable-bits", "Disable BITS", "Disable Background Intelligent Transfer", "System", @"HKLM\SYSTEM\CurrentControlSet\Services\BITS", "Start", 4, 2, TweakRisk.High, true),
                    T("disable-error-reporting", "Disable Error Reporting", "Disable Windows Error Reporting", "System", @"HKLM\SOFTWARE\Microsoft\Windows\Windows Error Reporting", "Disabled", 1, 0),
                    T("disable-auto-restart", "Disable Auto Restart", "Disable auto restart after updates", "System", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU", "NoAutoRebootWithLoggedOnUsers", 1, 0),
                    T("disable-notifications", "Disable Notifications", "Disable toast notifications", "System", @"HKCU\Software\Microsoft\Windows\CurrentVersion\PushNotifications", "ToastEnabled", 0, 1),
                    T("disable-smartscreen", "Disable SmartScreen", "Disable SmartScreen filter", "System", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\System", "EnableSmartScreen", 0, 1, TweakRisk.Medium),
                    T("disable-windows-tips", "Disable Windows Tips", "Disable Windows tips and suggestions", "System", @"HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SoftLandingEnabled", 0, 1),
                    T("disable-lock-screen", "Disable Lock Screen", "Disable lock screen before login", "System", @"HKLM\SOFTWARE\Policies\Microsoft\Windows\Personalization", "NoLockScreen", 1, 0),
                    T("disable-action-center", "Disable Action Center", "Disable Action Center notifications", "System", @"HKCU\Software\Policies\Microsoft\Windows\Explorer", "DisableNotificationCenter", 1, 0),
                }
            },
            new()
            {
                Name = "Explorer", Description = "File Explorer and visual tweaks", Icon = "Folder",
                Tweaks = new()
                {
                    T("show-file-extensions", "Show File Extensions", "Always show file extensions in Explorer", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "HideFileExt", 0, 1),
                    T("show-hidden-files", "Show Hidden Files", "Show hidden files and folders", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "Hidden", 1, 2),
                    T("show-system-files", "Show System Files", "Show protected operating system files", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSuperHidden", 1, 0, TweakRisk.Medium),
                    T("classic-context-menu", "Classic Context Menu", "Restore classic right-click context menu", "Explorer", @"HKCU\Software\Classes\CLSID\{86ca1aa0-a74e-4293-abe8-d26b6e0e8f1d}\InprocServer32", "(Default)", "", "", TweakRisk.Low, false, true),
                    T("dark-mode", "Enable Dark Mode", "Enable system-wide dark mode", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 0, 1),
                    T("taskbar-small-icons", "Small Taskbar Icons", "Use small icons on taskbar", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarSmallIcons", 1, 0),
                    T("taskbar-never-combine", "Never Combine Taskbar", "Never combine taskbar buttons", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarGlomLevel", 2, 0),
                    T("show-seconds-clock", "Show Seconds in Clock", "Show seconds in system tray clock", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ShowSecondsInSystemClock", 1, 0),
                    T("disable-bing-search", "Disable Bing Search", "Disable Bing in Windows Search", "Explorer", @"HKCU\Software\Policies\Microsoft\Windows\Explorer", "DisableSearchBoxSuggestions", 1, 0),
                    T("this-pc-default", "Open This PC by Default", "Open This PC instead of Quick Access", "Explorer", @"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "LaunchTo", 1, 0),
                }
            },
            new()
            {
                Name = "Network", Description = "Network and internet tweaks", Icon = "Globe",
                Tweaks = new()
                {
                    T("disable-smb1", "Disable SMBv1", "Disable insecure SMBv1 protocol", "Network", @"HKLM\SYSTEM\CurrentControlSet\Services\LanmanServer\Parameters", "SMB1", 0, 1, TweakRisk.Medium),
                    T("disable-netbios", "Disable NetBIOS", "Disable NetBIOS over TCP/IP", "Network", @"HKLM\SYSTEM\CurrentControlSet\Services\NetBT\Parameters", "NodeType", 2, 1, TweakRisk.Medium),
                    T("disable-llmnr", "Disable LLMNR", "Disable Link-Local Multicast Name Resolution", "Network", @"HKLM\SOFTWARE\Policies\Microsoft\Windows NT\DNSClient", "EnableMulticast", 0, 1),
                    T("disable-tcp-timestamps", "Disable TCP Timestamps", "Disable TCP timestamps for performance", "Network", @"HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "Tcp1323Opts", 0, 3),
                    T("enable-network-discovery", "Enable Network Discovery", "Enable network discovery and file sharing", "Network", @"HKLM\SYSTEM\CurrentControlSet\Control\Network\NewNetworkWindowOff", "", "", "", TweakRisk.Low, false),
                }
            },
            new()
            {
                Name = "Context Menu", Description = "Right-click context menu customization", Icon = "List",
                Tweaks = new()
                {
                    T("add-cmd-here", "Add Command Prompt Here", "Add 'Open command window here' to context menu", "Context Menu", @"HKCR\Directory\shell\cmd", "", "", "", TweakRisk.Low, false),
                    T("add-powershell-here", "Add PowerShell Here", "Add 'Open PowerShell here' to context menu", "Context Menu", @"HKCR\Directory\shell\powershell", "", "", "", TweakRisk.Low, false),
                    T("add-new-folder", "Add New Folder Option", "Add 'New Folder' to context menu", "Context Menu", @"HKCR\Directory\Background\shell\NewFolder", "", "", "", TweakRisk.Low, false),
                    T("add-take-ownership", "Add Take Ownership", "Add 'Take Ownership' to context menu", "Context Menu", @"HKCR\*\shell\runas", "", "", "", TweakRisk.Low, false),
                    T("add-copy-path", "Add Copy Path", "Add 'Copy as path' to context menu", "Context Menu", @"HKCR\*\shell\copypath", "", "", "", TweakRisk.Low, false),
                    T("add-recycle-bin", "Add Recycle Bin to Context", "Add Recycle Bin options to desktop context menu", "Context Menu", @"HKCR\DesktopBackground\Shell\RecycleBin", "", "", "", TweakRisk.Low, false),
                }
            },
            new()
            {
                Name = "Services", Description = "Disable unnecessary Windows services", Icon = "Gears",
                Tweaks = new()
                {
                    T("disable-diagtrack", "Disable Diagnostics Tracking", "Disable Connected User Experiences", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\DiagTrack", "Start", 4, 2, TweakRisk.Medium),
                    T("disable-dmwappush", "Disable WAP Push", "Disable WAP Push Message Routing", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\dmwappushservice", "Start", 4, 2),
                    T("disable-print-spooler", "Disable Print Spooler", "Disable Print Spooler service", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\Spooler", "Start", 4, 2, TweakRisk.Medium),
                    T("disable-remote-registry", "Disable Remote Registry", "Disable Remote Registry service", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\RemoteRegistry", "Start", 4, 2),
                    T("disable-fax", "Disable Fax Service", "Disable Windows Fax service", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\Fax", "Start", 4, 2),
                    T("disable-xbox-services", "Disable Xbox Services", "Disable all Xbox Live services", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\XblAuthManager", "Start", 4, 2, TweakRisk.Medium),
                    T("disable-tablet-input", "Disable Tablet Input", "Disable Tablet PC Input service", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\TabletInputService", "Start", 4, 2),
                    T("disable-touch-keyboard", "Disable Touch Keyboard", "Disable Touch Keyboard service", "Services", @"HKLM\SYSTEM\CurrentControlSet\Services\TouchInputService", "Start", 4, 2),
                }
            }
        };
    }

    private static WindowsTweak T(string id, string name, string desc, string cat, string regPath, string regKey, object? tweaked, object? def, TweakRisk risk = TweakRisk.Low, bool restart = false, bool explorerRestart = false)
    {
        return new WindowsTweak
        {
            Id = id, Name = name, Description = desc, Category = cat,
            RegistryPath = regPath, RegistryKey = regKey,
            DefaultValue = def, TweakedValue = tweaked,
            Risk = risk, RequiresRestart = restart, RequiresExplorerRestart = explorerRestart,
            ValueType = tweaked is string ? TweakValueType.String : TweakValueType.DWord
        };
    }

    public Task<List<TweakCategory>> GetAllTweaksAsync() => Task.FromResult(_categories);
    public Task<TweakCategory?> GetCategoryAsync(string category) => Task.FromResult(_categories.FirstOrDefault(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase)));
    public Task<WindowsTweak?> GetTweakAsync(string tweakId) => Task.FromResult(_categories.SelectMany(c => c.Tweaks).FirstOrDefault(t => t.Id == tweakId));

    public Task<bool> ApplyTweakAsync(string tweakId)
    {
        var tweak = _categories.SelectMany(c => c.Tweaks).FirstOrDefault(t => t.Id == tweakId);
        if (tweak == null) return Task.FromResult(false);
        try
        {
            SetRegistry(tweak.RegistryPath, tweak.RegistryKey, tweak.TweakedValue, tweak.ValueType);
            tweak.IsApplied = true;
            if (tweak.RequiresExplorerRestart) RestartExplorer();
            return Task.FromResult(true);
        }
        catch { return Task.FromResult(false); }
    }

    public Task<bool> RevertTweakAsync(string tweakId)
    {
        var tweak = _categories.SelectMany(c => c.Tweaks).FirstOrDefault(t => t.Id == tweakId);
        if (tweak == null) return Task.FromResult(false);
        try
        {
            SetRegistry(tweak.RegistryPath, tweak.RegistryKey, tweak.DefaultValue, tweak.ValueType);
            tweak.IsApplied = false;
            if (tweak.RequiresExplorerRestart) RestartExplorer();
            return Task.FromResult(true);
        }
        catch { return Task.FromResult(false); }
    }

    public Task<bool> IsTweakAppliedAsync(string tweakId)
    {
        var tweak = _categories.SelectMany(c => c.Tweaks).FirstOrDefault(t => t.Id == tweakId);
        if (tweak == null || string.IsNullOrEmpty(tweak.RegistryKey)) return Task.FromResult(false);
        try
        {
            var current = GetRegistry(tweak.RegistryPath, tweak.RegistryKey);
            return Task.FromResult(current?.ToString() == tweak.TweakedValue?.ToString());
        }
        catch { return Task.FromResult(false); }
    }

    public async Task<bool> ApplyCategoryAsync(string category)
    {
        var cat = _categories.FirstOrDefault(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
        if (cat == null) return false;
        foreach (var tweak in cat.Tweaks) await ApplyTweakAsync(tweak.Id);
        return true;
    }

    public async Task<bool> RevertCategoryAsync(string category)
    {
        var cat = _categories.FirstOrDefault(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
        if (cat == null) return false;
        foreach (var tweak in cat.Tweaks) await RevertTweakAsync(tweak.Id);
        return true;
    }

    public async Task<bool> ApplyAllAsync() { foreach (var cat in _categories) await ApplyCategoryAsync(cat.Name); return true; }
    public async Task<bool> RevertAllAsync() { foreach (var cat in _categories) await RevertCategoryAsync(cat.Name); return true; }

    public Task<List<WindowsServiceInfo>> GetServicesAsync() => Task.FromResult(new List<WindowsServiceInfo>
    {
        new() { Name = "DiagTrack", DisplayName = "Connected User Experiences and Telemetry", Status = "Running", StartType = "Automatic", CanDisable = true, Description = "Collects telemetry data" },
        new() { Name = "dmwappushservice", DisplayName = "WAP Push Message Routing Service", Status = "Stopped", StartType = "Automatic", CanDisable = true },
        new() { Name = "SysMain", DisplayName = "SysMain (Superfetch)", Status = "Running", StartType = "Automatic", CanDisable = true },
        new() { Name = "WSearch", DisplayName = "Windows Search", Status = "Running", StartType = "Automatic", CanDisable = true },
        new() { Name = "Spooler", DisplayName = "Print Spooler", Status = "Running", StartType = "Automatic", CanDisable = true },
        new() { Name = "BITS", DisplayName = "Background Intelligent Transfer", Status = "Running", StartType = "Automatic", CanDisable = true },
        new() { Name = "RemoteRegistry", DisplayName = "Remote Registry", Status = "Stopped", StartType = "Manual", CanDisable = true },
        new() { Name = "Fax", DisplayName = "Fax", Status = "Stopped", StartType = "Manual", CanDisable = true },
        new() { Name = "XblAuthManager", DisplayName = "Xbox Live Auth Manager", Status = "Stopped", StartType = "Manual", CanDisable = true },
        new() { Name = "TabletInputService", DisplayName = "Touch Keyboard and Handwriting Panel", Status = "Running", StartType = "Automatic", CanDisable = true },
        new() { Name = "lfsvc", DisplayName = "Geolocation Service", Status = "Running", StartType = "Automatic", CanDisable = true },
        new() { Name = "MapsBroker", DisplayName = "Downloaded Maps Manager", Status = "Running", StartType = "Automatic", CanDisable = true },
    });

    public Task<bool> DisableServiceAsync(string serviceName) { RunCmd($"sc config {serviceName} start= disabled"); RunCmd($"sc stop {serviceName}"); return Task.FromResult(true); }
    public Task<bool> EnableServiceAsync(string serviceName) { RunCmd($"sc config {serviceName} start= auto"); RunCmd($"sc start {serviceName}"); return Task.FromResult(true); }
    public Task<bool> StopServiceAsync(string serviceName) { RunCmd($"sc stop {serviceName}"); return Task.FromResult(true); }

    public Task<List<ScheduledTaskInfo>> GetScheduledTasksAsync() => Task.FromResult(new List<ScheduledTaskInfo>
    {
        new() { Name = "MicrosoftCompatibilityAppraiser", Path = @"\Microsoft\Windows\Application Experience", Status = "Ready", IsEnabled = true },
        new() { Name = "ProgramDataUpdater", Path = @"\Microsoft\Windows\Application Experience", Status = "Ready", IsEnabled = true },
        new() { Name = "Consolidator", Path = @"\Microsoft\Windows\Customer Experience Improvement Program", Status = "Ready", IsEnabled = true },
        new() { Name = "UsbCeip", Path = @"\Microsoft\Windows\Customer Experience Improvement Program", Status = "Ready", IsEnabled = true },
        new() { Name = "Proxy", Path = @"\Microsoft\Windows\Customer Experience Improvement Program", Status = "Ready", IsEnabled = true },
        new() { Name = "BthSQM", Path = @"\Microsoft\Windows\Customer Experience Improvement Program", Status = "Ready", IsEnabled = true },
        new() { Name = "Uploader", Path = @"\Microsoft\Windows\DiskDiagnostic", Status = "Ready", IsEnabled = true },
    });

    public Task<bool> DisableScheduledTaskAsync(string taskPath) { RunCmd($"schtasks /Change /TN \"{taskPath}\" /Disable"); return Task.FromResult(true); }
    public Task<bool> EnableScheduledTaskAsync(string taskPath) { RunCmd($"schtasks /Change /TN \"{taskPath}\" /Enable"); return Task.FromResult(true); }

    public Task<List<StartupItem>> GetStartupItemsAsync() => Task.FromResult(new List<StartupItem>
    {
        new() { Name = "OneDrive", Command = @"C:\Users\%USERNAME%\AppData\Local\Microsoft\OneDrive\OneDrive.exe", Location = "HKCU\\Run", IsEnabled = true, Publisher = "Microsoft" },
        new() { Name = "Teams", Command = "ms-teams.exe", Location = "HKCU\\Run", IsEnabled = true, Publisher = "Microsoft" },
        new() { Name = "Spotify", Command = "Spotify.exe", Location = "HKCU\\Run", IsEnabled = true, Publisher = "Spotify AB" },
        new() { Name = "Discord", Command = "Discord.exe", Location = "HKCU\\Run", IsEnabled = true, Publisher = "Discord Inc." },
    });

    public Task<bool> DisableStartupItemAsync(string name)
    {
        SetRegistry(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Run", name, null, TweakValueType.String);
        return Task.FromResult(true);
    }

    public Task<List<ContextMenuEntry>> GetContextMenuEntriesAsync() => Task.FromResult(new List<ContextMenuEntry>
    {
        new() { Name = "Open with Notepad", FileType = "*", MenuType = "File", IsEnabled = true },
        new() { Name = "Take Ownership", FileType = "*", MenuType = "File", IsEnabled = true },
        new() { Name = "Copy as Path", FileType = "*", MenuType = "File", IsEnabled = true },
        new() { Name = "Open Command Prompt Here", FileType = "Directory", MenuType = "Folder", IsEnabled = true },
        new() { Name = "Open PowerShell Here", FileType = "Directory", MenuType = "Folder", IsEnabled = true },
    });

    public Task<bool> RemoveContextMenuEntryAsync(string name) => Task.FromResult(true);
    public Task<bool> AddContextMenuEntryAsync(ContextMenuEntry entry) => Task.FromResult(true);

    public Task<List<WindowsUpdateInfo>> GetUpdatesAsync() => Task.FromResult(new List<WindowsUpdateInfo>
    {
        new() { Title = "2026-05 Cumulative Update for Windows 11", KB = "KB5058411", Size = "450 MB", Status = "Installed" },
        new() { Title = "2026-04 Cumulative Update for Windows 11", KB = "KB5055523", Size = "380 MB", Status = "Installed" },
        new() { Title = "Windows Malicious Software Removal Tool", KB = "KB890830", Size = "75 MB", Status = "Installed" },
    });

    public Task<bool> UninstallUpdateAsync(string kb) { RunCmd($"wusa /uninstall /kb:{kb.Replace("KB", "")} /quiet /norestart"); return Task.FromResult(true); }

    public Task<List<FirewallRule>> GetFirewallRulesAsync() => Task.FromResult(new List<FirewallRule>
    {
        new() { Name = "File and Printer Sharing", Direction = "Inbound", Action = "Allow", Protocol = "TCP", LocalPort = "445", IsEnabled = true, Profile = "Domain,Private" },
        new() { Name = "Remote Desktop", Direction = "Inbound", Action = "Allow", Protocol = "TCP", LocalPort = "3389", IsEnabled = true, Profile = "Domain" },
        new() { Name = "Windows Remote Management", Direction = "Inbound", Action = "Allow", Protocol = "TCP", LocalPort = "5985", IsEnabled = true, Profile = "Domain" },
    });

    public Task<bool> ToggleFirewallRuleAsync(string name, bool enabled) => Task.FromResult(true);
    public Task<bool> AddFirewallRuleAsync(FirewallRule rule) => Task.FromResult(true);

    public Task<List<NetworkAdapter>> GetNetworkAdaptersAsync() => Task.FromResult(new List<NetworkAdapter>
    {
        new() { Name = "Ethernet", MacAddress = "00:1A:2B:3C:4D:5E", IpAddress = "192.168.50.1", SubnetMask = "255.255.255.0", Gateway = "192.168.50.1", Dns = "8.8.8.8", Status = "Up", Speed = 1000000000, IsDhcp = true },
        new() { Name = "Wi-Fi", MacAddress = "AA:BB:CC:DD:EE:FF", IpAddress = "192.168.50.100", Status = "Up", Speed = 866000000, IsDhcp = true },
    });

    public Task<bool> SetStaticIpAsync(string adapterName, string ip, string subnet, string gateway, string dns)
    {
        RunCmd($"netsh interface ip set address \"{adapterName}\" static {ip} {subnet} {gateway}");
        RunCmd($"netsh interface ip set dns \"{adapterName}\" static {dns}");
        return Task.FromResult(true);
    }

    public Task<bool> SetDhcpAsync(string adapterName)
    {
        RunCmd($"netsh interface ip set address \"{adapterName}\" dhcp");
        RunCmd($"netsh interface ip set dns \"{adapterName}\" dhcp");
        return Task.FromResult(true);
    }

    public Task<bool> FlushDnsAsync() { RunCmd("ipconfig /flushdns"); return Task.FromResult(true); }

    public Task<List<DriveInfo2>> GetDrivesAsync()
    {
        var drives = System.IO.DriveInfo.GetDrives().Select(d => new DriveInfo2
        {
            Letter = d.Name, Label = d.VolumeLabel, FileSystem = d.DriveFormat,
            TotalBytes = d.TotalSize, FreeBytes = d.TotalFreeSpace,
            DriveType = d.DriveType.ToString(), IsReady = d.IsReady
        }).ToList();
        return Task.FromResult(drives);
    }

    public Task<List<EnvironmentVar>> GetEnvironmentVariablesAsync()
    {
        var vars = new List<EnvironmentVar>();
        foreach (System.Collections.DictionaryEntry e in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine))
            vars.Add(new() { Name = e.Key.ToString()!, Value = e.Value.ToString()!, Scope = "Machine", IsPath = e.Key.ToString()!.Equals("PATH", StringComparison.OrdinalIgnoreCase) });
        foreach (System.Collections.DictionaryEntry e in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User))
            vars.Add(new() { Name = e.Key.ToString()!, Value = e.Value.ToString()!, Scope = "User", IsPath = e.Key.ToString()!.Equals("PATH", StringComparison.OrdinalIgnoreCase) });
        return Task.FromResult(vars);
    }

    public Task<bool> SetEnvironmentVariableAsync(string name, string value, string scope)
    {
        var target = scope == "Machine" ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;
        Environment.SetEnvironmentVariable(name, value, target);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteEnvironmentVariableAsync(string name, string scope)
    {
        var target = scope == "Machine" ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;
        Environment.SetEnvironmentVariable(name, null, target);
        return Task.FromResult(true);
    }

    public Task<List<InstalledApp>> GetInstalledAppsAsync() => Task.FromResult(new List<InstalledApp>
    {
        new() { Name = "Google Chrome", Version = "131.0.6778.86", Publisher = "Google LLC" },
        new() { Name = "Visual Studio Code", Version = "1.96.0", Publisher = "Microsoft Corporation" },
        new() { Name = "7-Zip", Version = "24.08", Publisher = "Igor Pavlov" },
        new() { Name = "VLC Media Player", Version = "3.0.21", Publisher = "VideoLAN" },
        new() { Name = "Git", Version = "2.47.0", Publisher = "The Git Development Community" },
    });

    public Task<bool> UninstallAppAsync(string name) { RunCmd($"wmic product where name='{name}' call uninstall /nointeractive"); return Task.FromResult(true); }

    // Winaero-style one-click tweaks
    public Task<bool> SetUacLevelAsync(int level) { SetRegistry(@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin", level, TweakValueType.DWord); return Task.FromResult(true); }
    public Task<int> GetUacLevelAsync()
    {
        var val = GetRegistry(@"HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "ConsentPromptBehaviorAdmin");
        return Task.FromResult(val is int i ? i : 5);
    }
    public Task<bool> DisableTelemetryAsync() => ApplyTweakAsync("disable-telemetry");
    public Task<bool> EnableTelemetryAsync() => RevertTweakAsync("disable-telemetry");
    public Task<bool> DisableWindowsDefenderAsync() => ApplyTweakAsync("disable-windows-defender");
    public Task<bool> EnableWindowsDefenderAsync() => RevertTweakAsync("disable-windows-defender");
    public Task<bool> DisableWindowsUpdateAsync() => ApplyTweakAsync("disable-windows-update");
    public Task<bool> EnableWindowsUpdateAsync() => RevertTweakAsync("disable-windows-update");
    public Task<bool> DisableCortanaAsync() => ApplyTweakAsync("disable-cortana");
    public Task<bool> EnableCortanaAsync() => RevertTweakAsync("disable-cortana");
    public Task<bool> DisableSearchIndexingAsync() => ApplyTweakAsync("disable-search-indexing");
    public Task<bool> EnableSearchIndexingAsync() => RevertTweakAsync("disable-search-indexing");
    public Task<bool> DisableHibernationAsync() => ApplyTweakAsync("disable-hibernation");
    public Task<bool> EnableHibernationAsync() => RevertTweakAsync("disable-hibernation");
    public Task<bool> DisableFastStartupAsync() => ApplyTweakAsync("disable-fast-startup");
    public Task<bool> EnableFastStartupAsync() => RevertTweakAsync("disable-fast-startup");
    public Task<bool> DisableSuperfetchAsync() => ApplyTweakAsync("disable-superfetch");
    public Task<bool> EnableSuperfetchAsync() => RevertTweakAsync("disable-superfetch");
    public Task<bool> DisableBITSAsync() => ApplyTweakAsync("disable-bits");
    public Task<bool> EnableBITSAsync() => RevertTweakAsync("disable-bits");
    public Task<bool> DisableErrorReportingAsync() => ApplyTweakAsync("disable-error-reporting");
    public Task<bool> EnableErrorReportingAsync() => RevertTweakAsync("disable-error-reporting");
    public Task<bool> DisableAutoRestartAsync() => ApplyTweakAsync("disable-auto-restart");
    public Task<bool> EnableAutoRestartAsync() => RevertTweakAsync("disable-auto-restart");
    public Task<bool> ShowFileExtensionsAsync() => ApplyTweakAsync("show-file-extensions");
    public Task<bool> HideFileExtensionsAsync() => RevertTweakAsync("show-file-extensions");
    public Task<bool> ShowHiddenFilesAsync() => ApplyTweakAsync("show-hidden-files");
    public Task<bool> HideHiddenFilesAsync() => RevertTweakAsync("show-hidden-files");
    public Task<bool> ShowSystemFilesAsync() => ApplyTweakAsync("show-system-files");
    public Task<bool> HideSystemFilesAsync() => RevertTweakAsync("show-system-files");
    public Task<bool> EnableDarkModeAsync() => ApplyTweakAsync("dark-mode");
    public Task<bool> DisableDarkModeAsync() => RevertTweakAsync("dark-mode");
    public Task<bool> DisableLockScreenAsync() => ApplyTweakAsync("disable-lock-screen");
    public Task<bool> EnableLockScreenAsync() => RevertTweakAsync("disable-lock-screen");
    public Task<bool> DisableActionCenterAsync() => ApplyTweakAsync("disable-action-center");
    public Task<bool> EnableActionCenterAsync() => RevertTweakAsync("disable-action-center");
    public Task<bool> DisableStartMenuAdsAsync() => ApplyTweakAsync("disable-start-menu-ads");
    public Task<bool> EnableStartMenuAdsAsync() => RevertTweakAsync("disable-start-menu-ads");
    public Task<bool> DisableTaskbarSearchAsync() => ApplyTweakAsync("disable-taskbar-search");
    public Task<bool> EnableTaskbarSearchAsync() => RevertTweakAsync("disable-taskbar-search");
    public Task<bool> DisableTaskViewAsync() => ApplyTweakAsync("disable-task-view");
    public Task<bool> EnableTaskViewAsync() => RevertTweakAsync("disable-task-view");
    public Task<bool> DisableNewsWidgetAsync() => ApplyTweakAsync("disable-news-widget");
    public Task<bool> EnableNewsWidgetAsync() => RevertTweakAsync("disable-news-widget");
    public Task<bool> DisableCopilotAsync() => ApplyTweakAsync("disable-copilot");
    public Task<bool> EnableCopilotAsync() => RevertTweakAsync("disable-copilot");
    public Task<bool> DisableEdgeTabsAsync() => ApplyTweakAsync("disable-edge-tabs");
    public Task<bool> EnableEdgeTabsAsync() => RevertTweakAsync("disable-edge-tabs");
    public Task<bool> ClassicContextMenuAsync() => ApplyTweakAsync("classic-context-menu");
    public Task<bool> ModernContextMenuAsync() => RevertTweakAsync("classic-context-menu");
    public Task<bool> DisableOneDriveAsync() => ApplyTweakAsync("disable-onedrive");
    public Task<bool> EnableOneDriveAsync() => RevertTweakAsync("disable-onedrive");
    public Task<bool> DisableGameBarAsync() => ApplyTweakAsync("disable-game-bar");
    public Task<bool> EnableGameBarAsync() => RevertTweakAsync("disable-game-bar");
    public Task<bool> DisableXboxAsync() => ApplyTweakAsync("disable-xbox-services");
    public Task<bool> EnableXboxAsync() => RevertTweakAsync("disable-xbox-services");
    public Task<bool> DisableNotificationsAsync() => ApplyTweakAsync("disable-notifications");
    public Task<bool> EnableNotificationsAsync() => RevertTweakAsync("disable-notifications");
    public Task<bool> DisableSmartScreenAsync() => ApplyTweakAsync("disable-smartscreen");
    public Task<bool> EnableSmartScreenAsync() => RevertTweakAsync("disable-smartscreen");
    public Task<bool> DisableWindowsTipsAsync() => ApplyTweakAsync("disable-windows-tips");
    public Task<bool> EnableWindowsTipsAsync() => RevertTweakAsync("disable-windows-tips");
    public Task<bool> DisableLocationAsync() => ApplyTweakAsync("disable-location");
    public Task<bool> EnableLocationAsync() => RevertTweakAsync("disable-location");
    public Task<bool> DisableAdvertisingIdAsync() => ApplyTweakAsync("disable-advertising-id");
    public Task<bool> EnableAdvertisingIdAsync() => RevertTweakAsync("disable-advertising-id");
    public Task<bool> DisableActivityHistoryAsync() => ApplyTweakAsync("disable-activity-history");
    public Task<bool> EnableActivityHistoryAsync() => RevertTweakAsync("disable-activity-history");
    public Task<bool> DisableClipboardHistoryAsync() => ApplyTweakAsync("disable-clipboard-history");
    public Task<bool> EnableClipboardHistoryAsync() => RevertTweakAsync("disable-clipboard-history");
    public Task<bool> DisableAutologgerAsync() => ApplyTweakAsync("disable-autologger");
    public Task<bool> EnableAutologgerAsync() => RevertTweakAsync("disable-autologger");
    public Task<bool> DisableWifiSenseAsync() => ApplyTweakAsync("disable-wifi-sense");
    public Task<bool> EnableWifiSenseAsync() => RevertTweakAsync("disable-wifi-sense");
    public Task<bool> DisableSyncAsync() => ApplyTweakAsync("disable-sync");
    public Task<bool> EnableSyncAsync() => RevertTweakAsync("disable-sync");
    public Task<bool> DisableInputPersonalizationAsync() => ApplyTweakAsync("disable-input-personalization");
    public Task<bool> EnableInputPersonalizationAsync() => RevertTweakAsync("disable-input-personalization");

    private static void SetRegistry(string path, string? key, object? value, TweakValueType type)
    {
        try
        {
            var hive = path.StartsWith(@"HKLM") ? RegistryHive.LocalMachine : RegistryHive.CurrentUser;
            var subPath = path.Replace(@"HKLM\", "").Replace(@"HKCU\", "").Replace(@"HKCR\", "");
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var subKey = baseKey.CreateSubKey(subPath, true);
            if (key != null && value != null)
            {
                subKey?.SetValue(key, value, type == TweakValueType.String ? RegistryValueKind.String : RegistryValueKind.DWord);
            }
            else if (key != null && value == null)
            {
                subKey?.DeleteValue(key, false);
            }
        }
        catch { }
    }

    private static object? GetRegistry(string path, string key)
    {
        try
        {
            var hive = path.StartsWith(@"HKLM") ? RegistryHive.LocalMachine : RegistryHive.CurrentUser;
            var subPath = path.Replace(@"HKLM\", "").Replace(@"HKCU\", "").Replace(@"HKCR\", "");
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64);
            using var subKey = baseKey.OpenSubKey(subPath);
            return subKey?.GetValue(key);
        }
        catch { return null; }
    }

    private static void RestartExplorer()
    {
        try
        {
            foreach (var proc in Process.GetProcessesByName("explorer")) proc.Kill();
            Process.Start("explorer.exe");
        }
        catch { }
    }

    private static void RunCmd(string cmd)
    {
        try
        {
            var psi = new ProcessStartInfo { FileName = "cmd", Arguments = $"/c {cmd}", UseShellExecute = false, CreateNoWindow = true };
            Process.Start(psi)?.WaitForExit(5000);
        }
        catch { }
    }
}
