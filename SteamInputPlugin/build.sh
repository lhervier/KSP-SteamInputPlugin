#!/usr/bin/env bash
# Thin wrapper: builds the mod (plugin) only, via the shared generic build in
# KSP-Shared/tools. The controller-config side of the repo is built separately
# (build-config.* at the repo root).
set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")"
export MOD_CSPROJ="SteamInput.csproj"
exec bash KSP-Shared/tools/build.sh
