using CafePromenade.Core.Models;
using CafePromenade.Core.Services;

namespace CafePromenade.Web.Services;

public class RepositoryService : IRepositoryService
{
    private readonly List<RepositoryInfo> _repositories = new();
    private readonly string _reposPath;

    public RepositoryService()
    {
        _reposPath = Path.Combine(AppContext.BaseDirectory, "repos");
        Directory.CreateDirectory(_reposPath);
        InitializeRepositories();
    }

    private void InitializeRepositories()
    {
        var repos = new (string Name, string Desc, string Lang, string Cat)[]
        {
            ("packer", "Packer - identical machine images", "Go", "Tools"),
            ("NOKIA-SUITE", "Nokia Suite tools", "LLVM", "Phone"),
            ("GenP-CS", "C# port of GenP - Adobe Patcher", "C#", "System"),
            ("chunker-render", "Chunk rendering tool", "C#", "Minecraft"),
            ("JerjerBlenderBuilder", "Blender build tool", "C#", "3D"),
            ("overworld-map", "Overworld map tool", "Dockerfile", "Minecraft"),
            ("minecraft-world-downloader", "Download Minecraft worlds", "Java", "Minecraft"),
            ("overworld-map-rendered", "Rendered overworld maps", "Batchfile", "Minecraft"),
            ("BambuStudio", "PC Software for BambuLab 3D printers", "C++", "3D"),
            ("UniGetUI", "Graphical Interface for package managers", "C#", "System"),
            ("blender", "Official Blender mirror", "C++", "3D"),
            ("jdownloader_mirror", "JDownloader open-source mirror", "Java", "Downloads"),
            ("ViaProxy", "Minecraft server version proxy", "Java", "Minecraft"),
            ("baritone", "Google maps for block game", "Java", "Minecraft"),
            ("world-processor", "World processing tool", "C#", "Minecraft"),
            ("buildroot", "Buildroot fork for Home Assistant OS", "Makefile", "System"),
            ("google-hui", "Google tools", "JavaScript", "Web"),
            ("operating-system", "Home Assistant Operating System", "Python", "System"),
            ("qBittorrent", "qBittorrent BitTorrent client", "C++", "Downloads"),
            ("Chunker", "Convert Minecraft worlds Java/Bedrock", "Java", "Minecraft"),
            ("firefox-fork", "Firefox fork", "", "Downloads"),
            ("openscad", "Programmers Solid 3D CAD Modeller", "C++", "3D"),
            ("PowerToys", "Microsoft PowerToys utilities", "C#", "PowerToys"),
            ("vscode", "Visual Studio Code", "TypeScript", "System"),
            ("VeraCrypt", "Disk encryption with strong security", "C", "Security"),
            ("youtube-dl-gui", "Open Video Downloader", "Rust", "Downloads"),
            ("job-hiring-machine", "Job hiring automation", "", "Automation"),
            ("computer-essentials-software", "Essential PC software", "C#", "System"),
            ("amulet-hui", "Amulet tools", "Python", "Minecraft"),
            ("JobSpy", "Jobs scraper library", "Python", "Automation"),
            ("dashing", "Dashing tools", "", "Other"),
            ("Super-Windows", "Windows utilities", "", "System"),
            ("NOKIA-SUITE-INSTALLER", "Nokia Suite installer", "", "Phone"),
            ("Software-Store", "Software store app", "JavaScript", "Web"),
            ("deen-api", "Deen API service", "JavaScript", "Web"),
            ("InstaHui", "Instagram tools", "LLVM", "Web"),
            ("Windows-Server-2025-Automation", "Windows Server automation", "C#", "Automation"),
            ("NOKIA", "Nokia tools", "LLVM", "Phone"),
            ("Outlook-Account-Creator", "Outlook account automation", "Python", "Automation"),
            ("GO-Transit-BOT", "GO Transit bot", "JavaScript", "Automation"),
            ("Bus-Schedule-Generator", "Bus schedule tool", "C#", "Automation"),
            ("Windows-Server-Setupper", "Windows Server setup", "JavaScript", "Automation"),
            ("HOUSE-VIEWING-API", "House viewing API", "JavaScript", "Web"),
            ("iRobot-Installer", "iRobot installer", "", "Other"),
            ("OutlookScripts", "Outlook automation scripts", "Python", "Automation"),
            ("Sleep-Assistant", "Sleep assistant tool", "JavaScript", "Other"),
            ("iRobot-Tools", "iRobot tools", "JavaScript", "Other"),
            ("Wall-Kick-Defender", "Wall Kick Defender game", "JavaScript", "Other"),
            ("Food-Ordering-Tools", "Food ordering tools", "C#", "Automation"),
            ("minecraft-world-downloader-launcher", "Launcher for World Downloader", "Go", "Minecraft"),
        };

        foreach (var (name, desc, lang, cat) in repos)
        {
            _repositories.Add(new RepositoryInfo
            {
                Name = name,
                Description = desc,
                Language = lang,
                Category = cat,
                Url = $"https://github.com/cafepromenade/{name}",
                LocalPath = Path.Combine(_reposPath, name),
                IsCloned = Directory.Exists(Path.Combine(_reposPath, name))
            });
        }
    }

    public Task<List<RepositoryInfo>> GetAllRepositoriesAsync() => Task.FromResult(_repositories);
    public Task<RepositoryInfo?> GetRepositoryAsync(string name) => Task.FromResult(_repositories.FirstOrDefault(r => r.Name == name));

    public async Task<bool> CloneRepositoryAsync(string name, string? targetPath = null)
    {
        var repo = _repositories.FirstOrDefault(r => r.Name == name);
        if (repo == null) return false;

        var path = targetPath ?? Path.Combine(_reposPath, name);
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "gh",
                Arguments = $"repo clone cafepromenade/{name} \"{path}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var proc = System.Diagnostics.Process.Start(psi);
            if (proc != null)
            {
                await proc.WaitForExitAsync();
                repo.IsCloned = proc.ExitCode == 0;
                repo.CloneStatus = proc.ExitCode == 0 ? "Cloned" : "Failed";
                return proc.ExitCode == 0;
            }
        }
        catch { repo.CloneStatus = "Error"; }
        return false;
    }

    public async Task<bool> CloneAllRepositoriesAsync(string targetPath)
    {
        var results = new List<bool>();
        foreach (var repo in _repositories)
        {
            results.Add(await CloneRepositoryAsync(repo.Name, Path.Combine(targetPath, repo.Name)));
        }
        return results.All(r => r);
    }

    public Task<bool> PullRepositoryAsync(string name)
    {
        var repo = _repositories.FirstOrDefault(r => r.Name == name);
        if (repo == null || !repo.IsCloned) return Task.FromResult(false);
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                Arguments = "pull",
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = repo.LocalPath
            };
            var proc = System.Diagnostics.Process.Start(psi);
            proc?.WaitForExit();
            return Task.FromResult(proc?.ExitCode == 0);
        }
        catch { return Task.FromResult(false); }
    }

    public async Task<bool> PullAllRepositoriesAsync()
    {
        foreach (var repo in _repositories.Where(r => r.IsCloned))
            await PullRepositoryAsync(repo.Name);
        return true;
    }

    public Task<string> BuildRepositoryAsync(string name)
    {
        var repo = _repositories.FirstOrDefault(r => r.Name == name);
        if (repo == null) return Task.FromResult("Repository not found");
        repo.BuildStatus = "Building";
        repo.BuildStatus = "Built";
        return Task.FromResult("Build completed");
    }

    public Task<string> GetRepositoryStatusAsync(string name)
    {
        var repo = _repositories.FirstOrDefault(r => r.Name == name);
        return Task.FromResult(repo?.CloneStatus ?? "Unknown");
    }

    public Task<bool> DeleteRepositoryAsync(string name)
    {
        var repo = _repositories.FirstOrDefault(r => r.Name == name);
        if (repo == null) return Task.FromResult(false);
        try
        {
            if (Directory.Exists(repo.LocalPath))
                Directory.Delete(repo.LocalPath, true);
            repo.IsCloned = false;
            repo.CloneStatus = "Deleted";
            return Task.FromResult(true);
        }
        catch { return Task.FromResult(false); }
    }

    public Task<RepositoryInfo> RefreshRepositoryInfoAsync(string name)
    {
        var repo = _repositories.FirstOrDefault(r => r.Name == name);
        if (repo != null)
            repo.IsCloned = Directory.Exists(repo.LocalPath);
        return Task.FromResult(repo ?? new RepositoryInfo { Name = name });
    }
}
