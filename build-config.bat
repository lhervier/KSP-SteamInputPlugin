@echo off
setlocal enabledelayedexpansion

echo.
echo -------------------------------------------
echo Running npm ci
echo -------------------------------------------
cd SteamInputConfig
npm ci
if errorlevel 1 (
    echo ERROR: Failed to run npm ci
    exit /b 1
)
cd ..

echo.
echo -------------------------------------------
echo Checking Release folder
echo -------------------------------------------

if not exist "Release" (
    mkdir "Release"
    if errorlevel 1 (
        echo ERROR: Failed to create Release folder
        exit /b 1
    )
)

echo.
echo -------------------------------------------
echo Building VDF files for controllers
echo and game_actions
echo -------------------------------------------
cd SteamInputConfig
if errorlevel 1 (
    echo ERROR: Failed to change directory to SteamInputConfig
    exit /b 1
)

echo.
echo Building VDF...
node merge.js 2>&1
if errorlevel 1 (
    echo ERROR: Failed to build VDF
    type controllers.log
    exit /b 1
)

cd ..
if errorlevel 1 (
    echo ERROR: Failed to return to original directory
    exit /b 1
)

echo.
echo Copying VDF files to Release folder
copy /y "SteamInputConfig\*.vdf" "Release\"
if errorlevel 1 (
    echo ERROR: Failed to copy VDF files
    exit /b 1
)
echo.
echo Config Build completed successfully
