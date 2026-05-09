using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface IWindowsTweakService
{
    Task<List<TweakCategory>> GetAllTweaksAsync();
    Task<TweakCategory?> GetCategoryAsync(string category);
    Task<WindowsTweak?> GetTweakAsync(string tweakId);
    Task<bool> ApplyTweakAsync(string tweakId);
    Task<bool> RevertTweakAsync(string tweakId);
    Task<bool> IsTweakAppliedAsync(string tweakId);
    Task<bool> ApplyCategoryAsync(string category);
    Task<bool> RevertCategoryAsync(string category);
    Task<bool> ApplyAllAsync();
    Task<bool> RevertAllAsync();

    Task<List<WindowsServiceInfo>> GetServicesAsync();
    Task<bool> DisableServiceAsync(string serviceName);
    Task<bool> EnableServiceAsync(string serviceName);
    Task<bool> StopServiceAsync(string serviceName);

    Task<List<ScheduledTaskInfo>> GetScheduledTasksAsync();
    Task<bool> DisableScheduledTaskAsync(string taskPath);
    Task<bool> EnableScheduledTaskAsync(string taskPath);

    Task<List<StartupItem>> GetStartupItemsAsync();
    Task<bool> DisableStartupItemAsync(string name);

    Task<List<ContextMenuEntry>> GetContextMenuEntriesAsync();
    Task<bool> RemoveContextMenuEntryAsync(string name);
    Task<bool> AddContextMenuEntryAsync(ContextMenuEntry entry);

    Task<List<WindowsUpdateInfo>> GetUpdatesAsync();
    Task<bool> UninstallUpdateAsync(string kb);

    Task<List<FirewallRule>> GetFirewallRulesAsync();
    Task<bool> ToggleFirewallRuleAsync(string name, bool enabled);
    Task<bool> AddFirewallRuleAsync(FirewallRule rule);

    Task<List<NetworkAdapter>> GetNetworkAdaptersAsync();
    Task<bool> SetStaticIpAsync(string adapterName, string ip, string subnet, string gateway, string dns);
    Task<bool> SetDhcpAsync(string adapterName);
    Task<bool> FlushDnsAsync();

    Task<List<DriveInfo2>> GetDrivesAsync();

    Task<List<EnvironmentVar>> GetEnvironmentVariablesAsync();
    Task<bool> SetEnvironmentVariableAsync(string name, string value, string scope);
    Task<bool> DeleteEnvironmentVariableAsync(string name, string scope);

    Task<List<InstalledApp>> GetInstalledAppsAsync();
    Task<bool> UninstallAppAsync(string name);

    Task<bool> SetUacLevelAsync(int level);
    Task<int> GetUacLevelAsync();
    Task<bool> DisableTelemetryAsync();
    Task<bool> EnableTelemetryAsync();
    Task<bool> DisableWindowsDefenderAsync();
    Task<bool> EnableWindowsDefenderAsync();
    Task<bool> DisableWindowsUpdateAsync();
    Task<bool> EnableWindowsUpdateAsync();
    Task<bool> DisableCortanaAsync();
    Task<bool> EnableCortanaAsync();
    Task<bool> DisableSearchIndexingAsync();
    Task<bool> EnableSearchIndexingAsync();
    Task<bool> DisableHibernationAsync();
    Task<bool> EnableHibernationAsync();
    Task<bool> DisableFastStartupAsync();
    Task<bool> EnableFastStartupAsync();
    Task<bool> DisableSuperfetchAsync();
    Task<bool> EnableSuperfetchAsync();
    Task<bool> DisableBITSAsync();
    Task<bool> EnableBITSAsync();
    Task<bool> DisableErrorReportingAsync();
    Task<bool> EnableErrorReportingAsync();
    Task<bool> DisableAutoRestartAsync();
    Task<bool> EnableAutoRestartAsync();
    Task<bool> ShowFileExtensionsAsync();
    Task<bool> HideFileExtensionsAsync();
    Task<bool> ShowHiddenFilesAsync();
    Task<bool> HideHiddenFilesAsync();
    Task<bool> ShowSystemFilesAsync();
    Task<bool> HideSystemFilesAsync();
    Task<bool> EnableDarkModeAsync();
    Task<bool> DisableDarkModeAsync();
    Task<bool> DisableLockScreenAsync();
    Task<bool> EnableLockScreenAsync();
    Task<bool> DisableActionCenterAsync();
    Task<bool> EnableActionCenterAsync();
    Task<bool> DisableStartMenuAdsAsync();
    Task<bool> EnableStartMenuAdsAsync();
    Task<bool> DisableTaskbarSearchAsync();
    Task<bool> EnableTaskbarSearchAsync();
    Task<bool> DisableTaskViewAsync();
    Task<bool> EnableTaskViewAsync();
    Task<bool> DisableNewsWidgetAsync();
    Task<bool> EnableNewsWidgetAsync();
    Task<bool> DisableCopilotAsync();
    Task<bool> EnableCopilotAsync();
    Task<bool> DisableEdgeTabsAsync();
    Task<bool> EnableEdgeTabsAsync();
    Task<bool> ClassicContextMenuAsync();
    Task<bool> ModernContextMenuAsync();
    Task<bool> DisableOneDriveAsync();
    Task<bool> EnableOneDriveAsync();
    Task<bool> DisableGameBarAsync();
    Task<bool> EnableGameBarAsync();
    Task<bool> DisableXboxAsync();
    Task<bool> EnableXboxAsync();
    Task<bool> DisableNotificationsAsync();
    Task<bool> EnableNotificationsAsync();
    Task<bool> DisableSmartScreenAsync();
    Task<bool> EnableSmartScreenAsync();
    Task<bool> DisableWindowsTipsAsync();
    Task<bool> EnableWindowsTipsAsync();
    Task<bool> DisableLocationAsync();
    Task<bool> EnableLocationAsync();
    Task<bool> DisableAdvertisingIdAsync();
    Task<bool> EnableAdvertisingIdAsync();
    Task<bool> DisableActivityHistoryAsync();
    Task<bool> EnableActivityHistoryAsync();
    Task<bool> DisableClipboardHistoryAsync();
    Task<bool> EnableClipboardHistoryAsync();
    Task<bool> DisableAutologgerAsync();
    Task<bool> EnableAutologgerAsync();
    Task<bool> DisableWifiSenseAsync();
    Task<bool> EnableWifiSenseAsync();
    Task<bool> DisableSyncAsync();
    Task<bool> EnableSyncAsync();
    Task<bool> DisableInputPersonalizationAsync();
    Task<bool> EnableInputPersonalizationAsync();
}
