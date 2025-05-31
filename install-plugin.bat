@echo off
setlocal

set APPID=220200

if "%KSPDIR%"=="" (
    echo WARN: KSPDIR environment variable is missing. Using default.
    set "KSPDIR=C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"
)

echo Script parameters :
echo - KSPDIR: %KSPDIR%

echo.
echo -------------------------------------------
echo Removing existing plugin folder
echo -------------------------------------------

echo Removing Release folder
if exist "%KSPDIR%\GameData\SteamInput" rmdir /s /q "%KSPDIR%\GameData\SteamInput"

echo.
echo -------------------------------------------
echo Unzipping Plugin
echo -------------------------------------------

echo Unzipping zip archive
powershell -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -Path 'Release\SteamInput.zip' -DestinationPath '%KSPDIR%\GameData\SteamInput' -Force"
if errorlevel 1 (
    echo ERROR: Failed to unzip the plugin
    exit /b 1
)

echo Plugin Installation completed successfully
