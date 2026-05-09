namespace CafePromenade.Core.Models;

public class RepositoryInfo
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Language { get; set; } = "";
    public int Stars { get; set; }
    public string Url { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime LastUpdated { get; set; }
    public bool IsCloned { get; set; }
    public string LocalPath { get; set; } = "";
    public string CloneStatus { get; set; } = "Not Cloned";
    public string BuildStatus { get; set; } = "Not Built";
}
