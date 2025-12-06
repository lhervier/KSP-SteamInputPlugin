# Steam Input Plugin for Kerbal Space Program

## Overview

This plugin enhances the native Steam Input support (through the support of the Steam Controller) in Kerbal Space Program by providing additional action sets and fixing the Maneuver Mode detection issue introduced in KSP 1.6. For more details about the original bug, see [KSP Issue #22165](https://bugs.kerbalspaceprogram.com/issues/22165).

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

It will also add a button in the action bar to help you troubleshoot problems by changing the logging level. Default is "Info". Set it to "Debug" or event "Trace" to have more details in the KSP.log file. 

When in flight mode, this icon is not displayed by default. Go to the difficulty settings to find a dedicated section with an option to display the icon.

If you want to see the logs in realtime, don't forget to tell KSP to flush its logs immediatly in the debug console (using the ALT+F12 shortcut), and check the "Flush logs" option in the Debug/Console section.

## System Requirements

- Windows 10 or later
- Steam client installed and running
- Kerbal Space Program 1.12 (Steam version)
- A Steam Input compatible controller, with back buttons:
  - Steam Controller
  - Horipad Steam Controller
  - Xbox One/Series controller with back buttons mapped to joystick click
  - PlayStation 4/5 controller with back buttons mapped to joystick click (Raiju Tournament Edition for example)
  - XBox Elite controller, set on the default profile so Steam can use the back buttons. Don't forget to install the additionnal tools via the Steam parameters.
  - Other controllers with Steam Input support

## Installation

### 1. Configure Steam Input Actions

1. Create a folder named `controller_config` in your Steam directory (e.g., `C:\Program Files (x86)\Steam\controller_config`)
   - Note: A folder named `controller_base` should already exist. Don't touch to this folder !
2. Copy the file `game_actions_220200_[your language].vdf` into this folder

### 2. Deploy Steam VDF Configuration

You have two options:
- Launch the game and create your own bindings through the Steam interface
- Use one of the pre-configured VDF files included with this mod:
  - `controller_steamcontroller_gordon_[your language].vdf`: For Steam Controller users
  - `controller_hori_steam_[your language].vdf`: For the Horipad Steam Controller
  - `controller_ps4_[your language].vdf`: For PS4/PS5 like controllers (see requirements)
  - `controller_xbox_[your language].vdf`: For Xbox One/Series controllers (see requirements)
  - `controller_xboxelite_[your language].vdf`: For Xbox Elite controllers (see requirements)

Custom vdf configurations must be placed in the folder:
```
${SteamDir}/steamapps/common/Steam Controller Configs/[your userid]/config/220200/
```

### 3. Install the Plugin

Extract the plugin files into the `GameData/SteamInput` folder of your KSP installation

## Usage

After installation, the plugin will automatically detect and activate the appropriate action sets based on your current game context. No additional configuration is required.

You will find an icon in the KSP Tool bar. Clicking on it will allow you to change the logging level (in the KSP.log file). By default, this icon is hidden when in game. You can show it in the game difficulty settings, in the "Steam Input" section.

### Troubleshooting

If you encounter issues:

1. Verify that Steam is running and KSP is launched through Steam
2. Ensure the VDF configuration files are in the correct location
3. Verify that the plugin is properly installed in the GameData folder
4. Check that your controller is properly connected and recognized by Steam. 
5. Check the KSP.log file for any error messages related to the plugin. Plugins logs are prefixed with the "[SteamInput]" string. You can change the logging level ingame.

Issues specific to the XBox Elite Controller :

1. Check that you have installed the XBox enhanced feature support :
  - Go to Steam Parameters, in the "Controllers" section
  - Click on the "install" button next to "XBox enhanced feature support"
1. Check that your controller firmware is up to date
2. Check that no profile is selected on the controller (the LED below the XBox button must be OFF !)

## Known issue

I provide a VDF file for each language, even if vdf files can be localized. You will also find a version with the localization keys inside. But, they will not work. Steam will always use the english version of the keys... Help is welcomed...

## Support

For issues, feature requests, or contributions, please visit the [GitHub repository](https://github.com/lhervier/SteamInputConfig).

## Building from source

TBD :

- Windows PC
- VSCode
- netcore
- dotnet Framework 4.7
- node

build.bat : Build all the release in the "Release" folder
install.bat : Deploy all the files

## License

This project is licensed under the MIT License - see the LICENSE file for details.
