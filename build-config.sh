#!/bin/bash
set -e

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
echo "et game_actions"
echo "-------------------------------------------"

cd SteamInputConfig
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de changer de répertoire vers SteamInputConfig"
    exit 1
fi

echo ""
echo "Construction des VDF..."
node merge.js 2>&1
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la construction des VDF"
    if [ -f "controllers.log" ]; then
        cat "controllers.log"
    fi
    exit 1
fi

cd ..
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de revenir au répertoire original"
    exit 1
fi

echo ""
echo "Copie des fichiers VDF vers le dossier Release"
cp SteamInputConfig/*.vdf "Release/"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de copier les fichiers VDF"
    exit 1
fi

echo ""
echo "Construction de la Configuration terminée avec succès"
