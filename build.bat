@echo off
setlocal

set KSPDIR=%~1
set PROJECTDIR=%~2
set TARGETPATH=%~3
set OUTPUTPATH=%~4

if "%KSPDIR%"=="" (
    echo ERROR: KSPDIR parameter is missing
    exit /b 1
)

echo.
echo ===========================================
echo Building and Deploying Plugin
echo ===========================================

echo Removing existing SteamInput folder in GameData
if exist "%KSPDIR%\GameData\SteamInput" rmdir /s /q "%KSPDIR%\GameData\SteamInput"
echo Creating SteamInput folder in GameData
mkdir "%KSPDIR%\GameData\SteamInput"

echo.
echo Copying Plugin Files...
echo - Copying SteamInput.dll
copy /y "%TARGETPATH%" "%KSPDIR%\GameData\SteamInput"
echo - Copying Textures
xcopy /y /i "%PROJECTDIR%SteamInputPlugin\Textures" "%KSPDIR%\GameData\SteamInput\Textures"

echo.
echo ===========================================
echo Building VDF files for controllers
echo ===========================================
cd /d "%PROJECTDIR%SteamInputConfig"

echo.
echo Building Steam Controller VDF...
node merge.js "controller_steamcontroller_gordon" "english" > steam_controller.log 2>&1
if errorlevel 1 (
echo ERROR: Failed to build Steam Controller VDF
type steam_controller.log
exit /b 1
)

cd /d "%PROJECTDIR%"

echo.
echo ===========================================
echo Creating Release Package
echo ===========================================

echo Removing existing Release folder
if exist "%PROJECTDIR%Release" rmdir /s /q "%PROJECTDIR%Release"

echo Creating Release directory
mkdir "%PROJECTDIR%Release"

echo Creating zip archive
powershell -Command "Compress-Archive -Path '%KSPDIR%\GameData\SteamInput\*' -DestinationPath '%PROJECTDIR%Release\SteamInput.zip' -Force"

echo Copying VDF files to Release folder
copy /y "%PROJECTDIR%SteamInputConfig\game_actions_220200.vdf" "%PROJECTDIR%Release\"
copy /y "%PROJECTDIR%SteamInputConfig\controller_steamcontroller_gordon.vdf" "%PROJECTDIR%Release\"

echo Copying README.md to Release folder
copy /y "%PROJECTDIR%README.md" "%PROJECTDIR%Release\"

echo.
echo Build completed successfully
