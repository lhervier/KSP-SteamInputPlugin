# Steam Input Plugin for Kerbal Space Program

## Overview

This plugin enhances the native Steam Controller support in Kerbal Space Program by providing additional action sets and fixing the Maneuver Mode detection issue introduced in KSP 1.6. For more details about the original bug, see [KSP Issue #22165](https://bugs.kerbalspaceprogram.com/issues/22165).

## Features

The plugin adds several new action groups to enhance your gameplay experience:

- These action groups, implemented when using Squad native plugin, are also supported:
  - **Menu**
  - **Flight**
  - **Docking**
  - **Editor**
- The **Map** is split into three distinct action groups:
  - Flight mode controls
  - Docking mode controls
  - EVA mode controls
- Additional action groups:
  - **Tracking Station**: Dedicated controls for the tracking station interface
  - **Mission Builder**: Custom controls for the mission editor
  - **Construction Mode**: Special action group for EVA with engineers
  - **IVA**: Dedicated action group for IVA
  - **FreeIVA**: Dedicated action group for the [FreeIVA mod](https://github.com/FirstPersonKSP/FreeIva)

## System Requirements

- Windows 10 or later
- Steam client installed and running
- Kerbal Space Program 1.12 (latest version)
- A Steam Input compatible controller, with back buttons:
  - Steam Controller
  - Xbox One/Series controller with back buttons mapped to joystick click
  - PlayStation 4/5 controller with back buttons mapped to joystick click (Raiju Tournament Edition for example)
  - Other controllers with Steam Input support

## Installation

### 1. Configure Steam Controller Actions

1. Create a folder named `controller_config` in your Steam directory (e.g., `C:\Program Files (x86)\Steam\controller_config`)
   - Note: A folder named `controller_base` should already exist
2. Copy the file `game_actions_220200.vdf` into this folder

### 2. Deploy Steam VDF Configuration

You have two options:
- Launch the game and create your own bindings through the Steam interface
- Use one of the pre-configured VDF files included with this mod:
  - `controller_steamcontroller_gordon.vdf`: For Steam Controller users
  - `controller_ps4.vdf`: For PS4/PS5 like controllers (see requirements)
  - `controller_xbox.vdf`: For Xbox One/Series controllers (see requirements)

Custom vdf configurations must be placed in the folder:
```
${SteamDir}/steamapps/common/Steam Controller Configs/[your userid]/config/220200/
```

### 3. Install the Plugin

Extract the plugin files into the `GameData/SteamInput` folder of your KSP installation

## Usage

After installation, the plugin will automatically detect and activate the appropriate action sets based on your current game context. No additional configuration is required.

You will find an icon in the KSP Tool bar. Clicking on it will allow you to change the logging level (in the KSP.log file)

### Troubleshooting

If you encounter issues:

1. Verify that Steam is running and KSP is launched through Steam
2. Check that your controller is properly connected and recognized by Steam
3. Ensure the VDF configuration files are in the correct location
4. Verify that the plugin is properly installed in the GameData folder
5. Check the KSP.log file for any error messages related to the plugin. Plugins logs are prefixed with the "[SteamInput]" string.

## Support

For issues, feature requests, or contributions, please visit the [GitHub repository](https://github.com/lhervier/SteamInputConfig).

## License

This project is licensed under the MIT License - see the LICENSE file for details.
