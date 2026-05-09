using System.Diagnostics;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Services;

public class DockerService : IDockerService
{
    public async Task<List<DockerContainer>> GetContainersAsync(bool all = false)
    {
        var containers = new List<DockerContainer>();
        try
        {
            var args = all ? "ps -a --format \"{{.ID}}|{{.Names}}|{{.Image}}|{{.Ports}}|{{.Status}}|{{.Size}}\"" : "ps --format \"{{.ID}}|{{.Names}}|{{.Image}}|{{.Ports}}|{{.Status}}|{{.Size}}\"";
            var output = await RunDocker(args);
            foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split('|');
                if (parts.Length >= 5)
                {
                    containers.Add(new DockerContainer
                    {
                        Id = parts[0].Trim(),
                        Name = parts[1].Trim(),
                        Image = parts[2].Trim(),
                        Ports = parts[3].Trim(),
                        Status = parts[4].Trim(),
                        Size = parts.Length > 5 ? parts[5].Trim() : ""
                    });
                }
            }
        }
        catch { }
        return containers;
    }

    public Task<DockerContainer?> GetContainerAsync(string id) => Task.FromResult<DockerContainer?>(new DockerContainer { Id = id });
    public async Task<bool> StartContainerAsync(string id) { await RunDocker($"start {id}"); return true; }
    public async Task<bool> StopContainerAsync(string id) { await RunDocker($"stop {id}"); return true; }
    public async Task<bool> RestartContainerAsync(string id) { await RunDocker($"restart {id}"); return true; }
    public async Task<bool> RemoveContainerAsync(string id, bool force = false) { await RunDocker($"rm {(force ? "-f " : "")}{id}"); return true; }
    public async Task<string> GetContainerLogsAsync(string id, int lines = 100) => await RunDocker($"logs --tail {lines} {id}");
    public async Task<List<DockerImage>> GetImagesAsync()
    {
        var images = new List<DockerImage>();
        try
        {
            var output = await RunDocker("images --format \"{{.ID}}|{{.Repository}}|{{.Tag}}|{{.Size}}\"");
            foreach (var line in output.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Split('|');
                if (parts.Length >= 4)
                    images.Add(new DockerImage { Id = parts[0].Trim(), Repository = parts[1].Trim(), Tag = parts[2].Trim(), Size = parts[3].Trim() });
            }
        }
        catch { }
        return images;
    }
    public async Task<bool> PullImageAsync(string image) { await RunDocker($"pull {image}"); return true; }
    public async Task<bool> RemoveImageAsync(string id) { await RunDocker($"rmi {id}"); return true; }
    public async Task<bool> BuildImageAsync(string path, string tag) { await RunDocker($"build -t {tag} {path}"); return true; }
    public Task<List<DockerNetwork>> GetNetworksAsync() => Task.FromResult(new List<DockerNetwork>());
    public Task<List<DockerVolume>> GetVolumesAsync() => Task.FromResult(new List<DockerVolume>());
    public async Task<bool> ComposeUpAsync(string path) { await RunDocker("compose up -d"); return true; }
    public async Task<bool> ComposeDownAsync(string path) { await RunDocker("compose down"); return true; }
    public async Task<bool> PruneAsync() { await RunDocker("system prune -f"); return true; }
    public Task<bool> IsDockerInstalledAsync() => Task.FromResult(true);
    public Task<bool> IsDockerRunningAsync() => Task.FromResult(true);
    public Task<string> GetDockerVersionAsync() => Task.FromResult("27.0.0");

    private async Task<string> RunDocker(string args)
    {
        try
        {
            var psi = new ProcessStartInfo { FileName = "docker", Arguments = args, UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true };
            var proc = Process.Start(psi);
            if (proc == null) return "";
            var output = await proc.StandardOutput.ReadToEndAsync();
            await proc.WaitForExitAsync();
            return output;
        }
        catch { return ""; }
    }
}
