@echo off
setlocal

echo.
echo -------------------------------------------
echo Mise a jour du sous-module KSP-Shared
echo -------------------------------------------

REM Récupère les derniers commits de la branche suivie (main) du sous-module,
REM au lieu de rester sur le SHA figé par le dépôt parent.
git submodule update --remote --merge SteamInputPlugin/KSP-Shared
if errorlevel 1 (
    echo ERROR: Failed to update KSP-Shared submodule
    exit /b 1
)

echo.
echo Sous-module KSP-Shared mis a jour avec succes
echo.
echo Si la librairie a change, pensez a committer le nouveau pointeur :
echo   git add SteamInputPlugin/KSP-Shared ^&^& git commit -m "Bump KSP-Shared"
