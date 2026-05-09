# CafePromenade SuperApp

A unified management platform for all CafePromenade tools, repositories, and services. Runs as **WinUI3 Desktop**, **ASP.NET Website**, **CLI**, **TUI**, and **Docker**.

## Features

### Repository Management (50 repos)
- Clone, pull, build all cafepromenade repositories
- Category filtering: Minecraft, 3D, System, Downloads, Automation, Web, Phone, Security

### VeraCrypt - Full Disk Encryption
- Create, mount, dismount volumes
- Change password, backup/restore headers
- Encrypt/decrypt system partition
- Benchmark, wipe cache, create keyfiles
- All encryption algorithms: AES, Serpent, Twofish, Camellia, Kuznyechik
- All hash algorithms: SHA-512, SHA-256, BLAKE2s, Whirlpool, Streebog

### NTLite - Windows Image Customization
- Load ISO/WIM/ESD images
- Remove components (IE, Edge, Cortana, Xbox, Telemetry, etc.)
- Integrate updates and drivers
- Apply tweaks and unattended setup
- Create custom ISOs

### Messaging - Local Telegram-like Chat
- Local messaging server on localhost:5180
- Real-time chat with SignalR
- Telegram API bridge integration
- Groups, channels, private chats
- File sharing, voice messages

### Windows Tweaks (50+ Winaero-style tweaks)
- Privacy: Disable telemetry, advertising ID, activity history, location tracking
- System: Disable UAC, Windows Update, Defender, Cortana, Copilot
- Explorer: Show file extensions, hidden files, classic context menu, dark mode
- Network: Disable SMBv1, NetBIOS, LLMNR, flush DNS
- Services: Disable DiagTrack, SysMain, Print Spooler, Xbox services
- Context Menu: Add Command Prompt, PowerShell, Take Ownership
- One-click tweaks for all common customizations

### Credential Vault
- Encrypted credential storage with AES-256
- Master password protection with PBKDF2
- Save credentials for all services
- Default host: 192.168.50.1, default username: iRobot
- Export/import vault, credential rotation

### PowerToys (24 modules)
- Always on Top, Color Picker, FancyZones, PowerToys Run
- Image Resizer, Keyboard Manager, Text Extractor, Screen Ruler
- All 24 modules with enable/disable toggles

### Docker Management
- List, start, stop, restart containers
- View logs, pull images, compose up/down
- Prune unused resources

### 3D Printing & Modeling
- BambuStudio, OpenSCAD, Blender, JerjerBlenderBuilder

### Minecraft Suite (10 tools)
- World Downloader, Chunker, ViaProxy, Baritone
- Overworld Map, World Processor, Amulet

### Download Managers
- JDownloader, qBittorrent, YouTube DL GUI, Firefox Fork

### Automation & Scripts
- JobSpy, Outlook automation, GO Transit BOT
- Windows Server automation, Food Ordering Tools

## Quick Start

### WinUI3 Desktop
```bash
dotnet run --project src/CafePromenade.Desktop
```

### ASP.NET Website (with Swagger)
```bash
dotnet run --project src/CafePromenade.Web
# Open http://localhost:5180/swagger
```

### CLI
```bash
dotnet run --project src/CafePromenade.CLI -- help
dotnet run --project src/CafePromenade.CLI -- repos list
dotnet run --project src/CafePromenade.CLI -- veracrypt list
dotnet run --project src/CafePromenade.CLI -- clone all
```

### TUI (Terminal UI)
```bash
dotnet run --project src/CafePromenade.TUI
```

### Docker
```bash
docker compose up -d
# API available at http://localhost:5180
```

## API Endpoints

| Endpoint | Description |
|----------|-------------|
| `GET /api/repositories` | List all repos |
| `POST /api/repositories/{name}/clone` | Clone a repo |
| `GET /api/veracrypt/volumes` | Mounted volumes |
| `POST /api/veracrypt/create` | Create volume |
| `POST /api/veracrypt/mount` | Mount volume |
| `GET /api/ntlite/image` | Load Windows image |
| `GET /api/messaging/chats` | List chats |
| `POST /api/messaging/telegram/connect` | Connect Telegram |
| `GET /api/docker/containers` | List containers |
| `GET /api/system/status` | System status |
| `GET /api/system/powertoys` | PowerToys modules |
| `GET /api/windowstweaks` | All Windows tweaks |
| `POST /api/windowstweaks/{id}/apply` | Apply a tweak |
| `GET /api/credentials` | List credentials |
| `POST /api/credentials/unlock` | Unlock vault |

## Architecture

```
CafePromenade-SuperApp/
├── src/
│   ├── CafePromenade.Core/       # Models, interfaces, enums
│   ├── CafePromenade.Web/        # ASP.NET API + SignalR + services
│   ├── CafePromenade.Desktop/    # WinUI3 tabbed app (13 tabs)
│   ├── CafePromenade.CLI/        # Command-line interface
│   └── CafePromenade.TUI/        # Terminal UI
├── repos/                         # Cloned repos (gitignored)
├── Dockerfile                     # API container
├── Dockerfile.cli                 # CLI container
├── Dockerfile.tui                 # TUI container
└── docker-compose.yml             # Multi-container setup
```

## Default Configuration

- **Default Host**: 192.168.50.1
- **Default Username**: iRobot
- **API Port**: 5180
- **Credential Vault**: `%LOCALAPPDATA%\CafePromenade\Vault`
