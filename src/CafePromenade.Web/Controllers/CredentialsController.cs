using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController : ControllerBase
{
    private readonly ICredentialVaultService _vault;
    public CredentialsController(ICredentialVaultService vault) => _vault = vault;

    [HttpGet("config")]
    public async Task<IActionResult> GetConfig() => Ok(await _vault.GetConfigAsync());

    [HttpPost("unlock")]
    public async Task<IActionResult> Unlock([FromBody] UnlockRequest request)
    {
        var result = await _vault.UnlockAsync(request.Password);
        return result ? Ok(new { unlocked = true }) : Unauthorized(new { error = "Invalid master password" });
    }

    [HttpPost("lock")]
    public async Task<IActionResult> Lock() { await _vault.LockAsync(); return Ok(); }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        return Ok(new
        {
            locked = await _vault.IsLockedAsync(),
            count = await _vault.GetCredentialCountAsync(),
            defaultHost = await _vault.GetDefaultHostAsync(),
            defaultUsername = await _vault.GetDefaultUsernameAsync()
        });
    }

    [HttpPost("master-password")]
    public async Task<IActionResult> SetMasterPassword([FromBody] ChangeMasterRequest request)
    {
        var result = await _vault.SetMasterPasswordAsync(request.OldPassword, request.NewPassword);
        return result ? Ok() : StatusCode(500);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _vault.GetAllCredentialsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var cred = await _vault.GetCredentialAsync(id);
        return cred == null ? NotFound() : Ok(cred);
    }

    [HttpGet("service/{service}")]
    public async Task<IActionResult> GetByService(string service)
    {
        var cred = await _vault.GetCredentialByServiceAsync(service);
        return cred == null ? NotFound() : Ok(cred);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] CredentialVault credential)
    {
        var saved = await _vault.SaveCredentialAsync(credential);
        return Ok(saved);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _vault.DeleteCredentialAsync(id);
        return result ? Ok() : NotFound();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query) => Ok(await _vault.SearchCredentialsAsync(query));

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(CredentialCategory category) => Ok(await _vault.GetByCategoryAsync(category));

    [HttpGet("resolve/{service}")]
    public async Task<IActionResult> Resolve(string service) => Ok(await _vault.ResolveCredentialAsync(service));

    [HttpPost("default-host")]
    public async Task<IActionResult> SetDefaultHost([FromBody] DefaultRequest request)
    {
        await _vault.SaveDefaultHostAsync(request.Value);
        return Ok();
    }

    [HttpPost("default-username")]
    public async Task<IActionResult> SetDefaultUsername([FromBody] DefaultRequest request)
    {
        await _vault.SaveDefaultUsernameAsync(request.Value);
        return Ok();
    }

    [HttpGet("default-host")]
    public async Task<IActionResult> GetDefaultHost() => Ok(new { host = await _vault.GetDefaultHostAsync() });

    [HttpGet("default-username")]
    public async Task<IActionResult> GetDefaultUsername() => Ok(new { username = await _vault.GetDefaultUsernameAsync() });

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] ExportRequest request)
    {
        var result = await _vault.ExportVaultAsync(request.Path, request.Password);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromBody] ImportRequest request)
    {
        var result = await _vault.ImportVaultAsync(request.Path, request.Password);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("{id}/rotate")]
    public async Task<IActionResult> Rotate(string id)
    {
        var result = await _vault.RotateCredentialAsync(id);
        return result ? Ok() : NotFound();
    }

    [HttpPost("clear")]
    public async Task<IActionResult> Clear() { await _vault.ClearAllAsync(); return Ok(); }

    [HttpPost("quick-save")]
    public async Task<IActionResult> QuickSave([FromBody] QuickSaveRequest request)
    {
        var cred = new CredentialVault
        {
            Name = request.Name,
            Service = request.Service,
            Host = request.Host,
            Port = request.Port,
            Username = request.Username,
            EncryptedPassword = request.Password,
            Category = request.Category,
            Notes = request.Notes
        };
        var saved = await _vault.SaveCredentialAsync(cred);
        return Ok(saved);
    }
}

public class UnlockRequest { public string Password { get; set; } = ""; }
public class ChangeMasterRequest { public string OldPassword { get; set; } = ""; public string NewPassword { get; set; } = ""; }
public class DefaultRequest { public string Value { get; set; } = ""; }
public class ExportRequest { public string Path { get; set; } = ""; public string Password { get; set; } = ""; }
public class ImportRequest { public string Path { get; set; } = ""; public string Password { get; set; } = ""; }
public class QuickSaveRequest
{
    public string Name { get; set; } = "";
    public string Service { get; set; } = "";
    public string Host { get; set; } = "192.168.50.1";
    public int Port { get; set; }
    public string Username { get; set; } = "iRobot";
    public string Password { get; set; } = "";
    public CredentialCategory Category { get; set; } = CredentialCategory.General;
    public string Notes { get; set; } = "";
}
