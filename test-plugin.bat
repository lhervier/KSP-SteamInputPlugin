@echo off
setlocal enabledelayedexpansion

echo.
echo -------------------------------------------
echo Detection de la structure KSP
echo -------------------------------------------

if not defined KSPDIR (
    echo ERREUR: La variable d'environnement KSPDIR n'est pas definie
    echo Veuillez definir KSPDIR avec le chemin vers votre installation KSP
    exit /b 1
)

if exist "!KSPDIR!\KSP_x64_Data\Managed\Assembly-CSharp.dll" (
    echo Structure Windows detectee ^(KSP_x64_Data^)
    set "KSP_DATA_DIR=!KSPDIR!\KSP_x64_Data"
) else if exist "!KSPDIR!\KSP_Data\Managed\Assembly-CSharp.dll" (
    echo Structure Linux detectee ^(KSP_Data^)
    set "KSP_DATA_DIR=!KSPDIR!\KSP_Data"
) else (
    echo ERREUR: Assembly-CSharp.dll non trouve sous !KSPDIR!
    exit /b 1
)

echo Utilisation de KSP_DATA_DIR: !KSP_DATA_DIR!

echo.
echo -------------------------------------------
echo Running tests
echo -------------------------------------------
dotnet test SteamInputPlugin.Tests\SteamInputPlugin.Tests.csproj -p:KSP_DATA_DIR="%KSP_DATA_DIR%" --logger "console;verbosity=detailed"
if errorlevel 1 (
    echo ERROR: Tests failed
    exit /b 1
)

echo.
echo Tests completed successfully
