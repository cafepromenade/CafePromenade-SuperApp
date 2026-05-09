using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepositoriesController : ControllerBase
{
    private readonly IRepositoryService _service;
    public RepositoriesController(IRepositoryService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllRepositoriesAsync());

    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name)
    {
        var repo = await _service.GetRepositoryAsync(name);
        return repo == null ? NotFound() : Ok(repo);
    }

    [HttpPost("{name}/clone")]
    public async Task<IActionResult> Clone(string name)
    {
        var result = await _service.CloneRepositoryAsync(name);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("clone-all")]
    public async Task<IActionResult> CloneAll([FromQuery] string path)
    {
        var result = await _service.CloneAllRepositoriesAsync(path);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("{name}/pull")]
    public async Task<IActionResult> Pull(string name)
    {
        var result = await _service.PullRepositoryAsync(name);
        return result ? Ok() : StatusCode(500);
    }

    [HttpPost("pull-all")]
    public async Task<IActionResult> PullAll()
    {
        await _service.PullAllRepositoriesAsync();
        return Ok();
    }

    [HttpPost("{name}/build")]
    public async Task<IActionResult> Build(string name) => Ok(await _service.BuildRepositoryAsync(name));

    [HttpGet("{name}/status")]
    public async Task<IActionResult> Status(string name) => Ok(await _service.GetRepositoryStatusAsync(name));

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        var result = await _service.DeleteRepositoryAsync(name);
        return result ? Ok() : NotFound();
    }
}
