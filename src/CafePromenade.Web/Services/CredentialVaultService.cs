using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Services;

public class CredentialVaultService : ICredentialVaultService
{
    private readonly string _vaultDir;
    private readonly string _configPath;
    private readonly string _credentialsPath;
    private VaultConfig _config;
    private List<CredentialVault> _credentials = new();
    private bool _unlocked;
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public CredentialVaultService()
    {
        _vaultDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CafePromenade", "Vault");
        Directory.CreateDirectory(_vaultDir);
        _configPath = Path.Combine(_vaultDir, "vault-config.json");
        _credentialsPath = Path.Combine(_vaultDir, "credentials.enc");
        _config = LoadConfig();
    }

    private VaultConfig LoadConfig()
    {
        if (File.Exists(_configPath))
        {
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<VaultConfig>(json) ?? CreateDefaultConfig();
        }
        return CreateDefaultConfig();
    }

    private VaultConfig CreateDefaultConfig()
    {
        var config = new VaultConfig
        {
            VaultPath = _vaultDir,
            DefaultHost = "192.168.50.1",
            DefaultUsername = "iRobot",
            DefaultPort = 22,
            UseWindowsCredentialStore = true,
            AutoLockMinutes = 15
        };
        SaveConfig(config);
        return config;
    }

    private void SaveConfig(VaultConfig config)
    {
        var json = JsonSerializer.Serialize(config, _jsonOptions);
        File.WriteAllText(_configPath, json);
    }

    private void SaveCredentials()
    {
        var json = JsonSerializer.Serialize(_credentials, _jsonOptions);
        var encrypted = EncryptString(json, _config.MasterPasswordHash);
        File.WriteAllText(_credentialsPath, encrypted);
    }

    private void LoadCredentials()
    {
        if (!File.Exists(_credentialsPath)) { _credentials = new(); return; }
        try
        {
            var encrypted = File.ReadAllText(_credentialsPath);
            var json = DecryptString(encrypted, _config.MasterPasswordHash);
            _credentials = JsonSerializer.Deserialize<List<CredentialVault>>(json) ?? new();
        }
        catch { _credentials = new(); }
    }

    public Task<VaultConfig> GetConfigAsync() => Task.FromResult(_config);

    public Task<bool> UnlockAsync(string masterPassword)
    {
        if (string.IsNullOrEmpty(_config.MasterPasswordHash))
        {
            _config.MasterPasswordHash = HashPassword(masterPassword);
            SaveConfig(_config);
            _unlocked = true;
            LoadCredentials();
            return Task.FromResult(true);
        }

        if (VerifyPassword(masterPassword, _config.MasterPasswordHash))
        {
            _unlocked = true;
            _config.IsLocked = false;
            _config.LastUnlocked = DateTime.UtcNow;
            SaveConfig(_config);
            LoadCredentials();
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> LockAsync()
    {
        _unlocked = false;
        _config.IsLocked = true;
        SaveConfig(_config);
        _credentials.Clear();
        return Task.FromResult(true);
    }

    public Task<bool> IsLockedAsync() => Task.FromResult(!_unlocked);

    public Task<bool> SetMasterPasswordAsync(string oldPassword, string newPassword)
    {
        if (!string.IsNullOrEmpty(_config.MasterPasswordHash) && !VerifyPassword(oldPassword, _config.MasterPasswordHash))
            return Task.FromResult(false);

        _config.MasterPasswordHash = HashPassword(newPassword);
        SaveConfig(_config);
        SaveCredentials();
        return Task.FromResult(true);
    }

    public Task<CredentialVault> SaveCredentialAsync(CredentialVault credential)
    {
        var existing = _credentials.FirstOrDefault(c => c.Id == credential.Id);
        if (existing != null)
        {
            var idx = _credentials.IndexOf(existing);
            credential.CreatedAt = existing.CreatedAt;
            _credentials[idx] = credential;
        }
        else
        {
            credential.Id = Guid.NewGuid().ToString();
            credential.CreatedAt = DateTime.UtcNow;
            _credentials.Add(credential);
        }
        SaveCredentials();
        return Task.FromResult(credential);
    }

    public Task<List<CredentialVault>> GetAllCredentialsAsync() => Task.FromResult(_credentials.ToList());
    public Task<CredentialVault?> GetCredentialAsync(string id) => Task.FromResult(_credentials.FirstOrDefault(c => c.Id == id));
    public Task<CredentialVault?> GetCredentialByServiceAsync(string service) => Task.FromResult(_credentials.FirstOrDefault(c => c.Service.Equals(service, StringComparison.OrdinalIgnoreCase)));

    public Task<bool> DeleteCredentialAsync(string id)
    {
        var cred = _credentials.FirstOrDefault(c => c.Id == id);
        if (cred == null) return Task.FromResult(false);
        _credentials.Remove(cred);
        SaveCredentials();
        return Task.FromResult(true);
    }

    public Task<List<CredentialVault>> SearchCredentialsAsync(string query)
    {
        var results = _credentials.Where(c =>
            c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Service.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Host.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Username.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Notes.Contains(query, StringComparison.OrdinalIgnoreCase)
        ).ToList();
        return Task.FromResult(results);
    }

    public Task<List<CredentialVault>> GetByCategoryAsync(CredentialCategory category)
    {
        return Task.FromResult(_credentials.Where(c => c.Category == category).ToList());
    }

    public Task<bool> SaveDefaultHostAsync(string host)
    {
        _config.DefaultHost = host;
        SaveConfig(_config);
        return Task.FromResult(true);
    }

    public Task<bool> SaveDefaultUsernameAsync(string username)
    {
        _config.DefaultUsername = username;
        SaveConfig(_config);
        return Task.FromResult(true);
    }

    public Task<string> GetDefaultHostAsync() => Task.FromResult(_config.DefaultHost);
    public Task<string> GetDefaultUsernameAsync() => Task.FromResult(_config.DefaultUsername);

    public async Task<StoredCredential> ResolveCredentialAsync(string service)
    {
        var cred = _credentials.FirstOrDefault(c => c.Service.Equals(service, StringComparison.OrdinalIgnoreCase));
        if (cred == null)
        {
            return new StoredCredential
            {
                ServiceName = service,
                Host = _config.DefaultHost,
                Username = _config.DefaultUsername
            };
        }
        cred.LastUsed = DateTime.UtcNow;
        SaveCredentials();
        return new StoredCredential
        {
            ServiceName = cred.Service,
            Host = cred.Host,
            Username = cred.Username,
            Password = DecryptString(cred.EncryptedPassword, _config.MasterPasswordHash),
            ExtraFields = cred.CustomFields
        };
    }

    public Task<bool> ExportVaultAsync(string path, string password)
    {
        var exportData = new
        {
            Config = _config,
            Credentials = _credentials,
            ExportedAt = DateTime.UtcNow,
            Version = "1.0"
        };
        var json = JsonSerializer.Serialize(exportData, _jsonOptions);
        var encrypted = EncryptString(json, HashPassword(password));
        File.WriteAllText(path, encrypted);
        return Task.FromResult(true);
    }

    public Task<bool> ImportVaultAsync(string path, string password)
    {
        if (!File.Exists(path)) return Task.FromResult(false);
        try
        {
            var encrypted = File.ReadAllText(path);
            var json = DecryptString(encrypted, HashPassword(password));
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            if (data.TryGetProperty("Credentials", out var creds))
            {
                var imported = JsonSerializer.Deserialize<List<CredentialVault>>(creds.GetRawText()) ?? new();
                foreach (var cred in imported)
                {
                    cred.Id = Guid.NewGuid().ToString();
                    _credentials.Add(cred);
                }
                SaveCredentials();
            }
            return Task.FromResult(true);
        }
        catch { return Task.FromResult(false); }
    }

    public Task<bool> RotateCredentialAsync(string id)
    {
        var cred = _credentials.FirstOrDefault(c => c.Id == id);
        if (cred == null) return Task.FromResult(false);
        var newPass = GeneratePassword(24);
        cred.EncryptedPassword = EncryptString(newPass, _config.MasterPasswordHash);
        cred.LastRotated = DateTime.UtcNow;
        SaveCredentials();
        return Task.FromResult(true);
    }

    public Task<int> GetCredentialCountAsync() => Task.FromResult(_credentials.Count);

    public Task<bool> ClearAllAsync()
    {
        _credentials.Clear();
        SaveCredentials();
        return Task.FromResult(true);
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);
            var testHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(hash, testHash);
        }
        catch { return false; }
    }

    private static string EncryptString(string plainText, string key)
    {
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        var iv = RandomNumberGenerator.GetBytes(16);
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = iv;
        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(iv) + ":" + Convert.ToBase64String(encrypted);
    }

    private static string DecryptString(string cipherText, string key)
    {
        var parts = cipherText.Split(':');
        if (parts.Length != 2) throw new FormatException("Invalid cipher format");
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        var iv = Convert.FromBase64String(parts[0]);
        var cipher = Convert.FromBase64String(parts[1]);
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor();
        var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

    private static string GeneratePassword(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+";
        var bytes = RandomNumberGenerator.GetBytes(length);
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
            sb.Append(chars[bytes[i] % chars.Length]);
        return sb.ToString();
    }
}
