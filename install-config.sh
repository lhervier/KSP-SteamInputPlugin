#!/bin/bash
set -e

APPID=220200
KSPLANG="${KSPLANG:-french}"
USERID="${USERID:-27319809}"

echo ""
echo "==========================================="
echo "Installation de la configuration Steam Input"
echo "==========================================="

# Vérifier que le dossier Release existe
if [ ! -d "Release" ]; then
    echo "ERREUR: Le dossier Release n'existe pas"
    echo "Exécutez d'abord: ./build.sh"
    exit 1
fi

GA_SRC="Release/game_actions_${APPID}_${KSPLANG}.vdf"
SC_SRC="Release/ksp_steaminput_steamcontroller_${KSPLANG}.vdf"
SC_V2_SRC="Release/ksp_steaminput_steamcontroller_v2_${KSPLANG}.vdf"
HORI_SRC="Release/ksp_steaminput_hori_steam_${KSPLANG}.vdf"
ELITE_SRC="Release/ksp_steaminput_xboxelite_${KSPLANG}.vdf"
PS4_SRC="Release/ksp_steaminput_ps4_${KSPLANG}.vdf"

for f in "$GA_SRC" "$SC_SRC" "$SC_V2_SRC" "$HORI_SRC" "$ELITE_SRC" "$PS4_SRC"; do
    if [ ! -f "$f" ]; then
        echo "ERREUR: Fichier manquant: $f"
        echo "Exécutez d'abord: ./build.sh"
        exit 1
    fi
done

echo ""
echo "Paramètres : KSPLANG=$KSPLANG USERID=$USERID APPID=$APPID"

echo ""
echo "==========================================="
echo "Configuration Steam Input"
echo "==========================================="

# Créer le dossier controller_config dans Steam
STEAM_DIR=""
if [ -n "${STEAMDIR:-}" ] && [ -d "$STEAMDIR" ]; then
    STEAM_DIR="$STEAMDIR"
elif [ -d "$HOME/.steam/steam" ]; then
    STEAM_DIR="$HOME/.steam/steam"
elif [ -d "$HOME/.local/share/Steam" ]; then
    STEAM_DIR="$HOME/.local/share/Steam"
else
    echo "ATTENTION: Dossier Steam non trouvé (STEAMDIR, ~/.steam/steam, ~/.local/share/Steam)"
    echo "export STEAMDIR=/chemin/vers/Steam puis relancez, ou copiez à la main:"
    echo "  game_actions → \${SteamDir}/controller_config/game_actions_${APPID}.vdf"
    echo "  contrôleurs → \${SteamDir}/steamapps/common/Steam Controller Configs/${USERID}/config/${APPID}/"
    ls -la Release/*.vdf 2>/dev/null || true
    exit 1
fi

CONTROLLER_ACTION_DIR="$STEAM_DIR/controller_config"
CONTROLLER_CONFIG_DIR="$STEAM_DIR/steamapps/common/Steam Controller Configs/$USERID/config/$APPID"

echo "Steam: $STEAM_DIR"
mkdir -p "$CONTROLLER_ACTION_DIR"
mkdir -p "$CONTROLLER_CONFIG_DIR"

echo "Copie game_actions → $CONTROLLER_ACTION_DIR/game_actions_${APPID}.vdf"
cp "$GA_SRC" "$CONTROLLER_ACTION_DIR/game_actions_${APPID}.vdf"

echo "Copie configs manettes → $CONTROLLER_CONFIG_DIR/"
cp "$SC_SRC" "$CONTROLLER_CONFIG_DIR/controller_steamcontroller_gordon.vdf"
cp "$SC_V2_SRC" "$CONTROLLER_CONFIG_DIR/controller_triton.vdf"
cp "$HORI_SRC" "$CONTROLLER_CONFIG_DIR/controller_hori_steam.vdf"
cp "$ELITE_SRC" "$CONTROLLER_CONFIG_DIR/controller_xboxelite.vdf"
cp "$PS4_SRC" "$CONTROLLER_CONFIG_DIR/controller_ps4.vdf"

echo ""
echo "==========================================="
echo "Installation de la configuration terminée avec succès !"
echo "==========================================="
echo ""
echo "Fichiers installés:"
echo "- $CONTROLLER_ACTION_DIR/game_actions_${APPID}.vdf"
echo "- $CONTROLLER_CONFIG_DIR/controller_steamcontroller_gordon.vdf"
echo "- $CONTROLLER_CONFIG_DIR/controller_triton.vdf"
echo "- $CONTROLLER_CONFIG_DIR/controller_hori_steam.vdf"
echo "- $CONTROLLER_CONFIG_DIR/controller_xboxelite.vdf"
echo "- $CONTROLLER_CONFIG_DIR/controller_ps4.vdf"
echo ""
