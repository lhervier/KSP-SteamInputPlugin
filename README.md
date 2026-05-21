# Steam Input Plugin for Kerbal Space Program

## Overview

This plugin extends Kerbal Space Program’s native Steam Input support by adding extra action sets and fixing maneuver mode detection, which broke in KSP 1.6. For background on the original issue, see [KSP Issue #22165](https://bugs.kerbalspaceprogram.com/issues/22165).

It also restores joystick (HID game controller) support, which stopped working reliably in the Steam build because of Squad’s built-in Steam Controller plugin. This mod disables that plugin and ships a replacement.

## Features

The plugin adds several action groups:

- **Compatible with Squad’s built-in Steam Input plugin** for:
  - **Menu** — menus and UI
  - **Flight** — flight (including maneuver node mode)
  - **Docking** — docking
  - **Editor** — vehicle editor
- **Map** is split into three action groups:
  - **MapFlight** — map while using flight controls
  - **MapDocking** — map while using docking controls
  - **MapEVA** — map while in EVA
- **Additional action groups:**
  - **Tracking Station** — tracking station interface
  - **Mission Builder** — mission editor
  - **Construction Mode** — EVA construction with engineers
  - **IVA** — interior view
  - **FreeIVA** — [FreeIVA mod](https://github.com/FirstPersonKSP/FreeIva)

A toolbar button lets you change the plugin’s log level for troubleshooting. The default is **Info**; set **Debug** or **Trace** for more detail in `KSP.log`.

In flight, that button is hidden by default. Enable it under **Difficulty** → **Steam Input** (or the equivalent section for your game mode).

To view logs in near real time, open the debug console (**Alt+F12**), go to **Debug** → **Console**, and enable **Flush logs**

## System Requirements

- Linux or Windows 10 or later
- Steam client installed and running
- Kerbal Space Program 1.12 (Steam build)
- A Steam Input–compatible controller with **back buttons** (or equivalent), for the bundled layouts:
  - Steam Controller (v1)
  - Steam Controller (v2)
  - HORIPAD for Steam
  - Xbox Elite controller — use the default profile so Steam can use the paddles; install **Xbox enhanced feature support** from Steam (see **Settings** → **Controller**).
  - DualShock 4 / DualSense — only with extra rear inputs mapped (e.g. to stick clicks), or a model with back buttons; see **Troubleshooting** below.
  - Any other Steam Input–capable device — you can build your own layout in Steam’s configuration UI.

## Installation

### 1. Configure Steam Input actions

1. Create a folder named `controller_config` in your Steam directory (e.g. `C:\Program Files (x86)\Steam\controller_config`).
   - A `controller_base` folder may already exist there; **do not modify it**.
2. Copy `game_actions_220200_[your language].vdf` into `controller_config` and rename it to `game_actions_220200.vdf`.

### 2. Install the plugin

Extract the plugin into `GameData/SteamInput` inside your KSP install.

### 3. Deploy the Steam VDF layout (optional)

You can either:

- Launch the game and bind everything in Steam’s controller UI, or  
- Use one of the pre-built layouts shipped with the mod:
  - `ksp_steaminput_steamcontroller_[language].vdf` — Steam Controller v1
  - `ksp_steaminput_steamcontroller_v2_[language].vdf` — Steam Controller v2
  - `ksp_steaminput_hori_[language].vdf` — HORIPAD for Steam
  - `ksp_steaminput_ps4_[language].vdf` — PlayStation-style pads (see requirements above)
  - `ksp_steaminput_xboxelite_[language].vdf` — Xbox Elite (see requirements above)

Place custom layouts in:

```
${SteamDir}/steamapps/common/Steam Controller Configs/[your SteamID]/config/220200/
```

In-game, open the Steam overlay / controller settings. Under **Your configurations**, you should see the layout you added.

## Usage

After installation, the plugin selects the correct action sets from your game context. No extra setup is required.

Use the toolbar icon to change the log level (written to `KSP.log`). As above, the icon can be shown from difficulty / **Steam Input** settings.

### Troubleshooting

If something goes wrong:

1. Steam is running and KSP is started from Steam.
2. VDF files are in the paths described above.
3. The plugin files are under `GameData/SteamInput`.
4. The controller is connected and visible to Steam.
5. Check `KSP.log` for lines prefixed with `[SteamInput]`; increase the log level in-game if needed.

#### Xbox Elite Controller

1. Install **Xbox enhanced feature support**: **Steam** → **Settings** → **Controller** → install next to that option.
2. Update the controller firmware if prompted.
3. No Xbox Accessories “slot” profile should override Steam: the LED under the Xbox button should be **off** when using the default Steam profile.

#### PS4 / PS5–style controllers

1. Standard DualShock 4 / DualSense pads have no rear buttons; the bundled PS4-style layout expects extra inputs (e.g. stick clicks used as back buttons).
2. Map those extra inputs as described in **System Requirements**.
3. You can always author a custom Steam Input configuration for your hardware.

## Known issues

Separate VDF files are provided per language even though VDF can carry localization; getting Steam’s localization path to work reliably is still **TODO** — contributions welcome.

On Linux, non–US English keyboard layouts (e.g. non-QWERTY) may cause issues; see [this Bazzite report](https://github.com/ublue-os/bazzite/issues/3464).

## Support

Issues, feature requests, and contributions: [GitHub repository](https://github.com/lhervier/SteamInputConfig).

## Building from source

**TODO**

- Windows / Linux
- VS Code
- .NET / target framework (e.g. .NET Framework 4.7)
- Node.js (for VDF merge scripts)

- `build.bat` — build release outputs into the `Release` folder  
- `install.bat` — deploy built files

## License

This project is licensed under the MIT License — see the `LICENSE` file for details.
