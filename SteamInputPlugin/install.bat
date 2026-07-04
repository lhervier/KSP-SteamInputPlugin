@echo off
REM Thin wrapper: installs the mod (plugin) only, via the shared generic install
REM in KSP-Shared\tools (backs up and restores PluginData). The controller-config
REM side of the repo is installed separately (install-config.* at the repo root).
setlocal
cd /d "%~dp0"
set "MOD_NAME=SteamInputMod"
call "KSP-Shared\tools\install.bat"
exit /b %errorlevel%
