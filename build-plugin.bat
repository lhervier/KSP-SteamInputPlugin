@echo off
setlocal enabledelayedexpansion

echo.
echo -------------------------------------------
echo Checking Release folder
echo -------------------------------------------
if not exist "Release" (
    mkdir "Release"
)
if errorlevel 1 (
    echo ERROR: Failed to create Release folder
    exit /b 1
)

echo -------------------------------------------
echo Building dotnet project
echo -------------------------------------------
dotnet build
if errorlevel 1 (
    echo ERROR: Failed to build dotnet project
    exit /b 1
)

echo.
echo -------------------------------------------
echo Building Plugin Zip file
echo -------------------------------------------

echo Removing SteamInput folder if it already exists
if exist "Release\SteamInput" rmdir /s /q "Release\SteamInput"

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
echo Plugin Build completed successfully
