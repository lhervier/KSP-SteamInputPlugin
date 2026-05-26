# Linux Guide

This document covers running, building, and installing the Steam Input plugin on Linux. For general features and controller requirements, see [README.md](README.md).

## Running on Linux

### Windows-first development

This plugin and its bundled layouts were built and tested mainly on **Windows**. Paths, scripts, and defaults assume a typical Windows Steam install (`KSP_x64_Data`, and so on).

On Linux, the **native** Steam build has been tested and works. Linux is supported, but expect less day-to-day testing than on Windows.

### Native vs Proton

| | Native Linux build | Proton (Windows build) |
|---|-------------------|------------------------|
| **Recommendation** | Preferred on Linux | Optional; not the primary target |
| **Plugin** | Verified working | Should work if Steam Input is set up the same way |
| **Keyboard layout** | See below — **same issue on both** | See below — **same issue on both** |

Use the native build when you can. Proton is a valid option if you need it for mods or compatibility, but do not expect the plugin to behave differently regarding keyboard layout between the two.

### Keyboard layout (native and Proton)

If your active keyboard layout is not **US English (QWERTY)**, Steam Input bindings often map to the wrong keys or stop working as intended. This affects **both** the native Linux build and the Proton build equally — it is not a Proton-only problem.

**Workaround:** switch to a US QWERTY layout (in the desktop environment, in Steam, or both) while playing KSP.

For more context, see the [Bazzite issue discussion](https://github.com/ublue-os/bazzite/issues/3464) and **Known issues** in [README.md](README.md).

### Modifier key (native Linux only)

On the native Linux build, KSP’s default modifier key is **Right Shift** instead of **Left Alt**. The layouts in this repository assume **Left Alt**. Edit `settings.cfg` in your KSP install, search for `RightShift`, and set the value to `LeftAlt`.

---

## Building and Installing

### Prerequisites

| Requirement | Notes |
|-------------|--------|
| Linux (e.g. Ubuntu 24.04+) | Other distributions should work if dependencies are available |
| [.NET SDK](https://dotnet.microsoft.com/download) 8 or later | Tested with .NET 10; used only to **compile** the plugin |
| Node.js and npm | To generate Steam Input VDF files |
| Kerbal Space Program 1.12 | Steam install; `KSPDIR` must point at the game root |

The plugin targets **.NET Framework 4.7.2** (same as KSP). You do **not** need Mono to build: `dotnet build` uses the NuGet package `Microsoft.NETFramework.ReferenceAssemblies.net472`. The resulting DLL still runs on KSP’s Mono/Unity runtime.

### Install dependencies

**.NET SDK**

Follow the instructions on the [.NET download page](https://dotnet.microsoft.com/download). On Ubuntu you can also install a distro package when available, for example:

```bash
sudo apt install -y dotnet-sdk-10.0
```

Verify:

```bash
dotnet --version
```

**Node.js**

```bash
sudo apt install -y nodejs npm
node --version
npm --version
```

### Build

1. **Set `KSPDIR`** (game install root). `setup-env.sh` searches common Steam paths and appends `KSPDIR` to `~/.bashrc`:

   ```bash
   ./setup-env.sh
   source ~/.bashrc
   ```

2. **Build everything** (plugin + VDF configs):

   ```bash
   ./build.sh
   ```

3. **Install** into KSP and Steam config locations:

   ```bash
   ./install.sh
   ```

### Build scripts

| Script | Purpose |
|--------|---------|
| `setup-env.sh` | Detect KSP and set `KSPDIR` |
| `build.sh` | Full build (plugin + configuration) |
| `build-plugin.sh` | C# plugin only |
| `build-config.sh` | VDF configuration files only |
| `install.sh` | Install plugin and Steam configs |
| `install-plugin.sh` | Plugin only |
| `install-config.sh` | Controller configuration only |
| `test-build.sh` | Quick check of tooling and VDF merge (no full plugin build) |

### What the build does

1. **`dotnet build`** — compiles the C# plugin against .NET Framework 4.7.2, referencing assemblies from `$KSPDIR/KSP_x64_Data/Managed` (or `KSP_Data/Managed` on older layouts).
2. **Node.js** — merges modular VDF sources into controller and game-action files.
3. **`zip`** — packages the plugin into `Release/SteamInput.zip`.

### Output in `Release/`

After a successful build:

| File | Description |
|------|-------------|
| `SteamInput.zip` | Plugin archive for `GameData/SteamInput` |
| `game_actions_220200_<language>.vdf` | Steam Input game actions |
| `ksp_steaminput_<controller>_<language>.vdf` | Per-controller layouts |
| `README.md` | Copy of the main readme |

---

## Troubleshooting

### `KSPDIR` not set

If `setup-env.sh` does not find your install:

```bash
export KSPDIR="/path/to/Kerbal Space Program"
```

The build scripts look for `Assembly-CSharp.dll` under:

- `$KSPDIR/KSP_x64_Data/Managed/` (typical Steam layout on Linux and Windows)
- `$KSPDIR/KSP_Data/Managed/` (legacy layout)

### C# build fails

Confirm the SDK and game path:

```bash
dotnet --version
echo "$KSPDIR"
ls "$KSPDIR/KSP_x64_Data/Managed/Assembly-CSharp.dll"
```

Build manually (adjust `KSP_DATA_DIR` if you use `KSP_Data`):

```bash
dotnet build SteamInputPlugin/SteamInput.csproj -p:KSP_DATA_DIR="$KSPDIR/KSP_x64_Data"
```

Or use:

```bash
./build-plugin.sh
```

### VDF generation fails

Ensure Node.js and npm are installed. `build-config.sh` (and `build.sh`) run `npm ci` in `MergeScripts/` automatically.

```bash
node --version
npm --version
cd MergeScripts && npm ci
```

---

## Linux vs Windows

| Topic | Linux | Windows |
|-------|-------|---------|
| Build command | `dotnet build` | `dotnet build` |
| KSP data folder | Usually `KSP_x64_Data` (same as Windows Steam) | `KSP_x64_Data` |
| Steam paths | `~/.steam/steam/` or `~/.local/share/Steam/` | `C:\Program Files (x86)\Steam\` |
| Plugin archive | `zip` in `build-plugin.sh` | PowerShell `Compress-Archive` |

For installation steps shared across platforms, see [README.md](README.md).
