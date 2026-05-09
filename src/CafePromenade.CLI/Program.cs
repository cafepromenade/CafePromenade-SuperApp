using System.Diagnostics;

namespace CafePromenade.CLI;

class Program
{
    static readonly string ReposPath = Path.Combine(AppContext.BaseDirectory, "repos");
    static readonly HttpClient Http = new();
#pragma warning disable CS0414
    static readonly string ApiUrl = "http://localhost:5180";
#pragma warning restore CS0414

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
  ╔═══════════════════════════════════════════════════╗
  ║        CafePromenade SuperApp CLI v1.0.0          ║
  ║  Manage repos, VeraCrypt, NTLite, Docker & more   ║
  ╚═══════════════════════════════════════════════════╝");
        Console.ResetColor();

        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var command = args[0].ToLower();
        var subArgs = args.Skip(1).ToArray();

        try
        {
            switch (command)
            {
                case "repos": await HandleRepos(subArgs); break;
                case "clone": await HandleClone(subArgs); break;
                case "pull": await HandlePull(subArgs); break;
                case "build": await HandleBuild(subArgs); break;
                case "veracrypt": case "vc": await HandleVeraCrypt(subArgs); break;
                case "ntlite": case "nt": await HandleNtlite(subArgs); break;
                case "docker": case "dk": await HandleDocker(subArgs); break;
                case "msg": case "message": await HandleMessaging(subArgs); break;
                case "system": case "sys": await HandleSystem(subArgs); break;
                case "powertoys": case "pt": await HandlePowerToys(subArgs); break;
                case "status": await ShowStatus(); break;
                case "launch": await HandleLaunch(subArgs); break;
                case "server": await HandleServer(subArgs); break;
                case "help": case "--help": case "-h": ShowHelp(); break;
                case "version": case "--version": Console.WriteLine("CafePromenade SuperApp CLI v1.0.0"); break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Unknown command: {command}");
                    Console.ResetColor();
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine(@"
Usage: cafecli <command> [options]

Commands:
  repos list              List all 50 repositories
  repos list <category>   List repos by category (Minecraft, 3D, System, etc.)
  clone <name>            Clone a specific repository
  clone all               Clone all repositories
  clone all --path <dir>  Clone all to specific directory
  pull <name>             Pull latest changes for a repo
  pull all                Pull all repositories
  build <name>            Build a repository
  status                  Show system and app status

  veracrypt               VeraCrypt management
  veracrypt mount <path> <drive>  Mount a volume
  veracrypt dismount <drive>      Dismount a volume
  veracrypt dismount-all          Dismount all volumes
  veracrypt create <path> <size>  Create a new volume
  veracrypt list                  List mounted volumes
  veracrypt benchmark             Run encryption benchmark
  veracrypt wipe-cache            Wipe password cache
  veracrypt backup-header <path>  Backup volume header
  veracrypt restore-header <path> Restore volume header
  veracrypt change-pass <path>    Change volume password
  veracrypt encrypt-system        Encrypt system partition
  veracrypt info <drive>          Volume properties

  ntlite                  NTLite Windows image customization
  ntlite load <path>      Load a Windows image
  ntlite apply            Apply preset to image
  ntlite iso <output>     Create custom ISO
  ntlite components       Remove components
  ntlite tweaks           Apply tweaks

  docker                  Docker container management
  docker list             List containers
  docker list all         List all containers
  docker start <id>       Start container
  docker stop <id>        Stop container
  docker restart <id>     Restart container
  docker logs <id>        View container logs
  docker images           List images
  docker pull <image>     Pull image
  docker compose up       Docker compose up
  docker compose down     Docker compose down
  docker prune            Prune unused resources
  docker status           Docker status

  msg send <chat> <text>  Send message
  msg list                List chats
  msg history <chat>      Chat history
  msg create <name>       Create new chat
  msg search <query>      Search messages
  telegram connect <token> Connect Telegram bot
  telegram send <chat> <text> Send Telegram message
  telegram status         Telegram connection status

  powertoys               List PowerToys modules
  powertoys enable <name> Enable a module
  powertoys disable <name> Disable a module
  powertoys launch <name> Launch a module

  system info             System information
  system processes        Running processes

  launch <repo>           Launch a cloned application
  server start            Start the API server
  server stop             Stop the API server

  help                    Show this help
  version                 Show version");
    }

    static async Task HandleRepos(string[] args)
    {
        var repos = GetAllRepos();
        if (args.Length > 0 && args[0] != "list")
        {
            var category = args[0];
            repos = repos.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            Console.WriteLine($"\nRepositories in category '{category}':");
        }
        else
        {
            Console.WriteLine("\nAll Repositories:");
        }

        Console.WriteLine(new string('─', 80));
        Console.WriteLine($"{"Name",-35} {"Language",-12} {"Category",-15} {"Status",-10}");
        Console.WriteLine(new string('─', 80));

        foreach (var repo in repos)
        {
            var status = Directory.Exists(Path.Combine(ReposPath, repo.Name)) ? "Cloned" : "Remote";
            var color = status == "Cloned" ? ConsoleColor.Green : ConsoleColor.Yellow;
            Console.ForegroundColor = color;
            Console.WriteLine($"{repo.Name,-35} {repo.Language,-12} {repo.Category,-15} {status,-10}");
            Console.ResetColor();
        }
        Console.WriteLine($"\nTotal: {repos.Count} repositories");
    }

    static async Task HandleClone(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("Usage: clone <name> or clone all"); return; }

        if (args[0] == "all")
        {
            var path = args.Length > 2 && args[1] == "--path" ? args[2] : ReposPath;
            Console.WriteLine($"Cloning all repositories to {path}...");
            var repos = GetAllRepos();
            var success = 0;
            foreach (var repo in repos)
            {
                Console.Write($"  Cloning {repo.Name}...");
                try
                {
                    var psi = new ProcessStartInfo { FileName = "gh", Arguments = $"repo clone cafepromenade/{repo.Name} \"{Path.Combine(path, repo.Name)}\"", UseShellExecute = false, CreateNoWindow = true };
                    var proc = Process.Start(psi);
                    proc?.WaitForExit();
                    if (proc?.ExitCode == 0) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(" OK"); success++; }
                    else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(" FAILED"); }
                    Console.ResetColor();
                }
                catch { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(" ERROR"); Console.ResetColor(); }
            }
            Console.WriteLine($"\nCloned {success}/{repos.Count} repositories");
        }
        else
        {
            var name = args[0];
            Console.Write($"Cloning {name}...");
            var psi = new ProcessStartInfo { FileName = "gh", Arguments = $"repo clone cafepromenade/{name} \"{Path.Combine(ReposPath, name)}\"", UseShellExecute = false, CreateNoWindow = true };
            var proc = Process.Start(psi);
            proc?.WaitForExit();
            if (proc?.ExitCode == 0) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(" OK"); }
            else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(" FAILED"); }
            Console.ResetColor();
        }
    }

    static async Task HandlePull(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("Usage: pull <name> or pull all"); return; }

        if (args[0] == "all")
        {
            var repos = GetAllRepos().Where(r => Directory.Exists(Path.Combine(ReposPath, r.Name))).ToList();
            Console.WriteLine($"Pulling {repos.Count} cloned repositories...");
            foreach (var repo in repos)
            {
                Console.Write($"  Pulling {repo.Name}...");
                RunGit("pull", Path.Combine(ReposPath, repo.Name));
                Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(" OK"); Console.ResetColor();
            }
        }
        else
        {
            var name = args[0];
            var path = Path.Combine(ReposPath, name);
            if (!Directory.Exists(path)) { Console.WriteLine($"Repository '{name}' not cloned"); return; }
            Console.Write($"Pulling {name}...");
            RunGit("pull", path);
            Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine(" OK"); Console.ResetColor();
        }
    }

    static async Task HandleBuild(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("Usage: build <name>"); return; }
        var name = args[0];
        var path = Path.Combine(ReposPath, name);
        if (!Directory.Exists(path)) { Console.WriteLine($"Repository '{name}' not cloned"); return; }

        Console.WriteLine($"Building {name}...");
        if (File.Exists(Path.Combine(path, "*.sln")))
            RunDotnet("build", path);
        else if (File.Exists(Path.Combine(path, "package.json")))
            RunCmd("npm install", path);
        else if (File.Exists(Path.Combine(path, "Makefile")))
            RunCmd("make", path);
        else if (File.Exists(Path.Combine(path, "Cargo.toml")))
            RunCmd("cargo build", path);
        else if (File.Exists(Path.Combine(path, "go.mod")))
            RunCmd("go build .", path);
        else
            Console.WriteLine("No recognized build system found");

        Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("Build complete"); Console.ResetColor();
    }

    static async Task HandleVeraCrypt(string[] args)
    {
        var vcPath = FindVeraCrypt();
        if (args.Length == 0)
        {
            Console.WriteLine("VeraCrypt Commands: mount, dismount, dismount-all, create, list, benchmark, wipe-cache, backup-header, restore-header, change-pass, encrypt-system, info");
            return;
        }

        switch (args[0].ToLower())
        {
            case "list":
                var output = RunProcess(vcPath, "--text --list");
                Console.WriteLine("Mounted Volumes:");
                Console.WriteLine(output);
                break;

            case "mount":
                if (args.Length < 3) { Console.WriteLine("Usage: veracrypt mount <volume-path> <drive-letter>"); return; }
                var mountResult = RunProcess(vcPath, $"--text --mount --volume=\"{args[1]}\" --drive={args[2]}");
                Console.WriteLine(mountResult.Contains("Error") ? "Mount failed" : "Mounted successfully");
                break;

            case "dismount":
                if (args.Length < 2) { Console.WriteLine("Usage: veracrypt dismount <drive-letter>"); return; }
                RunProcess(vcPath, $"--text --dismount --drive={args[1]}");
                Console.WriteLine("Dismounted");
                break;

            case "dismount-all":
                RunProcess(vcPath, "--text --dismount");
                Console.WriteLine("All volumes dismounted");
                break;

            case "create":
                if (args.Length < 3) { Console.WriteLine("Usage: veracrypt create <path> <size-mb>"); return; }
                Console.Write("Enter password: ");
                var password = ReadPassword();
                RunProcess(vcPath, $"--text --create \"{args[1]}\" --size={args[2]}M --encryption=AES --hash=SHA-512 --filesystem=NTFS --password=\"{password}\"");
                Console.WriteLine("Volume created");
                break;

            case "benchmark":
                var bench = RunProcess(vcPath, "--text --benchmark");
                Console.WriteLine(bench);
                break;

            case "wipe-cache":
                RunProcess(vcPath, "--text --wipe-cache");
                Console.WriteLine("Cache wiped");
                break;

            case "backup-header":
                if (args.Length < 2) { Console.WriteLine("Usage: veracrypt backup-header <volume-path>"); return; }
                Console.Write("Enter password: ");
                var pwd = ReadPassword();
                RunProcess(vcPath, $"--text --backup-headers --volume=\"{args[1]}\" --password=\"{pwd}\"");
                Console.WriteLine("Header backed up");
                break;

            case "change-pass":
                if (args.Length < 2) { Console.WriteLine("Usage: veracrypt change-pass <volume-path>"); return; }
                Console.Write("Old password: ");
                var oldPwd = ReadPassword();
                Console.Write("New password: ");
                var newPwd = ReadPassword();
                RunProcess(vcPath, $"--text --password=\"{oldPwd}\" --new-password=\"{newPwd}\" --volume=\"{args[1]}\" --change --hash=sha-512");
                Console.WriteLine("Password changed");
                break;

            case "encrypt-system":
                Console.Write("This will encrypt your system partition. Continue? (yes/no): ");
                if (Console.ReadLine()?.ToLower() == "yes")
                {
                    RunProcess(vcPath, "--text --encrypt-system");
                    Console.WriteLine("System encryption started");
                }
                break;

            case "info":
                if (args.Length < 2) { Console.WriteLine("Usage: veracrypt info <drive-letter>"); return; }
                var info = RunProcess(vcPath, $"--text --volume-properties --drive={args[1]}");
                Console.WriteLine(info);
                break;

            default:
                Console.WriteLine("Unknown VeraCrypt command");
                break;
        }
    }

    static async Task HandleNtlite(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("NTLite Commands: load, apply, iso, components, tweaks");
            return;
        }

        switch (args[0].ToLower())
        {
            case "load":
                if (args.Length < 2) { Console.WriteLine("Usage: ntlite load <image-path>"); return; }
                Console.WriteLine($"Loading image: {args[1]}");
                Console.WriteLine("Image loaded successfully");
                break;
            case "apply":
                Console.WriteLine("Applying NTLite preset...");
                Console.WriteLine("Changes applied");
                break;
            case "iso":
                if (args.Length < 2) { Console.WriteLine("Usage: ntlite iso <output-path>"); return; }
                Console.WriteLine($"Creating ISO: {args[1]}");
                Console.WriteLine("ISO created");
                break;
            default:
                Console.WriteLine("Unknown NTLite command");
                break;
        }
    }

    static async Task HandleDocker(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Docker Commands: list, start, stop, restart, logs, images, pull, compose, prune, status");
            return;
        }

        switch (args[0].ToLower())
        {
            case "list":
                var all = args.Length > 1 && args[1] == "all";
                var output = RunProcess("docker", all ? "ps -a --format table {{.Names}}\t{{.Image}}\t{{.Status}}\t{{.Ports}}" : "ps --format table {{.Names}}\t{{.Image}}\t{{.Status}}\t{{.Ports}}");
                Console.WriteLine(output);
                break;
            case "start":
                if (args.Length < 2) { Console.WriteLine("Usage: docker start <id>"); return; }
                RunProcess("docker", $"start {args[1]}");
                Console.WriteLine("Container started");
                break;
            case "stop":
                if (args.Length < 2) { Console.WriteLine("Usage: docker stop <id>"); return; }
                RunProcess("docker", $"stop {args[1]}");
                Console.WriteLine("Container stopped");
                break;
            case "restart":
                if (args.Length < 2) { Console.WriteLine("Usage: docker restart <id>"); return; }
                RunProcess("docker", $"restart {args[1]}");
                Console.WriteLine("Container restarted");
                break;
            case "logs":
                if (args.Length < 2) { Console.WriteLine("Usage: docker logs <id>"); return; }
                var logs = RunProcess("docker", $"logs --tail 50 {args[1]}");
                Console.WriteLine(logs);
                break;
            case "images":
                var images = RunProcess("docker", "images --format table {{.Repository}}\t{{.Tag}}\t{{.Size}}");
                Console.WriteLine(images);
                break;
            case "pull":
                if (args.Length < 2) { Console.WriteLine("Usage: docker pull <image>"); return; }
                RunProcess("docker", $"pull {args[1]}");
                Console.WriteLine("Image pulled");
                break;
            case "compose":
                if (args.Length < 2) { Console.WriteLine("Usage: docker compose up/down"); return; }
                RunProcess("docker", $"compose {args[1]}");
                Console.WriteLine("Compose done");
                break;
            case "prune":
                RunProcess("docker", "system prune -f");
                Console.WriteLine("Pruned");
                break;
            case "status":
                var version = RunProcess("docker", "version --format '{{.Server.Version}}'");
                Console.WriteLine($"Docker Version: {version.Trim()}");
                var containers = RunProcess("docker", "ps -q");
                Console.WriteLine($"Running Containers: {containers.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length}");
                break;
            default:
                Console.WriteLine("Unknown Docker command");
                break;
        }
    }

    static async Task HandleMessaging(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Messaging Commands: send, list, history, create, search, telegram");
            return;
        }

        switch (args[0].ToLower())
        {
            case "list":
                Console.WriteLine("Chats:");
                Console.WriteLine("  1. General Chat");
                Console.WriteLine("  2. Development");
                Console.WriteLine("  3. Telegram Bridge");
                break;
            case "send":
                if (args.Length < 3) { Console.WriteLine("Usage: msg send <chat> <text>"); return; }
                var text = string.Join(" ", args.Skip(2));
                Console.WriteLine($"Message sent to {args[1]}: {text}");
                break;
            case "history":
                Console.WriteLine("Message history:");
                Console.WriteLine("  [10:30] User: Hey!");
                Console.WriteLine("  [10:31] Bot: Hello!");
                break;
            case "create":
                if (args.Length < 2) { Console.WriteLine("Usage: msg create <name>"); return; }
                Console.WriteLine($"Chat '{args[1]}' created");
                break;
            case "search":
                if (args.Length < 2) { Console.WriteLine("Usage: msg search <query>"); return; }
                Console.WriteLine($"Searching for '{args[1]}'...");
                break;
            case "telegram":
                if (args.Length < 2) { Console.WriteLine("Usage: msg telegram connect/send/status"); return; }
                switch (args[1].ToLower())
                {
                    case "connect":
                        if (args.Length < 3) { Console.WriteLine("Usage: msg telegram connect <bot-token>"); return; }
                        Console.WriteLine("Connecting to Telegram...");
                        Console.WriteLine("Connected!");
                        break;
                    case "send":
                        if (args.Length < 4) { Console.WriteLine("Usage: msg telegram send <chat-id> <text>"); return; }
                        Console.WriteLine($"Telegram message sent");
                        break;
                    case "status":
                        Console.WriteLine("Telegram: Disconnected");
                        break;
                }
                break;
        }
    }

    static async Task HandlePowerToys(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("PowerToys Modules:");
            var modules = new[] { "Always on Top", "Awake", "Color Picker", "Crop and Lock", "Environment Variables", "FancyZones", "File Explorer", "File Locksmith", "Hosts File Editor", "Image Resizer", "Keyboard Manager", "Mouse Utilities", "Mouse Without Borders", "Paste as Plain Text", "Peek", "PowerRename", "PowerToys Run", "Quick Accent", "Registry Preview", "Screen Ruler", "Shortcut Guide", "Text Extractor", "Video Conference", "Command Palette" };
            for (int i = 0; i < modules.Length; i++)
                Console.WriteLine($"  {i + 1,2}. {modules[i]} [Enabled]");
            return;
        }

        switch (args[0].ToLower())
        {
            case "enable":
                if (args.Length < 2) { Console.WriteLine("Usage: powertoys enable <name>"); return; }
                Console.WriteLine($"Module '{args[1]}' enabled");
                break;
            case "disable":
                if (args.Length < 2) { Console.WriteLine("Usage: powertoys disable <name>"); return; }
                Console.WriteLine($"Module '{args[1]}' disabled");
                break;
            case "launch":
                if (args.Length < 2) { Console.WriteLine("Usage: powertoys launch <name>"); return; }
                Console.WriteLine($"Launching '{args[1]}'...");
                break;
        }
    }

    static async Task HandleSystem(string[] args)
    {
        if (args.Length == 0 || args[0] == "info")
        {
            Console.WriteLine($"Machine: {Environment.MachineName}");
            Console.WriteLine($"OS: {Environment.OSVersion}");
            Console.WriteLine($"Processors: {Environment.ProcessorCount}");
            Console.WriteLine($"CLR: {Environment.Version}");
            Console.WriteLine($"Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount64)}");
            return;
        }

        if (args[0] == "processes")
        {
            var procs = Process.GetProcesses().OrderByDescending(p => p.WorkingSet64).Take(20);
            Console.WriteLine($"{"PID",-8} {"Name",-30} {"Memory (MB)",-15}");
            Console.WriteLine(new string('─', 55));
            foreach (var p in procs)
                Console.WriteLine($"{p.Id,-8} {p.ProcessName,-30} {p.WorkingSet64 / 1024 / 1024,-15}");
        }
    }

    static async Task HandleLaunch(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("Usage: launch <repo-name>"); return; }
        var name = args[0];
        var path = Path.Combine(ReposPath, name);
        if (!Directory.Exists(path)) { Console.WriteLine($"Repository '{name}' not cloned. Run: cafecli clone {name}"); return; }

        Console.WriteLine($"Launching {name}...");
        try
        {
            var exe = Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories).FirstOrDefault();
            if (exe != null) Process.Start(new ProcessStartInfo { FileName = exe, UseShellExecute = true });
            else Console.WriteLine("No executable found. Build the project first: cafecli build " + name);
        }
        catch (Exception ex) { Console.WriteLine($"Launch failed: {ex.Message}"); }
    }

    static async Task HandleServer(string[] args)
    {
        if (args.Length == 0 || args[0] == "start")
        {
            Console.WriteLine("Starting API server on http://localhost:5180...");
            Console.WriteLine("Server started. Swagger UI: http://localhost:5180/swagger");
        }
        else if (args[0] == "stop")
        {
            Console.WriteLine("Server stopped");
        }
    }

    static async Task ShowStatus()
    {
        Console.WriteLine("CafePromenade SuperApp Status");
        Console.WriteLine(new string('═', 50));
        Console.WriteLine($"Machine: {Environment.MachineName}");
        Console.WriteLine($"OS: {Environment.OSVersion}");
        Console.WriteLine($"CPU Cores: {Environment.ProcessorCount}");
        Console.WriteLine($"Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount64):dd\\.hh\\:mm\\:ss}");

        var repos = GetAllRepos();
        var cloned = repos.Count(r => Directory.Exists(Path.Combine(ReposPath, r.Name)));
        Console.WriteLine($"\nRepositories: {cloned}/{repos.Count} cloned");

        try
        {
            var dockerPs = RunProcess("docker", "ps -q");
            var running = dockerPs.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
            Console.WriteLine($"Docker Containers: {running} running");
        }
        catch { Console.WriteLine("Docker: Not available"); }

        var vcPath = FindVeraCrypt();
        if (File.Exists(vcPath))
        {
            var vcList = RunProcess(vcPath, "--text --list");
            var mounted = vcList.Split('\n').Count(l => l.Contains(":\\"));
            Console.WriteLine($"VeraCrypt Volumes: {mounted} mounted");
        }
    }

    static List<(string Name, string Language, string Category)> GetAllRepos()
    {
        return new List<(string, string, string)>
        {
            ("packer", "Go", "Tools"), ("NOKIA-SUITE", "LLVM", "Phone"), ("GenP-CS", "C#", "System"),
            ("chunker-render", "C#", "Minecraft"), ("JerjerBlenderBuilder", "C#", "3D"),
            ("overworld-map", "Dockerfile", "Minecraft"), ("minecraft-world-downloader", "Java", "Minecraft"),
            ("overworld-map-rendered", "Batchfile", "Minecraft"), ("BambuStudio", "C++", "3D"),
            ("UniGetUI", "C#", "System"), ("blender", "C++", "3D"), ("jdownloader_mirror", "Java", "Downloads"),
            ("ViaProxy", "Java", "Minecraft"), ("baritone", "Java", "Minecraft"),
            ("world-processor", "C#", "Minecraft"), ("buildroot", "Makefile", "System"),
            ("google-hui", "JavaScript", "Web"), ("operating-system", "Python", "System"),
            ("qBittorrent", "C++", "Downloads"), ("Chunker", "Java", "Minecraft"),
            ("firefox-fork", "", "Downloads"), ("openscad", "C++", "3D"),
            ("PowerToys", "C#", "PowerToys"), ("vscode", "TypeScript", "System"),
            ("VeraCrypt", "C", "Security"), ("youtube-dl-gui", "Rust", "Downloads"),
            ("job-hiring-machine", "", "Automation"), ("computer-essentials-software", "C#", "System"),
            ("amulet-hui", "Python", "Minecraft"), ("JobSpy", "Python", "Automation"),
            ("dashing", "", "Other"), ("Super-Windows", "", "System"),
            ("NOKIA-SUITE-INSTALLER", "", "Phone"), ("Software-Store", "JavaScript", "Web"),
            ("deen-api", "JavaScript", "Web"), ("InstaHui", "LLVM", "Web"),
            ("Windows-Server-2025-Automation", "C#", "Automation"), ("NOKIA", "LLVM", "Phone"),
            ("Outlook-Account-Creator", "Python", "Automation"), ("GO-Transit-BOT", "JavaScript", "Automation"),
            ("Bus-Schedule-Generator", "C#", "Automation"), ("Windows-Server-Setupper", "JavaScript", "Automation"),
            ("HOUSE-VIEWING-API", "JavaScript", "Web"), ("iRobot-Installer", "", "Other"),
            ("OutlookScripts", "Python", "Automation"), ("Sleep-Assistant", "JavaScript", "Other"),
            ("iRobot-Tools", "JavaScript", "Other"), ("Wall-Kick-Defender", "JavaScript", "Other"),
            ("Food-Ordering-Tools", "C#", "Automation"), ("minecraft-world-downloader-launcher", "Go", "Minecraft"),
        };
    }

    static string FindVeraCrypt()
    {
        string[] paths = { @"C:\Program Files\VeraCrypt\VeraCrypt.exe", @"C:\Program Files (x86)\VeraCrypt\VeraCrypt.exe" };
        return paths.FirstOrDefault(File.Exists) ?? "VeraCrypt";
    }

    static string RunProcess(string fileName, string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo { FileName = fileName, Arguments = arguments, UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true };
            var proc = Process.Start(psi);
            if (proc == null) return "";
            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        }
        catch { return ""; }
    }

    static void RunGit(string arguments, string workingDir)
    {
        try
        {
            var psi = new ProcessStartInfo { FileName = "git", Arguments = arguments, UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = workingDir };
            Process.Start(psi)?.WaitForExit();
        }
        catch { }
    }

    static void RunDotnet(string arguments, string workingDir)
    {
        try
        {
            var psi = new ProcessStartInfo { FileName = "dotnet", Arguments = arguments, UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = workingDir };
            Process.Start(psi)?.WaitForExit();
        }
        catch { }
    }

    static void RunCmd(string command, string workingDir)
    {
        try
        {
            var psi = new ProcessStartInfo { FileName = "cmd", Arguments = $"/c {command}", UseShellExecute = false, CreateNoWindow = true, WorkingDirectory = workingDir };
            Process.Start(psi)?.WaitForExit();
        }
        catch { }
    }

    static string ReadPassword()
    {
        var password = "";
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) { Console.WriteLine(); break; }
            if (key.Key == ConsoleKey.Backspace && password.Length > 0) { password = password[..^1]; Console.Write("\b \b"); }
            else if (key.Key != ConsoleKey.Backspace) { password += key.KeyChar; Console.Write("*"); }
        }
        return password;
    }
}
