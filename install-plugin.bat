@echo off
setlocal EnableDelayedExpansion

set APPID=220200

if not defined KSPDIR (
    echo WARN: KSPDIR environment variable is missing. Using default.
    set "KSPDIR=C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"
)

set "PLUGIN_DIR=!KSPDIR!\GameData\SteamInput"
set "PLUGIN_DATA=!PLUGIN_DIR!\PluginData"
set "BACKUP=!TEMP!\KSP-SteamInput-PluginData-backup"

echo Script parameters :
echo - KSPDIR: !KSPDIR!

echo.
echo -------------------------------------------
echo Backing up existing PluginData (config)
echo -------------------------------------------

if exist "!BACKUP!" rmdir /s /q "!BACKUP!"
if exist "!PLUGIN_DATA!" (
    echo Saving !PLUGIN_DATA!
    mkdir "!BACKUP!" 2>nul
    xcopy /E /I /Y "!PLUGIN_DATA!" "!BACKUP!\" >nul
    if errorlevel 1 (
        echo ERROR: Failed to backup PluginData
        exit /b 1
    )
) else (
    echo No existing PluginData folder to backup
)

echo.
echo -------------------------------------------
echo Removing existing plugin folder
echo -------------------------------------------

echo Removing !PLUGIN_DIR!
if exist "!PLUGIN_DIR!" rmdir /s /q "!PLUGIN_DIR!"

echo.
echo -------------------------------------------
echo Unzipping Plugin
echo -------------------------------------------

if not exist "!KSPDIR!\GameData" mkdir "!KSPDIR!\GameData"
if not exist "!PLUGIN_DIR!" mkdir "!PLUGIN_DIR!"

echo Unzipping zip archive
powershell -NoProfile -ExecutionPolicy Bypass -Command "Expand-Archive -LiteralPath '%~dp0Release\SteamInput.zip' -DestinationPath '!PLUGIN_DIR!' -Force"
if errorlevel 1 (
    echo ERROR: Failed to unzip the plugin
    exit /b 1
)

echo.
echo -------------------------------------------
echo Restoring PluginData
echo -------------------------------------------

if exist "!BACKUP!" (
    echo Restoring config to !PLUGIN_DATA!
    if not exist "!PLUGIN_DATA!" mkdir "!PLUGIN_DATA!"
    xcopy /E /I /Y "!BACKUP!\*" "!PLUGIN_DATA!\" >nul
    if errorlevel 1 (
        echo ERROR: Failed to restore PluginData
        exit /b 1
    )
    rmdir /s /q "!BACKUP!"
) else (
    echo No PluginData backup to restore
)

echo Plugin Installation completed successfully
