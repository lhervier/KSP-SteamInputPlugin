@echo off
setlocal

set "STEAMDIR=C:\Program Files (x86)\Steam"
set USERID="27319809"
set APPID="220200"

if "%KSPDIR%"=="" (
    echo ERROR: KSPDIR environment variable is missing
    exit /b 1
)
echo KSPDIR: %KSPDIR%

echo.
echo ===========================================
echo Removing existing plugin folder
echo ===========================================

echo Removing Release folder
if exist "%KSPDIR%\GameData\SteamInput" rmdir /s /q "%KSPDIR%\GameData\SteamInput"

echo.
echo ===========================================
echo Unzipping Plugin
echo ===========================================

echo Unzipping zip archive
powershell -Command "Expand-Archive -Path 'Release\SteamInput.zip' -DestinationPath '%KSPDIR%\GameData\SteamInput' -Force"

echo.
echo ===========================================
echo Installing VDF files
echo ===========================================

set "CONTROLLER_ACTION_DIR=%STEAMDIR%\controller_config"
set "CONTROLLER_CONFIG_DIR=%STEAMDIR%\steamapps\common\Steam Controller Configs\%USERID%\config\%APPID%"

echo checking that folders exists
if not exist "%CONTROLLER_ACTION_DIR%"\ mkdir "%CONTROLLER_ACTION_DIR%"\
if not exist "%CONTROLLER_CONFIG_DIR%"\ mkdir "%CONTROLLER_CONFIG_DIR%"\

echo Copying action file
copy /y "Release\game_actions_%APPID%.vdf" "%CONTROLLER_ACTION_DIR%\game_actions_%APPID%.vdf"

echo Copying Controllers VDF
echo - Steam Controller
copy /y "Release\controller_steamcontroller_gordon.vdf" "%CONTROLLER_CONFIG_DIR%\controller_steamcontroller_gordon.vdf"
echo - PS4
copy /y "Release\controller_ps4.vdf" "%CONTROLLER_CONFIG_DIR%\controller_ps4.vdf"
echo - Hori Steam
copy /y "Release\controller_hori_steam.vdf" "%CONTROLLER_CONFIG_DIR%\controller_hori_steam.vdf"
