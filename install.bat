@echo off
setlocal

set APPID=220200

if "%KSPLANG%"=="" (
    echo WARN: KSPLANG environment variable is missing. Default is french.
    set "KSPLANG=french"
)

if "%KSPDIR%"=="" (
    echo WARN: KSPDIR environment variable is missing. Using default.
    set "KSPDIR=C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"
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
echo - KSPDIR: %KSPDIR%
echo - STEAMDIR: %STEAMDIR%
echo - USERID: %USERID%

echo.
echo ===========================================
echo Installing config
echo ===========================================
./install-config.bat

echo.
echo ===========================================
echo Installing plugin
echo ===========================================
./install-plugin.bat

echo Installation completed successfully
