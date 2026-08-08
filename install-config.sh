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

# Config base names, without the _<N> version suffix Steam appends to each export
CONTROLLER_BASES="
ksp_steaminput_steamcontroller_${KSPLANG}
ksp_steaminput_steamcontroller_v2_${KSPLANG}
ksp_steaminput_hori_steam_${KSPLANG}
ksp_steaminput_xboxelite_${KSPLANG}
ksp_steaminput_ps4_${KSPLANG}
"

check_source() {
    if [ ! -f "$1" ]; then
        echo "ERREUR: Fichier manquant: $1"
        echo "Exécutez d'abord: ./build.sh"
        exit 1
    fi
}

check_source "$GA_SRC"
for base in $CONTROLLER_BASES; do
    check_source "Release/${base}.vdf"
done

# Installs Release/<base>.vdf as a new version of the <base> config.
#
# Steam names every exported config "<base>_<N>.vdf" and increments N on each export,
# and the mod loads the highest N. So scan for the highest existing N (numbering may
# have gaps) and write N+1. An unsuffixed file counts as 0, like the mod does.
install_controller() {
    base="$1"
    max=-1
    if [ -f "$CONTROLLER_CONFIG_DIR/${base}.vdf" ]; then
        max=0
    fi
    for f in "$CONTROLLER_CONFIG_DIR/${base}"_*.vdf; do
        if [ ! -f "$f" ]; then
            continue
        fi
        suffix="${f##*_}"
        suffix="${suffix%.vdf}"
        case "$suffix" in
            ''|*[!0-9]*) continue ;;
        esac
        if [ "$suffix" -gt "$max" ]; then
            max="$suffix"
        fi
    done

    next=$((max + 1))
    cp "Release/${base}.vdf" "$CONTROLLER_CONFIG_DIR/${base}_${next}.vdf"
    echo "- ${base}_${next}.vdf"
}

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
echo "- game_actions_${APPID}.vdf"
for base in $CONTROLLER_BASES; do
    install_controller "$base"
done

echo ""
echo "==========================================="
echo "Installation de la configuration terminée avec succès !"
echo "==========================================="
echo ""
