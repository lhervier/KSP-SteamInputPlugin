#!/bin/bash
set -e

echo ""
echo "-------------------------------------------"
echo "Running npm ci"
echo "-------------------------------------------"

cd MergeScripts
npm ci
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de lancer npm ci"
    exit 1
fi
cd ..

echo ""
echo "-------------------------------------------"
echo "Vérification du dossier Release"
echo "-------------------------------------------"

if [ ! -d "Release" ]; then
    mkdir "Release"
    if [ $? -ne 0 ]; then
        echo "ERREUR: Impossible de créer le dossier Release"
        exit 1
    fi
fi

echo ""
echo "-------------------------------------------"
echo "Construction des fichiers VDF pour les contrôleurs"
echo "-------------------------------------------"

cd MergeScripts
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de changer de répertoire vers MergeScripts"
    exit 1
fi

echo ""
echo "Construction des VDF..."
CONTROLLERS_JSON="../SteamInputConfig/controllers.json"
node merge-controller.js "$CONTROLLERS_JSON" all 2>&1
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la construction des VDF"
    exit 1
fi
echo ""
echo "-------------------------------------------"
echo "Construction des fichiers VDF pour game_actions"
echo "-------------------------------------------"

echo ""
echo "Construction des VDF..."
GAME_ACTIONS_VDF="../SteamInputConfig/game_actions_220200.vdf"
node merge-game-actions.js "$GAME_ACTIONS_VDF" 2>&1
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la construction des VDF game_actions"
    exit 1
fi

cd ..
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de revenir au répertoire original"
    exit 1
fi

echo ""
echo "Copie des fichiers VDF vers le dossier Release"
cp MergeScripts/build/*.vdf "Release/"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de copier les fichiers VDF"
    exit 1
fi

echo ""
echo "Construction de la Configuration terminée avec succès"
