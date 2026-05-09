namespace CafePromenade.Core.Models;

public class CredentialVault
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Service { get; set; } = "";
    public string Host { get; set; } = "192.168.50.1";
    public int Port { get; set; }
    public string Username { get; set; } = "iRobot";
    public string EncryptedPassword { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string BotToken { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string CertificatePath { get; set; } = "";
    public string KeyFilePath { get; set; } = "";
    public Dictionary<string, string> CustomFields { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUsed { get; set; }
    public DateTime? LastRotated { get; set; }
    public bool IsDefault { get; set; }
    public string Notes { get; set; } = "";
    public CredentialCategory Category { get; set; } = CredentialCategory.General;
}

public enum CredentialCategory
{
    General,
    VeraCrypt,
    Telegram,
    Docker,
    SSH,
    FTP,
    Database,
    API,
    SMTP,
    VPN,
    SMB,
    SSH_KEY,
    WindowsAuth,
    Custom
}

public class VaultConfig
{
    public string VaultPath { get; set; } = "";
    public string MasterPasswordHash { get; set; } = "";
    public bool IsLocked { get; set; } = true;
    public DateTime? LastUnlocked { get; set; }
    public int AutoLockMinutes { get; set; } = 15;
    public bool UseWindowsCredentialStore { get; set; } = true;
    public string DefaultHost { get; set; } = "192.168.50.1";
    public string DefaultUsername { get; set; } = "iRobot";
    public int DefaultPort { get; set; } = 22;
}

public class StoredCredential
{
    public string ServiceName { get; set; } = "";
    public string Host { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public Dictionary<string, string> ExtraFields { get; set; } = new();
}
