using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface IVeraCryptService
{
    Task<List<VeraCryptVolume>> GetMountedVolumesAsync();
    Task<bool> CreateVolumeAsync(VeraCryptCreateOptions options);
    Task<bool> MountVolumeAsync(VeraCryptMountOptions options);
    Task<bool> DismountVolumeAsync(string driveLetter);
    Task<bool> DismountAllAsync();
    Task<bool> ChangePasswordAsync(string volumePath, string oldPassword, string newPassword);
    Task<bool> BackupHeaderAsync(string volumePath, string password, string backupPath);
    Task<bool> RestoreHeaderAsync(string volumePath, string password, string backupPath);
    Task<List<VeraCryptBenchmark>> RunBenchmarkAsync();
    Task<bool> EncryptSystemPartitionAsync();
    Task<bool> DecryptSystemPartitionAsync();
    Task<VeraCryptVolume?> GetVolumePropertiesAsync(string driveLetter);
    Task<bool> WipeCacheAsync();
    Task<bool> CreateKeyfileAsync(string path);
    Task<bool> RestoreVolumeHeaderAsync(string volumePath, string backupPath);
    Task<string> GetVersionAsync();
    Task<List<string>> GetAvailableDriveLettersAsync();
    Task<bool> IsVeraCryptInstalledAsync();
    Task<string> GetVeraCryptPathAsync();
}
