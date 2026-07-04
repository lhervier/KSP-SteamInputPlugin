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
echo Building plugin (mod)
echo ===========================================
REM The mod is a self-contained sub-project: it builds into SteamInputPlugin\Release.
REM Gather its zip into the repo-root Release so the distribution bundle stays complete.
cmd /c "SteamInputPlugin\build.bat"
if errorlevel 1 (
    echo ERROR: Failed to build plugin
    exit /b 1
)
copy /y "SteamInputPlugin\Release\SteamInputMod.zip" "Release\"
if errorlevel 1 (
    echo ERROR: Failed to copy plugin zip into Release
    exit /b 1
)

echo.
echo ===========================================
echo Building config
echo ===========================================
cmd /c "build-config.bat"
if errorlevel 1 (
    echo ERROR: Failed to build config
    exit /b 1
)

echo.
echo Build completed successfully
