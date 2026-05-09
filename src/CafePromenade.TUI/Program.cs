using System.Diagnostics;

namespace CafePromenade.TUI;

class Program
{
    static int _selectedTab = 0;
    static readonly string[] _tabs = { "Dashboard", "Repositories", "VeraCrypt", "NTLite", "Messaging", "PowerToys", "Docker", "System" };
    static readonly string ReposPath = Path.Combine(AppContext.BaseDirectory, "repos");
    static bool _running = true;

    static async Task Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.Clear();

        while (_running)
        {
            DrawUI();
            var key = Console.ReadKey(true);
            HandleInput(key);
        }

        Console.CursorVisible = true;
        Console.Clear();
        Console.WriteLine("CafePromenade TUI exited.");
    }

    static void DrawUI()
    {
        Console.SetCursorPosition(0, 0);

        // Title bar
        Console.BackgroundColor = ConsoleColor.DarkCyan;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(CenterText(" CafePromenade SuperApp TUI ", Console.WindowWidth));
        Console.ResetColor();
        Console.WriteLine();

        // Tabs
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < _tabs.Length; i++)
        {
            if (i == _selectedTab)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.Write($" {_tabs[i]} ");
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine(new string('─', Console.WindowWidth));

        // Content
        switch (_selectedTab)
        {
            case 0: DrawDashboard(); break;
            case 1: DrawRepositories(); break;
            case 2: DrawVeraCrypt(); break;
            case 3: DrawNtlite(); break;
            case 4: DrawMessaging(); break;
            case 5: DrawPowerToys(); break;
            case 6: DrawDocker(); break;
            case 7: DrawSystem(); break;
        }

        // Footer
        Console.SetCursorPosition(0, Console.WindowHeight - 2);
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" ← → Switch Tabs | Q Quit | Enter Select | R Refresh ");
        Console.ResetColor();
    }

    static void DrawDashboard()
    {
        var repos = GetAllRepos();
        var cloned = repos.Count(r => Directory.Exists(Path.Combine(ReposPath, r.Name)));

        Console.WriteLine();
        WriteColor("  Dashboard", ConsoleColor.Cyan, true);
        Console.WriteLine();
        WriteColor($"  Repositories:  {cloned}/{repos.Count} cloned", ConsoleColor.Green);
        WriteColor($"  VeraCrypt:     Checking...", ConsoleColor.Yellow);
        WriteColor($"  Docker:        Checking...", ConsoleColor.Yellow);
        WriteColor($"  Messaging:     Ready", ConsoleColor.Green);
        Console.WriteLine();
        WriteColor("  Quick Actions:", ConsoleColor.White, true);
        Console.WriteLine();
        Console.WriteLine("  [1] Clone All Repositories");
        Console.WriteLine("  [2] Mount VeraCrypt Volume");
        Console.WriteLine("  [3] Start Docker Containers");
        Console.WriteLine("  [4] Open Messaging");
        Console.WriteLine("  [5] System Status");
        Console.WriteLine();
        WriteColor($"  System: {Environment.MachineName} | OS: {Environment.OSVersion} | CPUs: {Environment.ProcessorCount}", ConsoleColor.Gray);
        WriteColor($"  Uptime: {TimeSpan.FromMilliseconds(Environment.TickCount64):dd\\.hh\\:mm\\:ss}", ConsoleColor.Gray);
    }

    static int _repoScroll = 0;
    static void DrawRepositories()
    {
        var repos = GetAllRepos();
        Console.WriteLine();
        WriteColor("  Repositories (↑↓ to scroll, C clone, P pull, Enter open)", ConsoleColor.Cyan, true);
        Console.WriteLine();
        Console.WriteLine($"  {"Name",-35} {"Language",-12} {"Category",-15} {"Status",-10}");
        Console.WriteLine($"  {new string('─', 72)}");

        var visible = Math.Min(repos.Count - _repoScroll, Console.WindowHeight - 10);
        for (int i = _repoScroll; i < _repoScroll + visible && i < repos.Count; i++)
        {
            var repo = repos[i];
            var status = Directory.Exists(Path.Combine(ReposPath, repo.Name)) ? "Cloned" : "Remote";
            var color = status == "Cloned" ? ConsoleColor.Green : ConsoleColor.DarkGray;
            WriteColor($"  {repo.Name,-35} {repo.Language,-12} {repo.Category,-15} {status,-10}", color);
        }
        Console.WriteLine();
        WriteColor($"  Showing {_repoScroll + 1}-{_repoScroll + visible} of {repos.Count}", ConsoleColor.Gray);
    }

    static void DrawVeraCrypt()
    {
        Console.WriteLine();
        WriteColor("  VeraCrypt - Full Disk Encryption Manager", ConsoleColor.Cyan, true);
        Console.WriteLine();
        Console.WriteLine("  [M] Mount Volume");
        Console.WriteLine("  [D] Dismount Volume");
        Console.WriteLine("  [A] Dismount All");
        Console.WriteLine("  [C] Create Volume");
        Console.WriteLine("  [B] Backup Header");
        Console.WriteLine("  [R] Restore Header");
        Console.WriteLine("  [P] Change Password");
        Console.WriteLine("  [E] Encrypt System");
        Console.WriteLine("  [W] Wipe Cache");
        Console.WriteLine("  [K] Create Keyfile");
        Console.WriteLine("  [I] Volume Info");
        Console.WriteLine("  [L] List Mounted");
        Console.WriteLine("  [T] Benchmark");
        Console.WriteLine();
        Console.WriteLine("  Options:");
        Console.WriteLine("    Encryption: AES, Serpent, Twofish, AES-Twofish, AES-Twofish-Serpent");
        Console.WriteLine("    Hash: SHA-512, SHA-256, BLAKE2s, Whirlpool, Streebog");
        Console.WriteLine("    Filesystem: NTFS, FAT, exFAT, ext2, ext3, ext4, Btrfs");
        Console.WriteLine("    Volume Type: Standard, Hidden");
        Console.WriteLine();
        Console.WriteLine("  Mount Options:");
        Console.WriteLine("    -r  Read-only");
        Console.WriteLine("    -m  Removable");
        Console.WriteLine("    --pim=<value>  PIM value");
        Console.WriteLine("    --keyfile=<path>  Keyfile path");
    }

    static void DrawNtlite()
    {
        Console.WriteLine();
        WriteColor("  NTLite - Windows Image Customization", ConsoleColor.Cyan, true);
        Console.WriteLine();
        Console.WriteLine("  [L] Load Image (ISO/WIM/ESD)");
        Console.WriteLine("  [A] Apply Changes");
        Console.WriteLine("  [I] Create ISO");
        Console.WriteLine("  [C] Remove Components");
        Console.WriteLine("  [U] Integrate Updates");
        Console.WriteLine("  [D] Integrate Drivers");
        Console.WriteLine("  [F] Enable/Disable Features");
        Console.WriteLine("  [T] Apply Tweaks");
        Console.WriteLine("  [S] Setup Unattended Install");
        Console.WriteLine("  [P] Manage Presets");
        Console.WriteLine();
        WriteColor("  Removable Components:", ConsoleColor.Yellow, true);
        Console.WriteLine("    Internet Explorer, Edge, Cortana, Windows Apps, Telemetry,");
        Console.WriteLine("    Advertising ID, Error Reporting, Xbox, Maps, News, Weather,");
        Console.WriteLine("    Mixed Reality, 3D Viewer, Feedback Hub, Print Drivers");
    }

    static void DrawMessaging()
    {
        Console.WriteLine();
        WriteColor("  Messaging - Local Telegram-like Chat", ConsoleColor.Cyan, true);
        Console.WriteLine();
        Console.WriteLine("  [N] New Chat");
        Console.WriteLine("  [G] New Group");
        Console.WriteLine("  [C] New Channel");
        Console.WriteLine("  [T] Connect Telegram Bot");
        Console.WriteLine("  [S] Start Local Server (localhost:5180)");
        Console.WriteLine();
        WriteColor("  Chats:", ConsoleColor.White, true);
        Console.WriteLine("    ● General Chat - Welcome to the local messaging system!");
        Console.WriteLine("    ● Telegram Bridge - Connect your Telegram account");
        Console.WriteLine("    ● Development - Building the SuperApp...");
        Console.WriteLine("    ● Docker Alerts - All containers running");
        Console.WriteLine();
        WriteColor("  Server Status: Ready to start on http://localhost:5180", ConsoleColor.Green);
        WriteColor("  Telegram: Not connected", ConsoleColor.DarkGray);
        WriteColor("  Features: Real-time messaging, file sharing, voice messages, groups, channels", ConsoleColor.Gray);
    }

    static void DrawPowerToys()
    {
        Console.WriteLine();
        WriteColor("  PowerToys - All Modules", ConsoleColor.Cyan, true);
        Console.WriteLine();
        var modules = new[]
        {
            ("Always on Top", "Win+Ctrl+T", true), ("Awake", "", true), ("Color Picker", "Win+Shift+C", true),
            ("Crop and Lock", "", true), ("Environment Variables", "", true), ("FancyZones", "Win+Shift+`", true),
            ("File Explorer", "", true), ("File Locksmith", "", true), ("Hosts File Editor", "", true),
            ("Image Resizer", "", true), ("Keyboard Manager", "Win+Ctrl+K", true), ("Mouse Utilities", "Left Ctrl x2", true),
            ("Mouse Without Borders", "", true), ("Paste as Plain Text", "Win+Ctrl+V", true), ("Peek", "Ctrl+Space", true),
            ("PowerRename", "", true), ("PowerToys Run", "Alt+Space", true), ("Quick Accent", "", true),
            ("Registry Preview", "", true), ("Screen Ruler", "Win+Shift+M", true), ("Shortcut Guide", "Win+Shift+/", true),
            ("Text Extractor", "Win+Shift+T", true), ("Video Conference", "Win+Shift+Q", true), ("Command Palette", "", true),
        };

        Console.WriteLine($"  {"Module",-30} {"Shortcut",-20} {"Status",-10}");
        Console.WriteLine($"  {new string('─', 60)}");
        foreach (var (name, shortcut, enabled) in modules)
        {
            var status = enabled ? "Enabled" : "Disabled";
            var color = enabled ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write($"  {name,-30} {shortcut,-20} ");
            WriteColor(status, color);
        }
        Console.WriteLine();
        Console.WriteLine("  [E] Enable module  [D] Disable module  [L] Launch module");
    }

    static void DrawDocker()
    {
        Console.WriteLine();
        WriteColor("  Docker Container Manager", ConsoleColor.Cyan, true);
        Console.WriteLine();
        Console.WriteLine("  [L] List Containers");
        Console.WriteLine("  [S] Start Container");
        Console.WriteLine("  [T] Stop Container");
        Console.WriteLine("  [R] Restart Container");
        Console.WriteLine("  [O] View Logs");
        Console.WriteLine("  [I] List Images");
        Console.WriteLine("  [P] Pull Image");
        Console.WriteLine("  [U] Compose Up");
        Console.WriteLine("  [D] Compose Down");
        Console.WriteLine("  [X] Prune Unused");
        Console.WriteLine("  [V] Docker Version");
        Console.WriteLine();
        WriteColor("  Quick Actions:", ConsoleColor.White, true);
        Console.WriteLine("    [1] Start All Containers");
        Console.WriteLine("    [2] Stop All Containers");
        Console.WriteLine("    [3] Restart All Containers");
    }

    static void DrawSystem()
    {
        Console.WriteLine();
        WriteColor("  System Information", ConsoleColor.Cyan, true);
        Console.WriteLine();
        WriteColor($"  Machine:     {Environment.MachineName}", ConsoleColor.White);
        WriteColor($"  OS:          {Environment.OSVersion}", ConsoleColor.White);
        WriteColor($"  Processors:  {Environment.ProcessorCount}", ConsoleColor.White);
        WriteColor($"  CLR:         {Environment.Version}", ConsoleColor.White);
        WriteColor($"  Uptime:      {TimeSpan.FromMilliseconds(Environment.TickCount64):dd\\.hh\\:mm\\:ss}", ConsoleColor.White);
        WriteColor($"  Working Set: {Environment.WorkingSet / 1024 / 1024} MB", ConsoleColor.White);
        Console.WriteLine();
        WriteColor("  Running Processes (Top 15 by Memory):", ConsoleColor.White, true);
        Console.WriteLine();
        Console.WriteLine($"  {"PID",-8} {"Name",-30} {"Memory (MB)",-15}");
        Console.WriteLine($"  {new string('─', 53)}");
        try
        {
            var procs = Process.GetProcesses().OrderByDescending(p => p.WorkingSet64).Take(15);
            foreach (var p in procs)
            {
                try { Console.WriteLine($"  {p.Id,-8} {p.ProcessName,-30} {p.WorkingSet64 / 1024 / 1024,-15}"); }
                catch { }
            }
        }
        catch { }
    }

    static void HandleInput(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow:
                _selectedTab = Math.Max(0, _selectedTab - 1);
                break;
            case ConsoleKey.RightArrow:
                _selectedTab = Math.Min(_tabs.Length - 1, _selectedTab + 1);
                break;
            case ConsoleKey.Q:
                _running = false;
                break;
            case ConsoleKey.R:
                Console.Clear();
                break;
            case ConsoleKey.UpArrow:
                if (_selectedTab == 1) _repoScroll = Math.Max(0, _repoScroll - 1);
                break;
            case ConsoleKey.DownArrow:
                if (_selectedTab == 1) _repoScroll = Math.Min(50 - 1, _repoScroll + 1);
                break;
        }

        if (_selectedTab == 1)
        {
            switch (char.ToUpper(key.KeyChar))
            {
                case 'C':
                    Console.Write("  Enter repo name to clone: ");
                    Console.CursorVisible = true;
                    var name = Console.ReadLine();
                    Console.CursorVisible = false;
                    if (!string.IsNullOrEmpty(name))
                    {
                        RunProcess("gh", $"repo clone cafepromenade/{name} \"{Path.Combine(ReposPath, name)}\"");
                    }
                    Console.Clear();
                    break;
            }
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

    static void WriteColor(string text, ConsoleColor color, bool bold = false)
    {
        Console.ForegroundColor = color;
        if (bold) Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    static string CenterText(string text, int width)
    {
        var padding = (width - text.Length) / 2;
        return text.PadLeft(padding + text.Length).PadRight(width);
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
}
