@echo off
setlocal

set PROJECTDIR=%~1
set TARGETPATH=%~2

if "%PROJECTDIR%"=="" (
    echo ERROR: PROJECTDIR parameter is missing
    exit /b 1
)
echo PROJECTDIR: %PROJECTDIR%
if "%TARGETPATH%"=="" (
    echo ERROR: TARGETPATH parameter is missing
    exit /b 1
)

echo.
echo ===========================================
echo  Preparing Release folder
echo ===========================================

echo Removing Release folder
if exist "%PROJECTDIR%Release" rmdir /s /q "%PROJECTDIR%Release"

echo Re-creating Release folder
mkdir "%PROJECTDIR%Release"

echo Copying README.md to Release folder
copy /y "%PROJECTDIR%README.md" "%PROJECTDIR%Release\"

echo.
echo ===========================================
echo Building Plugin Zip file
echo ===========================================

echo Creating zip structure
mkdir "%PROJECTDIR%Release\SteamInput"
mkdir "%PROJECTDIR%Release\SteamInput\Textures"

echo Copying Plugin Files...
echo - Copying SteamInput.dll
copy /y "%TARGETPATH%" "%PROJECTDIR%Release\SteamInput"
echo - Copying Textures
xcopy /y /i "%PROJECTDIR%SteamInputPlugin\Textures" "%PROJECTDIR%Release\SteamInput\Textures"

echo Creating zip archive
powershell -Command "Compress-Archive -Path '%PROJECTDIR%Release\SteamInput\*' -DestinationPath '%PROJECTDIR%Release\SteamInput.zip' -Force"

echo Removing zip folder
rmdir /s /q "%PROJECTDIR%Release\SteamInput"

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

echo.
echo Building PS4 VDF...
node merge.js "controller_ps4" "english" > ps4.log 2>&1
if errorlevel 1 (
echo ERROR: Failed to build PS4 VDF
type ps4.log
exit /b 1
)

cd /d "%PROJECTDIR%"

echo Copying VDF files to Release folder
copy /y "%PROJECTDIR%SteamInputConfig\game_actions_%APPID%.vdf" "%PROJECTDIR%Release\"
copy /y "%PROJECTDIR%SteamInputConfig\controller_steamcontroller_gordon.vdf" "%PROJECTDIR%Release\"
copy /y "%PROJECTDIR%SteamInputConfig\controller_ps4.vdf" "%PROJECTDIR%Release\"

echo.
echo Build completed successfully
