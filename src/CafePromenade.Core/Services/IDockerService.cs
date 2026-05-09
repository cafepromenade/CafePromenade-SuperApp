using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface IDockerService
{
    Task<List<DockerContainer>> GetContainersAsync(bool all = false);
    Task<DockerContainer?> GetContainerAsync(string id);
    Task<bool> StartContainerAsync(string id);
    Task<bool> StopContainerAsync(string id);
    Task<bool> RestartContainerAsync(string id);
    Task<bool> RemoveContainerAsync(string id, bool force = false);
    Task<string> GetContainerLogsAsync(string id, int lines = 100);
    Task<List<DockerImage>> GetImagesAsync();
    Task<bool> PullImageAsync(string image);
    Task<bool> RemoveImageAsync(string id);
    Task<bool> BuildImageAsync(string path, string tag);
    Task<List<DockerNetwork>> GetNetworksAsync();
    Task<List<DockerVolume>> GetVolumesAsync();
    Task<bool> ComposeUpAsync(string path);
    Task<bool> ComposeDownAsync(string path);
    Task<bool> PruneAsync();
    Task<bool> IsDockerInstalledAsync();
    Task<bool> IsDockerRunningAsync();
    Task<string> GetDockerVersionAsync();
}
