# Compilation and Installation on Linux

This guide explains how to compile and install the SteamInput mod for KSP on Ubuntu 24.04.

## Prerequisites

- Ubuntu 24.04 (or compatible Linux distribution)
- .NET 6.0 (installed via Microsoft repository)
- Mono (mono-complete package)
- Node.js and npm
- Kerbal Space Program installed via Steam

## Installing Dependencies

```bash
# Install .NET 6.0
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-6.0

# Install Mono
sudo apt install -y mono-complete

# Install Node.js
sudo apt install -y nodejs
curl -L https://npmjs.org/install.sh | sudo sh
```

## Compilation

1. **Environment Setup**:
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
- **mcs** (Mono C# Compiler) to compile the C# plugin
- **Node.js** to generate VDF configuration files
- **zip** to create the plugin archive

## Troubleshooting

### KSPDIR not found
If `setup-env.sh` doesn't find KSP automatically:
```bash
export KSPDIR="/path/to/your/KSP"
```

### C# Compilation Error
Check that Mono is installed:
```bash
sudo apt install -y mono-complete
```

### VDF Generation Error
Check that Node.js and npm are installed:
```bash
node --version
npm --version
```

## Differences from Windows

- KSP data folder is called `KSP_Data` (not `KSP_x64_Data`)
- Uses `mcs` instead of MSBuild
- Linux paths for Steam (`~/.steam/steam/` or `~/.local/share/Steam/`)

## Generated Files

After compilation, the `Release/` folder contains:
- `SteamInput.zip` : Plugin ready to install
- `game_actions_220200*.vdf` : Steam Input configuration files
- `controller_*.vdf` : Specific controller configurations
- `README.md` : Mod documentation
