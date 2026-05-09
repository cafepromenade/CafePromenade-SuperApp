using Microsoft.AspNetCore.Mvc;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DockerController : ControllerBase
{
    private readonly IDockerService _service;
    public DockerController(IDockerService service) => _service = service;

    [HttpGet("containers")]
    public async Task<IActionResult> GetContainers([FromQuery] bool all = false) => Ok(await _service.GetContainersAsync(all));

    [HttpGet("containers/{id}")]
    public async Task<IActionResult> GetContainer(string id) => Ok(await _service.GetContainerAsync(id));

    [HttpPost("containers/{id}/start")]
    public async Task<IActionResult> Start(string id) { await _service.StartContainerAsync(id); return Ok(); }

    [HttpPost("containers/{id}/stop")]
    public async Task<IActionResult> Stop(string id) { await _service.StopContainerAsync(id); return Ok(); }

    [HttpPost("containers/{id}/restart")]
    public async Task<IActionResult> Restart(string id) { await _service.RestartContainerAsync(id); return Ok(); }

    [HttpDelete("containers/{id}")]
    public async Task<IActionResult> Remove(string id, [FromQuery] bool force = false) { await _service.RemoveContainerAsync(id, force); return Ok(); }

    [HttpGet("containers/{id}/logs")]
    public async Task<IActionResult> Logs(string id, [FromQuery] int lines = 100) => Ok(await _service.GetContainerLogsAsync(id, lines));

    [HttpGet("images")]
    public async Task<IActionResult> GetImages() => Ok(await _service.GetImagesAsync());

    [HttpPost("images/pull")]
    public async Task<IActionResult> Pull([FromQuery] string image) { await _service.PullImageAsync(image); return Ok(); }

    [HttpPost("compose/up")]
    public async Task<IActionResult> ComposeUp([FromQuery] string path) { await _service.ComposeUpAsync(path); return Ok(); }

    [HttpPost("compose/down")]
    public async Task<IActionResult> ComposeDown([FromQuery] string path) { await _service.ComposeDownAsync(path); return Ok(); }

    [HttpPost("prune")]
    public async Task<IActionResult> Prune() { await _service.PruneAsync(); return Ok(); }

    [HttpGet("status")]
    public async Task<IActionResult> Status() => Ok(new { installed = await _service.IsDockerInstalledAsync(), running = await _service.IsDockerRunningAsync(), version = await _service.GetDockerVersionAsync() });
}
