#!/bin/bash
set -e

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

# Vérifier que les fichiers VDF existent
if [ ! -f "Release/game_actions_220200.vdf" ]; then
    echo "ERREUR: Fichiers VDF non trouvés dans Release/"
    echo "Exécutez d'abord: ./build.sh"
    exit 1
fi

echo ""
echo "==========================================="
echo "Configuration Steam Input"
echo "==========================================="

# Créer le dossier controller_config dans Steam
STEAM_DIR=""
if [ -d "$HOME/.steam/steam" ]; then
    STEAM_DIR="$HOME/.steam/steam"
elif [ -d "$HOME/.local/share/Steam" ]; then
    STEAM_DIR="$HOME/.local/share/Steam"
else
    echo "ATTENTION: Dossier Steam non trouvé"
    echo "Vous devrez copier manuellement les fichiers VDF dans:"
    echo "  \${SteamDir}/controller_config/"
    echo ""
    echo "Fichiers VDF disponibles dans Release/:"
    ls -la Release/*.vdf
    exit 0
fi

CONTROLLER_CONFIG_DIR="$STEAM_DIR/controller_config"
echo "Création du dossier: $CONTROLLER_CONFIG_DIR"
mkdir -p "$CONTROLLER_CONFIG_DIR"

# Copier les fichiers VDF
echo "Copie des fichiers de configuration VDF..."
cp Release/game_actions_220200*.vdf "$CONTROLLER_CONFIG_DIR/"

echo ""
echo "==========================================="
echo "Installation de la configuration terminée avec succès !"
echo "==========================================="
echo ""
echo "Configuration VDF copiée dans: $CONTROLLER_CONFIG_DIR"
echo ""
echo "Fichiers de configuration disponibles:"
echo "- game_actions_220200.vdf (général)"
echo "- game_actions_220200_english.vdf (anglais)"
echo "- game_actions_220200_french.vdf (français)"
echo ""
echo "Pour les contrôleurs spécifiques, copiez manuellement:"
echo "- controller_hori_steam*.vdf pour Horipad Steam"
echo "- controller_ps4*.vdf pour PS4/PS5"
echo "- controller_steamcontroller_gordon*.vdf pour Steam Controller"
echo ""
echo "Pour utiliser la configuration:"
echo "1. Lancez KSP via Steam"
echo "2. Configurez vos contrôleurs via l'interface Steam"
echo ""
echo "Note: Pour une installation complète, exécutez aussi:"
echo "  ./install-plugin.sh"
