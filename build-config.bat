@echo off
setlocal enabledelayedexpansion

echo.
echo -------------------------------------------
echo Running npm ci
echo -------------------------------------------
cd MergeScripts
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
cd MergeScripts
if errorlevel 1 (
    echo ERROR: Failed to change directory to MergeScripts
    exit /b 1
)

echo.
echo Building VDF for all controllers...
set "CONTROLLERS_JSON=..\SteamInputConfig\controllers.json"
node merge-controller.js "%CONTROLLERS_JSON%" all 2>&1
if errorlevel 1 (
    echo ERROR: Failed to build VDF
    exit /b 1
)

echo.
echo -------------------------------------------
echo Building game_actions VDF files...
echo -------------------------------------------
set "GAME_ACTIONS_VDF=..\SteamInputConfig\game_actions_220200.vdf"
node merge-game-actions.js "%GAME_ACTIONS_VDF%" 2>&1
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
copy /y "MergeScripts\build\*.vdf" "Release\"
if errorlevel 1 (
    echo ERROR: Failed to copy VDF files
    exit /b 1
)
echo.
echo Config Build completed successfully
