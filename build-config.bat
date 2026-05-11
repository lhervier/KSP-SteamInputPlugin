@echo off
setlocal enabledelayedexpansion

echo.
echo -------------------------------------------
echo Running npm ci
echo -------------------------------------------
cd SteamInputConfig
call npm ci
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
echo Building VDF files
echo -------------------------------------------
cd SteamInputConfig
if errorlevel 1 (
    echo ERROR: Failed to change directory to SteamInputConfig
    exit /b 1
)

echo.
echo Building VDF for all controllers...
node merge-controller.js all 2>&1
if errorlevel 1 (
    echo ERROR: Failed to build VDF
    exit /b 1
)

echo.
echo -------------------------------------------
echo Building game_actions VDF files...
echo -------------------------------------------
echo.
echo Building game_actions VDF files...
node merge-game-actions.js 2>&1
if errorlevel 1 (
    echo ERROR: Failed to build game_actions VDF
    exit /b 1
)

cd ..
if errorlevel 1 (
    echo ERROR: Failed to return to original directory
    exit /b 1
)

echo.
echo Copying VDF files to Release folder
copy /y "SteamInputConfig\build\*.vdf" "Release\"
if errorlevel 1 (
    echo ERROR: Failed to copy VDF files
    exit /b 1
)
echo.
echo Config Build completed successfully
