@echo off
setlocal enabledelayedexpansion

echo.
echo -------------------------------------------
echo Detection de la structure KSP
echo -------------------------------------------

REM Vérifier si KSPDIR est défini
if not defined KSPDIR (
    echo ERREUR: La variable d'environnement KSPDIR n'est pas définie
    echo Veuillez définir KSPDIR avec le chemin vers votre installation KSP
    echo Exemple: set "KSPDIR=C:\Program Files ^(x86^)\Steam\steamapps\common\Kerbal Space Program"
    exit /b 1
)

REM Vérifier que les DLLs KSP existent (Windows ou Linux)
REM Utiliser !KSPDIR! dans les blocs ( ) : les parenthèses de "(x86)" cassent sinon le parsing batch
if exist "!KSPDIR!\KSP_x64_Data\Managed\Assembly-CSharp.dll" (
    echo Structure Windows détectée ^(KSP_x64_Data^)
    set "KSP_DATA_DIR=!KSPDIR!\KSP_x64_Data"
) else if exist "!KSPDIR!\KSP_Data\Managed\Assembly-CSharp.dll" (
    echo Structure Linux détectée ^(KSP_Data^)
    set "KSP_DATA_DIR=!KSPDIR!\KSP_Data"
) else (
    echo ERREUR: Assembly-CSharp.dll non trouvé dans !KSPDIR!\KSP_x64_Data\Managed\ ou !KSPDIR!\KSP_Data\Managed\
    echo Vérifiez que KSPDIR pointe vers le bon répertoire KSP
    exit /b 1
)

echo Utilisation de KSPDIR: !KSPDIR!
echo Utilisation de KSP_DATA_DIR: !KSP_DATA_DIR!

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
dotnet build -p:KSP_DATA_DIR="%KSP_DATA_DIR%"
if errorlevel 1 (
    echo ERROR: Failed to build dotnet project
    exit /b 1
)

echo.
echo -------------------------------------------
echo Building Plugin Zip file
echo -------------------------------------------

echo Removing SteamInput folder if it already exists
if exist "Release\SteamInputMod" rmdir /s /q "Release\SteamInputMod"

echo Creating zip structure
mkdir "Release\SteamInputMod"
if errorlevel 1 (
    echo ERROR: Failed to create SteamInputMod folder
    exit /b 1
)
mkdir "Release\SteamInputMod\Textures"
if errorlevel 1 (
    echo ERROR: Failed to create Textures folder
    exit /b 1
)
mkdir "Release\SteamInputMod\Localization"
if errorlevel 1 (
    echo ERROR: Failed to create Localization folder
    exit /b 1
)

echo Copying Plugin Files...
echo - Copying SteamInputMod.dll
copy /y "Output\obj\SteamInputMod.dll" "Release\SteamInputMod"
if errorlevel 1 (
    echo ERROR: Failed to copy SteamInputMod.dll
    exit /b 1
)

echo - Copying texture files
if not exist "SteamInputPlugin\GameData\SteamInputMod\Textures" (
    echo ERROR: SteamInputPlugin\GameData\SteamInput\Textures folder not found
    exit /b 1
)
copy /y "SteamInputPlugin\GameData\SteamInputMod\Textures\*" "Release\SteamInputMod\Textures\"
if errorlevel 1 (
    echo ERROR: Failed to copy Textures
    exit /b 1
)
copy /y "SteamInputPlugin\KSP-Shared\GameData\Textures\*" "Release\SteamInputMod\Textures\"
if errorlevel 1 (
    echo ERROR: Failed to copy shared Textures
    exit /b 1
)

echo - Copying localization files
if not exist "SteamInputPlugin\GameData\SteamInputMod\Localization" (
    echo ERROR: SteamInputPlugin\GameData\SteamInputMod\Localization folder not found
    exit /b 1
)
copy /y "SteamInputPlugin\GameData\SteamInputMod\Localization\*" "Release\SteamInputMod\Localization\"
if errorlevel 1 (
    echo ERROR: Failed to copy Localization
    exit /b 1
)

echo Creating zip archive
powershell -Command "Compress-Archive -Path 'Release\SteamInputMod\*' -DestinationPath 'Release\SteamInputMod.zip' -Force"
if errorlevel 1 (
    echo ERROR: Failed to create zip archive
    exit /b 1
)

echo Removing zip folder
rmdir /s /q "Release\SteamInputMod"
if errorlevel 1 (
    echo ERROR: Failed to remove temporary SteamInput folder
    exit /b 1
)

echo.
echo Plugin Build completed successfully
