namespace CafePromenade.Core.Models;

public class DockerContainer
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Image { get; set; } = "";
    public string Status { get; set; } = "";
    public string Ports { get; set; } = "";
    public string Size { get; set; } = "";
    public DateTime Created { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
    public List<string> Networks { get; set; } = new();
    public List<string> Mounts { get; set; } = new();
}

public class DockerImage
{
    public string Id { get; set; } = "";
    public string Repository { get; set; } = "";
    public string Tag { get; set; } = "";
    public string Size { get; set; } = "";
    public DateTime Created { get; set; }
}

public class DockerComposeService
{
    public string Name { get; set; } = "";
    public string Image { get; set; } = "";
    public List<string> Ports { get; set; } = new();
    public Dictionary<string, string> Environment { get; set; } = new();
    public List<string> Volumes { get; set; } = new();
    public string Status { get; set; } = "";
}

public class DockerNetwork
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Driver { get; set; } = "";
    public string Scope { get; set; } = "";
    public List<string> Containers { get; set; } = new();
}

public class DockerVolume
{
    public string Name { get; set; } = "";
    public string Driver { get; set; } = "";
    public string Mountpoint { get; set; } = "";
    public Dictionary<string, string> Labels { get; set; } = new();
}
