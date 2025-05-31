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
echo Building plugin
echo ===========================================
cmd /c "build-plugin.bat"
if errorlevel 1 (
    echo ERROR: Failed to build plugin
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
