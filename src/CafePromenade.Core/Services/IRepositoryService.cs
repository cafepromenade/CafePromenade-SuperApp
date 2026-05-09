using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface IRepositoryService
{
    Task<List<RepositoryInfo>> GetAllRepositoriesAsync();
    Task<RepositoryInfo?> GetRepositoryAsync(string name);
    Task<bool> CloneRepositoryAsync(string name, string? targetPath = null);
    Task<bool> CloneAllRepositoriesAsync(string targetPath);
    Task<bool> PullRepositoryAsync(string name);
    Task<bool> PullAllRepositoriesAsync();
    Task<string> BuildRepositoryAsync(string name);
    Task<string> GetRepositoryStatusAsync(string name);
    Task<bool> DeleteRepositoryAsync(string name);
    Task<RepositoryInfo> RefreshRepositoryInfoAsync(string name);
}
