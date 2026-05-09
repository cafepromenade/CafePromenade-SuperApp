using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeraCryptController : ControllerBase
{
    private readonly IVeraCryptService _service;
    public VeraCryptController(IVeraCryptService service) => _service = service;

    [HttpGet("volumes")]
    public async Task<IActionResult> GetMounted() => Ok(await _service.GetMountedVolumesAsync());

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] VeraCryptCreateOptions options)
    {
        var result = await _service.CreateVolumeAsync(options);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("mount")]
    public async Task<IActionResult> Mount([FromBody] VeraCryptMountOptions options)
    {
        var result = await _service.MountVolumeAsync(options);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("dismount/{driveLetter}")]
    public async Task<IActionResult> Dismount(string driveLetter)
    {
        var result = await _service.DismountVolumeAsync(driveLetter);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("dismount-all")]
    public async Task<IActionResult> DismountAll()
    {
        var result = await _service.DismountAllAsync();
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _service.ChangePasswordAsync(request.VolumePath, request.OldPassword, request.NewPassword);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("backup-header")]
    public async Task<IActionResult> BackupHeader([FromBody] HeaderRequest request)
    {
        var result = await _service.BackupHeaderAsync(request.VolumePath, request.Password, request.BackupPath);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("restore-header")]
    public async Task<IActionResult> RestoreHeader([FromBody] HeaderRequest request)
    {
        var result = await _service.RestoreHeaderAsync(request.VolumePath, request.Password, request.BackupPath);
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet("benchmark")]
    public async Task<IActionResult> Benchmark() => Ok(await _service.RunBenchmarkAsync());

    [HttpPost("encrypt-system")]
    public async Task<IActionResult> EncryptSystem()
    {
        var result = await _service.EncryptSystemPartitionAsync();
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("decrypt-system")]
    public async Task<IActionResult> DecryptSystem()
    {
        var result = await _service.DecryptSystemPartitionAsync();
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet("properties/{driveLetter}")]
    public async Task<IActionResult> Properties(string driveLetter) => Ok(await _service.GetVolumePropertiesAsync(driveLetter));

    [HttpPost("wipe-cache")]
    public async Task<IActionResult> WipeCache()
    {
        var result = await _service.WipeCacheAsync();
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("create-keyfile")]
    public async Task<IActionResult> CreateKeyfile([FromQuery] string path)
    {
        var result = await _service.CreateKeyfileAsync(path);
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet("version")]
    public async Task<IActionResult> Version() => Ok(await _service.GetVersionAsync());

    [HttpGet("drive-letters")]
    public async Task<IActionResult> DriveLetters() => Ok(await _service.GetAvailableDriveLettersAsync());

    [HttpGet("installed")]
    public async Task<IActionResult> IsInstalled() => Ok(await _service.IsVeraCryptInstalledAsync());

    [HttpGet("path")]
    public async Task<IActionResult> Path() => Ok(await _service.GetVeraCryptPathAsync());
}

public class ChangePasswordRequest
{
    public string VolumePath { get; set; } = "";
    public string OldPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
}

public class HeaderRequest
{
    public string VolumePath { get; set; } = "";
    public string Password { get; set; } = "";
    public string BackupPath { get; set; } = "";
}
