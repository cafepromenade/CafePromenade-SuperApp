using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly ISystemService _service;
    public SystemController(ISystemService service) => _service = service;

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus() => Ok(await _service.GetSystemStatusAsync());

    [HttpGet("powertoys")]
    public async Task<IActionResult> GetPowerToys() => Ok(await _service.GetPowerToyModulesAsync());

    [HttpPost("powertoys/{name}/toggle")]
    public async Task<IActionResult> TogglePowerToy(string name, [FromQuery] bool enabled)
    {
        var result = await _service.TogglePowerToyModuleAsync(name, enabled);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("powertoys/{name}/launch")]
    public async Task<IActionResult> LaunchPowerToy(string name)
    {
        var result = await _service.LaunchPowerToyModuleAsync(name);
        return result ? Ok() : StatusCode(500);
    }
}
