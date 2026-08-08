@echo off
setlocal enabledelayedexpansion

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
call :install_controller "Steam Controller" "ksp_steaminput_steamcontroller_%KSPLANG%"
if errorlevel 1 exit /b 1
call :install_controller "Steam Controller V2" "ksp_steaminput_steamcontroller_v2_%KSPLANG%"
if errorlevel 1 exit /b 1
call :install_controller "Hori Steam" "ksp_steaminput_hori_steam_%KSPLANG%"
if errorlevel 1 exit /b 1
call :install_controller "Xbox Elite" "ksp_steaminput_xboxelite_%KSPLANG%"
if errorlevel 1 exit /b 1
call :install_controller "PS4/PS5" "ksp_steaminput_ps4_%KSPLANG%"
if errorlevel 1 exit /b 1

echo Config Installation completed successfully
exit /b 0

rem Installs Release\<base>.vdf as a new version of the <base> config, or fails (errorlevel 1).
rem %1 = label to display, %2 = config base name (no extension, no version suffix)
:install_controller
setlocal enabledelayedexpansion
set "LABEL=%~1"
set "BASE=%~2"
echo - !LABEL!

set "SRC=Release\!BASE!.vdf"
if not exist "!SRC!" (
    echo   ERROR: Missing source file "!SRC!". Run build-config.bat first.
    exit /b 1
)

rem Steam names every exported config "<base>_<N>.vdf" and increments N on each export,
rem and the mod loads the highest N. So scan for the highest existing N (numbering may
rem have gaps) and write N+1. An unsuffixed file counts as 0, like the mod does.
set "MAX=-1"
if exist "%CONTROLLER_CONFIG_DIR%\!BASE!.vdf" set "MAX=0"
for %%F in ("%CONTROLLER_CONFIG_DIR%\!BASE!_*.vdf") do (
    set "SUFFIX=%%~nF"
    set "SUFFIX=!SUFFIX:*%~2_=!"
    echo !SUFFIX!|findstr /r /c:"^[0-9][0-9]*$" >nul && (
        if !SUFFIX! gtr !MAX! set "MAX=!SUFFIX!"
    )
)
set /a NEXT=MAX+1

copy /y "!SRC!" "%CONTROLLER_CONFIG_DIR%\!BASE!_!NEXT!.vdf" >nul
if errorlevel 1 (
    echo   ERROR: Failed to copy "!SRC!" to "%CONTROLLER_CONFIG_DIR%\!BASE!_!NEXT!.vdf"
    exit /b 1
)
echo   installed as !BASE!_!NEXT!.vdf
exit /b 0
