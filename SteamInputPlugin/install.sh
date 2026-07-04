#!/usr/bin/env bash
# Thin wrapper: installs the mod (plugin) only, via the shared generic install in
# KSP-Shared/tools (backs up and restores PluginData). The controller-config side
# of the repo is installed separately (install-config.* at the repo root).
set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")"
export MOD_NAME="SteamInputMod"
exec bash KSP-Shared/tools/install.sh
