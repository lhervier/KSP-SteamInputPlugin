#!/bin/bash
set -e

echo ""
echo "==========================================="
echo "Installation du plugin SteamInput pour KSP"
echo "==========================================="

# Vérifier si KSPDIR est défini
if [ -z "$KSPDIR" ]; then
    echo "ERREUR: La variable d'environnement KSPDIR n'est pas définie"
    echo "Exécutez d'abord: ./setup-env.sh"
    exit 1
fi

# Vérifier que le dossier Release existe
if [ ! -d "Release" ]; then
    echo "ERREUR: Le dossier Release n'existe pas"
    echo "Exécutez d'abord: ./build.sh"
    exit 1
fi

# Vérifier que le plugin a été compilé
if [ ! -f "Release/SteamInput.zip" ]; then
    echo "ERREUR: SteamInput.zip non trouvé dans Release/"
    echo "Exécutez d'abord: ./build.sh"
    exit 1
fi

GAMEDATA_DIR="$KSPDIR/GameData/SteamInput"
PLUGIN_DATA="$GAMEDATA_DIR/PluginData"
BACKUP="${TMPDIR:-/tmp}/KSP-SteamInput-PluginData-backup"

echo "Installation du plugin dans: $KSPDIR"
echo ""

echo "-------------------------------------------"
echo "Sauvegarde du PluginData existant (config)"
echo "-------------------------------------------"

rm -rf "$BACKUP"
if [ -d "$PLUGIN_DATA" ]; then
    echo "Sauvegarde de $PLUGIN_DATA"
    cp -a "$PLUGIN_DATA" "$BACKUP"
else
    echo "Aucun dossier PluginData à sauvegarder"
fi

echo ""
echo "-------------------------------------------"
echo "Installation des fichiers du plugin"
echo "-------------------------------------------"

# Créer le dossier GameData/SteamInput s'il n'existe pas
echo "Création du dossier: $GAMEDATA_DIR"
mkdir -p "$GAMEDATA_DIR"

# Extraire le plugin
echo "Extraction du plugin..."
cd "$GAMEDATA_DIR"
unzip -o "$OLDPWD/Release/SteamInput.zip"
# Le zip contient un dossier SteamInput, on doit déplacer son contenu
if [ -d "SteamInput" ]; then
    echo "Déplacement du contenu du dossier SteamInput..."
    # Supprimer seulement les fichiers et dossiers du plugin (pas PluginData)
    rm -f *.dll *.pdb
    rm -rf Textures Localization
    # Déplacer le nouveau contenu
    mv SteamInput/* .
    rmdir SteamInput
fi
cd "$OLDPWD"

echo ""
echo "-------------------------------------------"
echo "Restauration du PluginData"
echo "-------------------------------------------"

if [ -d "$BACKUP" ]; then
    echo "Restauration de la config dans $PLUGIN_DATA"
    mkdir -p "$PLUGIN_DATA"
    cp -a "$BACKUP/." "$PLUGIN_DATA/"
    rm -rf "$BACKUP"
else
    echo "Aucune sauvegarde PluginData à restaurer"
fi

echo ""
echo "==========================================="
echo "Installation du plugin terminée avec succès !"
echo "==========================================="
echo ""
echo "Plugin installé dans: $GAMEDATA_DIR"
echo ""
echo "Pour utiliser le plugin:"
echo "1. Lancez KSP via Steam"
echo "2. Le plugin sera automatiquement chargé"
echo ""
echo "Note: Pour une configuration complète, exécutez aussi:"
echo "  ./install-config.sh"
