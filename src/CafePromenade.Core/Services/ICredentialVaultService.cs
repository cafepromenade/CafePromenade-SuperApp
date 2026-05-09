using CafePromenade.Core.Models;

namespace CafePromenade.Core.Services;

public interface ICredentialVaultService
{
    Task<VaultConfig> GetConfigAsync();
    Task<bool> UnlockAsync(string masterPassword);
    Task<bool> LockAsync();
    Task<bool> IsLockedAsync();
    Task<bool> SetMasterPasswordAsync(string oldPassword, string newPassword);

    Task<CredentialVault> SaveCredentialAsync(CredentialVault credential);
    Task<List<CredentialVault>> GetAllCredentialsAsync();
    Task<CredentialVault?> GetCredentialAsync(string id);
    Task<CredentialVault?> GetCredentialByServiceAsync(string service);
    Task<bool> DeleteCredentialAsync(string id);
    Task<List<CredentialVault>> SearchCredentialsAsync(string query);
    Task<List<CredentialVault>> GetByCategoryAsync(CredentialCategory category);

    Task<bool> SaveDefaultHostAsync(string host);
    Task<bool> SaveDefaultUsernameAsync(string username);
    Task<string> GetDefaultHostAsync();
    Task<string> GetDefaultUsernameAsync();

    Task<StoredCredential> ResolveCredentialAsync(string service);
    Task<bool> ExportVaultAsync(string path, string password);
    Task<bool> ImportVaultAsync(string path, string password);
    Task<bool> RotateCredentialAsync(string id);
    Task<int> GetCredentialCountAsync();
    Task<bool> ClearAllAsync();
}
