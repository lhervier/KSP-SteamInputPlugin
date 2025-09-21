# Using on Linux

This plugin is working with the native version. As of today, using proton will break key bindings with SteamInput if you don't have a US QWERTY Keyboard...

Also beware that by default, with the native Linux version of KSP, the modifier key is Right Shift instead of Left Alt. Edit the settings.cfg file (search for "RightShift"), and change the value (Set it to "LeftAlt").

# Compilation and Installation on Linux

This guide explains how to compile and install the SteamInput mod for KSP on Ubuntu 24.04.

## Prerequisites

- Ubuntu 24.04 (or compatible Linux distribution)
- .NET 6.0.425 (installed manually via Microsoft script)
- Mono 6.12 (mono-devel package)
- Node.js and npm
- Kerbal Space Program installed via Steam

## Tested Versions

- **Ubuntu**: 24.04.3 LTS (Noble Numbat)
- **.NET**: 6.0.425
- **Mono**: 6.12
- **Node.js**: Version from Ubuntu repositories
- **KSP**: Via Steam (KSP_x64_Data folder structure)

## Installing Dependencies

### .NET 6.0 Installation
**Note**: .NET 6.0 n'est plus disponible dans les dépôts officiels Ubuntu 24.04. Utilisez l'installation manuelle :

```bash
# Installer .NET 6.0 manuellement
curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --version 6.0.425

# Ajouter .NET au PATH (permanent)
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
source ~/.bashrc

# Vérifier l'installation
dotnet --version
```

### Mono Installation

You must install mono 6.12.X to support .net framework 4.7, but as of today, Ubuntu only support version 6.8.

You must go to the official website to install the reauired version.

```bash
# Vérifier l'installation
mono --version
```

### Node.js Installation
```bash
# Installer Node.js
sudo apt install -y nodejs npm

# Vérifier l'installation
node --version
npm --version
```

## Compilation

1. **Environment Setup**:

Will auto detect KSP installation path, and zill set a KSPDIR environment variable.

   ```bash
   ./setup-env.sh
   source ~/.bashrc
   ```

2. **Full Compilation**:
   ```bash
   ./build.sh
   ```

3. **Installation**:
   ```bash
   ./install.sh
   ```

## Available Scripts

- `setup-env.sh` : Automatically configures the KSPDIR variable
- `build.sh` : Compiles the complete mod (plugin + configuration)
- `build-plugin.sh` : Compiles only the C# plugin
- `build-config.sh` : Generates only the VDF configuration files
- `install.sh` : Installs the mod in KSP and configures Steam
- `install-plugin.sh` : Installs only the plugin
- `install-config.sh` : Installs only the controller configuration

## Compilation Structure

The compilation process uses:
- **msbuild** (Mono MSBuild) to compile the C# plugin (.NET Framework 4.7.2)
- **Node.js** to generate VDF configuration files
- **zip** to create the plugin archive

**Note**: Ce projet utilise .NET Framework 4.7.2, pas .NET Core. C'est pourquoi Mono est nécessaire pour la compilation sur Linux.

## Troubleshooting

### KSPDIR not found
If `setup-env.sh` doesn't find KSP automatically:
```bash
export KSPDIR="/path/to/your/KSP"
```

### C# Compilation Error
Check that Mono is installed and use msbuild:
```bash
# Installer Mono si nécessaire
sudo apt install -y mono-complete

# Compiler avec msbuild
msbuild SteamInput.csproj

# Ou utiliser le script de build
./build-plugin.sh
```

### VDF Generation Error
Check that Node.js and npm are installed:
```bash
node --version
npm --version
```

## Differences from Windows

- KSP data folder is called `KSP_x64_Data` (same as Windows on Ubuntu)
- Uses `msbuild` (Mono MSBuild)
- Linux paths for Steam (`~/.steam/steam/` or `~/.local/share/Steam/`)
- .NET 6.0 doit être installé manuellement (pas via les dépôts Ubuntu)
- Le projet utilise .NET Framework 4.7.2, nécessitant Mono pour la compilation

## Generated Files

After compilation, the `Release/` folder contains:
- `SteamInput.zip` : Plugin ready to install
- `game_actions_220200*.vdf` : Steam Input configuration files
- `controller_*.vdf` : Specific controller configurations
- `README.md` : Mod documentation
