@echo off
setlocal

set APPID=220200

if "%KSPLANG%"=="" (
    echo WARN: KSPLANG environment variable is missing. Default is french.
    set "KSPLANG=french"
)

if "%STEAMDIR%"=="" (
    echo WARN: STEAMDIR environment variable is missing. Using default.
    set "STEAMDIR=C:\Program Files (x86)\Steam"
)

if "%USERID%"=="" (
    echo WARN: USERID environment variable is missing. Using mine :P.
    set "USERID=27319809"
)

echo Script parameters :
echo - KSPLANG: %KSPLANG%
echo - STEAMDIR: %STEAMDIR%
echo - USERID: %USERID%

echo.
echo -------------------------------------------
echo Installing VDF files
echo -------------------------------------------

set "CONTROLLER_ACTION_DIR=%STEAMDIR%\controller_config"
set "CONTROLLER_CONFIG_DIR=%STEAMDIR%\steamapps\common\Steam Controller Configs\%USERID%\config\%APPID%"

echo Checking that folders exists
if not exist "%CONTROLLER_ACTION_DIR%" mkdir "%CONTROLLER_ACTION_DIR%"
if not exist "%CONTROLLER_CONFIG_DIR%" mkdir "%CONTROLLER_CONFIG_DIR%"

echo Copying action file
copy /y "Release\game_actions_%APPID%_%KSPLANG%.vdf" "%CONTROLLER_ACTION_DIR%\game_actions_%APPID%.vdf"
if errorlevel 1 (
    echo ERROR: Failed to copy action file
    exit /b 1
)

echo Copying Controllers VDF
echo - Steam Controller
copy /y "Release\ksp_steaminput_steamcontroller_%KSPLANG%.vdf" "%CONTROLLER_CONFIG_DIR%\controller_steamcontroller_gordon.vdf"
if errorlevel 1 (
    echo ERROR: Failed to copy Steam Controller config
    exit /b 1
)
echo - Hori Steam
copy /y "Release\ksp_steaminput_hori_steam_%KSPLANG%.vdf" "%CONTROLLER_CONFIG_DIR%\controller_hori_steam.vdf"
if errorlevel 1 (
    echo ERROR: Failed to copy Hori Steam config
    exit /b 1
)
echo - Xbox Elite
copy /y "Release\ksp_steaminput_xboxelite_%KSPLANG%.vdf" "%CONTROLLER_CONFIG_DIR%\controller_xboxelite.vdf"
if errorlevel 1 (
    echo ERROR: Failed to copy Xbox Elite config
    exit /b 1
)
echo Config Installation completed successfully
