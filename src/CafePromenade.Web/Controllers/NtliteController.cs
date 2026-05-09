using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NtliteController : ControllerBase
{
    private readonly INtliteService _service;
    public NtliteController(INtliteService service) => _service = service;

    [HttpGet("image")]
    public async Task<IActionResult> LoadImage([FromQuery] string path)
    {
        var info = await _service.LoadImageAsync(path);
        return info == null ? NotFound() : Ok(info);
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply([FromBody] NtlitePreset preset)
    {
        var result = await _service.ApplyPresetAsync(preset.ImagePath, preset);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("remove-components")]
    public async Task<IActionResult> RemoveComponents([FromQuery] string imagePath, [FromBody] List<string> components)
    {
        var result = await _service.RemoveComponentsAsync(imagePath, components);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("integrate-updates")]
    public async Task<IActionResult> IntegrateUpdates([FromQuery] string imagePath, [FromBody] List<string> updates)
    {
        var result = await _service.IntegrateUpdatesAsync(imagePath, updates);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("integrate-drivers")]
    public async Task<IActionResult> IntegrateDrivers([FromQuery] string imagePath, [FromBody] List<string> drivers)
    {
        var result = await _service.IntegrateDriversAsync(imagePath, drivers);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("features")]
    public async Task<IActionResult> SetFeatures([FromQuery] string imagePath, [FromBody] Dictionary<string, bool> features)
    {
        var result = await _service.SetFeaturesAsync(imagePath, features);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("tweaks")]
    public async Task<IActionResult> ApplyTweaks([FromQuery] string imagePath, [FromBody] Dictionary<string, string> tweaks)
    {
        var result = await _service.ApplyTweaksAsync(imagePath, tweaks);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("unattended")]
    public async Task<IActionResult> SetUnattended([FromQuery] string imagePath, [FromBody] UnattendedSettings settings)
    {
        var result = await _service.SetUnattendedAsync(imagePath, settings);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("create-iso")]
    public async Task<IActionResult> CreateIso([FromQuery] string outputPath, [FromQuery] string sourcePath)
    {
        var result = await _service.CreateIsoAsync(outputPath, sourcePath);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("preset/load")]
    public async Task<IActionResult> LoadPreset([FromQuery] string path) => Ok(await _service.LoadPresetAsync(path));

    [HttpPost("preset/save")]
    public async Task<IActionResult> SavePreset([FromBody] NtlitePreset preset, [FromQuery] string path)
    {
        var result = await _service.SavePresetAsync(preset, path);
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet("installed")]
    public async Task<IActionResult> IsInstalled() => Ok(await _service.IsNtliteInstalledAsync());
}
