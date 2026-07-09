@echo off
REM Thin wrapper: builds the mod (plugin) only, via the shared generic build in
REM KSP-Shared\tools. The controller-config side of the repo is built separately
REM (build-config.* at the repo root).
setlocal
cd /d "%~dp0"
set "MOD_CSPROJ=SteamInput.csproj"
call "KSP-Shared\tools\build.bat"
exit /b %errorlevel%
