using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WindowsTweaksController : ControllerBase
{
    private readonly IWindowsTweakService _service;
    public WindowsTweaksController(IWindowsTweakService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllTweaksAsync());

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetCategory(string category) => Ok(await _service.GetCategoryAsync(category));

    [HttpGet("{tweakId}")]
    public async Task<IActionResult> GetTweak(string tweakId)
    {
        var tweak = await _service.GetTweakAsync(tweakId);
        return tweak == null ? NotFound() : Ok(tweak);
    }

    [HttpPost("{tweakId}/apply")]
    public async Task<IActionResult> Apply(string tweakId)
    {
        var result = await _service.ApplyTweakAsync(tweakId);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("{tweakId}/revert")]
    public async Task<IActionResult> Revert(string tweakId)
    {
        var result = await _service.RevertTweakAsync(tweakId);
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet("{tweakId}/status")]
    public async Task<IActionResult> Status(string tweakId) => Ok(new { applied = await _service.IsTweakAppliedAsync(tweakId) });

    [HttpPost("category/{category}/apply")]
    public async Task<IActionResult> ApplyCategory(string category)
    {
        var result = await _service.ApplyCategoryAsync(category);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("category/{category}/revert")]
    public async Task<IActionResult> RevertCategory(string category)
    {
        var result = await _service.RevertCategoryAsync(category);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("apply-all")]
    public async Task<IActionResult> ApplyAll() { await _service.ApplyAllAsync(); return Ok(); }

    [HttpPost("revert-all")]
    public async Task<IActionResult> RevertAll() { await _service.RevertAllAsync(); return Ok(); }

    [HttpGet("services")]
    public async Task<IActionResult> GetServices() => Ok(await _service.GetServicesAsync());

    [HttpPost("services/{name}/disable")]
    public async Task<IActionResult> DisableService(string name) { await _service.DisableServiceAsync(name); return Ok(); }

    [HttpPost("services/{name}/enable")]
    public async Task<IActionResult> EnableService(string name) { await _service.EnableServiceAsync(name); return Ok(); }

    [HttpGet("scheduled-tasks")]
    public async Task<IActionResult> GetScheduledTasks() => Ok(await _service.GetScheduledTasksAsync());

    [HttpPost("scheduled-tasks/disable")]
    public async Task<IActionResult> DisableTask([FromQuery] string path) { await _service.DisableScheduledTaskAsync(path); return Ok(); }

    [HttpGet("startup")]
    public async Task<IActionResult> GetStartup() => Ok(await _service.GetStartupItemsAsync());

    [HttpGet("context-menu")]
    public async Task<IActionResult> GetContextMenu() => Ok(await _service.GetContextMenuEntriesAsync());

    [HttpGet("updates")]
    public async Task<IActionResult> GetUpdates() => Ok(await _service.GetUpdatesAsync());

    [HttpPost("updates/{kb}/uninstall")]
    public async Task<IActionResult> UninstallUpdate(string kb) { await _service.UninstallUpdateAsync(kb); return Ok(); }

    [HttpGet("firewall")]
    public async Task<IActionResult> GetFirewall() => Ok(await _service.GetFirewallRulesAsync());

    [HttpGet("network")]
    public async Task<IActionResult> GetNetwork() => Ok(await _service.GetNetworkAdaptersAsync());

    [HttpPost("network/static-ip")]
    public async Task<IActionResult> SetStaticIp([FromBody] StaticIpRequest request)
    {
        await _service.SetStaticIpAsync(request.Adapter, request.Ip, request.Subnet, request.Gateway, request.Dns);
        return Ok();
    }

    [HttpPost("network/dhcp")]
    public async Task<IActionResult> SetDhcp([FromQuery] string adapter) { await _service.SetDhcpAsync(adapter); return Ok(); }

    [HttpPost("network/flush-dns")]
    public async Task<IActionResult> FlushDns() { await _service.FlushDnsAsync(); return Ok(); }

    [HttpGet("drives")]
    public async Task<IActionResult> GetDrives() => Ok(await _service.GetDrivesAsync());

    [HttpGet("environment-variables")]
    public async Task<IActionResult> GetEnvVars() => Ok(await _service.GetEnvironmentVariablesAsync());

    [HttpPost("environment-variables")]
    public async Task<IActionResult> SetEnvVar([FromBody] EnvVarRequest request)
    {
        await _service.SetEnvironmentVariableAsync(request.Name, request.Value, request.Scope);
        return Ok();
    }

    [HttpDelete("environment-variables")]
    public async Task<IActionResult> DeleteEnvVar([FromQuery] string name, [FromQuery] string scope)
    {
        await _service.DeleteEnvironmentVariableAsync(name, scope);
        return Ok();
    }

    [HttpGet("installed-apps")]
    public async Task<IActionResult> GetInstalledApps() => Ok(await _service.GetInstalledAppsAsync());

    [HttpPost("uac")]
    public async Task<IActionResult> SetUac([FromQuery] int level) { await _service.SetUacLevelAsync(level); return Ok(); }

    [HttpGet("uac")]
    public async Task<IActionResult> GetUac() => Ok(new { level = await _service.GetUacLevelAsync() });

    [HttpPost("one-click/{tweak}")]
    public async Task<IActionResult> OneClick(string tweak)
    {
        var methods = new Dictionary<string, Func<Task<bool>>>
        {
            ["disable-telemetry"] = _service.DisableTelemetryAsync,
            ["enable-telemetry"] = _service.EnableTelemetryAsync,
            ["disable-defender"] = _service.DisableWindowsDefenderAsync,
            ["enable-defender"] = _service.EnableWindowsDefenderAsync,
            ["disable-update"] = _service.DisableWindowsUpdateAsync,
            ["enable-update"] = _service.EnableWindowsUpdateAsync,
            ["disable-cortana"] = _service.DisableCortanaAsync,
            ["enable-cortana"] = _service.EnableCortanaAsync,
            ["disable-copilot"] = _service.DisableCopilotAsync,
            ["enable-copilot"] = _service.EnableCopilotAsync,
            ["disable-onedrive"] = _service.DisableOneDriveAsync,
            ["enable-onedrive"] = _service.EnableOneDriveAsync,
            ["disable-xbox"] = _service.DisableXboxAsync,
            ["enable-xbox"] = _service.EnableXboxAsync,
            ["dark-mode"] = _service.EnableDarkModeAsync,
            ["light-mode"] = _service.DisableDarkModeAsync,
            ["classic-context"] = _service.ClassicContextMenuAsync,
            ["modern-context"] = _service.ModernContextMenuAsync,
            ["show-extensions"] = _service.ShowFileExtensionsAsync,
            ["hide-extensions"] = _service.HideFileExtensionsAsync,
            ["show-hidden"] = _service.ShowHiddenFilesAsync,
            ["hide-hidden"] = _service.HideHiddenFilesAsync,
            ["disable-lock-screen"] = _service.DisableLockScreenAsync,
            ["enable-lock-screen"] = _service.EnableLockScreenAsync,
            ["disable-notifications"] = _service.DisableNotificationsAsync,
            ["enable-notifications"] = _service.EnableNotificationsAsync,
            ["disable-hibernation"] = _service.DisableHibernationAsync,
            ["enable-hibernation"] = _service.EnableHibernationAsync,
            ["disable-fast-startup"] = _service.DisableFastStartupAsync,
            ["enable-fast-startup"] = _service.EnableFastStartupAsync,
            ["disable-superfetch"] = _service.DisableSuperfetchAsync,
            ["enable-superfetch"] = _service.EnableSuperfetchAsync,
            ["disable-search"] = _service.DisableSearchIndexingAsync,
            ["enable-search"] = _service.EnableSearchIndexingAsync,
            ["disable-game-bar"] = _service.DisableGameBarAsync,
            ["enable-game-bar"] = _service.EnableGameBarAsync,
            ["disable-advertising"] = _service.DisableAdvertisingIdAsync,
            ["enable-advertising"] = _service.EnableAdvertisingIdAsync,
            ["disable-activity"] = _service.DisableActivityHistoryAsync,
            ["enable-activity"] = _service.EnableActivityHistoryAsync,
            ["disable-location"] = _service.DisableLocationAsync,
            ["enable-location"] = _service.EnableLocationAsync,
            ["disable-tips"] = _service.DisableWindowsTipsAsync,
            ["enable-tips"] = _service.EnableWindowsTipsAsync,
            ["disable-smartscreen"] = _service.DisableSmartScreenAsync,
            ["enable-smartscreen"] = _service.EnableSmartScreenAsync,
        };
        if (methods.TryGetValue(tweak, out var method))
        {
            var result = await method();
            return result ? Ok() : StatusCode(500);
        }
        return NotFound(new { error = $"Unknown tweak: {tweak}" });
    }
}

public class StaticIpRequest { public string Adapter { get; set; } = ""; public string Ip { get; set; } = ""; public string Subnet { get; set; } = ""; public string Gateway { get; set; } = ""; public string Dns { get; set; } = ""; }
public class EnvVarRequest { public string Name { get; set; } = ""; public string Value { get; set; } = ""; public string Scope { get; set; } = "User"; }
