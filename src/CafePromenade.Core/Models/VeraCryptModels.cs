namespace CafePromenade.Core.Models;

public class VeraCryptVolume
{
    public string VolumePath { get; set; } = "";
    public string DriveLetter { get; set; } = "";
    public string EncryptionAlgorithm { get; set; } = "AES";
    public string HashAlgorithm { get; set; } = "SHA-512";
    public string FileSystem { get; set; } = "NTFS";
    public long SizeBytes { get; set; }
    public bool IsMounted { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? MountedAt { get; set; }
}

public class VeraCryptBenchmark
{
    public string Algorithm { get; set; } = "";
    public double EncryptionSpeed { get; set; }
    public double DecryptionSpeed { get; set; }
    public double RandomSpeed { get; set; }
    public string Unit { get; set; } = "MB/s";
}

public class VeraCryptCreateOptions
{
    public string VolumePath { get; set; } = "";
    public string Password { get; set; } = "";
    public long SizeBytes { get; set; }
    public string Encryption { get; set; } = "AES";
    public string Hash { get; set; } = "SHA-512";
    public string FileSystem { get; set; } = "NTFS";
    public bool IsHidden { get; set; }
    public string Keyfile { get; set; } = "";
    public int Pim { get; set; }
}

public class VeraCryptMountOptions
{
    public string VolumePath { get; set; } = "";
    public string Password { get; set; } = "";
    public string DriveLetter { get; set; } = "";
    public bool ReadOnly { get; set; }
    public bool Removable { get; set; }
    public int Pim { get; set; }
    public string Keyfile { get; set; } = "";
    public bool CachePassword { get; set; }
}
