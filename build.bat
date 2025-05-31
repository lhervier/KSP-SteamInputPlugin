@echo off
setlocal enabledelayedexpansion

echo.
echo ===========================================
echo  Preparing Release folder
echo ===========================================

echo Removing Release folder
if exist "Release" rmdir /s /q "Release"
if errorlevel 1 (
    echo ERROR: Failed to remove Release folder
    exit /b 1
)

echo Re-creating Release folder
mkdir "Release"
if errorlevel 1 (
    echo ERROR: Failed to create Release folder
    exit /b 1
)

echo Copying README.md to Release folder
copy /y "README.md" "Release\"
if errorlevel 1 (
    echo ERROR: Failed to copy README.md
    exit /b 1
)

echo .
echo ===========================================
echo Building dotnet project
echo ===========================================
dotnet build
if errorlevel 1 (
    echo ERROR: Failed to build dotnet project
    exit /b 1
)

echo.
echo ===========================================
echo Building Plugin Zip file
echo ===========================================

echo Creating zip structure
mkdir "Release\SteamInput"
if errorlevel 1 (
    echo ERROR: Failed to create SteamInput folder
    exit /b 1
)
mkdir "Release\SteamInput\Textures"
if errorlevel 1 (
    echo ERROR: Failed to create Textures folder
    exit /b 1
)

echo Copying Plugin Files...
echo - Copying SteamInput.dll
copy /y "Output\obj\SteamInputPlugin.dll" "Release\SteamInput"
if errorlevel 1 (
    echo ERROR: Failed to copy SteamInputPlugin.dll
    exit /b 1
)
echo - Copying Textures
xcopy /y /i "SteamInputPlugin\Textures" "Release\SteamInput\Textures"
if errorlevel 1 (
    echo ERROR: Failed to copy Textures
    exit /b 1
)

echo Creating zip archive
powershell -Command "Compress-Archive -Path 'Release\SteamInput\*' -DestinationPath 'Release\SteamInput.zip' -Force"
if errorlevel 1 (
    echo ERROR: Failed to create zip archive
    exit /b 1
)

echo Removing zip folder
rmdir /s /q "Release\SteamInput"
if errorlevel 1 (
    echo ERROR: Failed to remove temporary SteamInput folder
    exit /b 1
)

echo.
echo ===========================================
echo Building VDF files for controllers
echo and game_actions
echo ===========================================
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
echo Build completed successfully
