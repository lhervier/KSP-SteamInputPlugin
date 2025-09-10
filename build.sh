#!/bin/bash
set -e

echo ""
echo "==========================================="
echo "  Préparation du dossier Release"
echo "==========================================="

echo "Suppression du dossier Release"
if [ -d "Release" ]; then
    rm -rf "Release"
    if [ $? -ne 0 ]; then
        echo "ERREUR: Impossible de supprimer le dossier Release"
        exit 1
    fi
fi

echo "Recréation du dossier Release"
mkdir "Release"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de créer le dossier Release"
    exit 1
fi

echo "Copie de README.md vers le dossier Release"
cp "README.md" "Release/"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de copier README.md"
    exit 1
fi

echo ""
echo "==========================================="
echo "Construction du plugin"
echo "==========================================="
./build-plugin.sh
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la construction du plugin"
    exit 1
fi

echo ""
echo "==========================================="
echo "Construction de la configuration"
echo "==========================================="
./build-config.sh
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la construction de la configuration"
    exit 1
fi

echo ""
echo "Construction terminée avec succès"
