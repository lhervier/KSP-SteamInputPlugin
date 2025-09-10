#!/bin/bash
set -e

echo ""
echo "==========================================="
echo "Installation du mod SteamInput pour KSP"
echo "==========================================="

echo ""
echo "==========================================="
echo "Installation du plugin"
echo "==========================================="
./install-plugin.sh
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de l'installation du plugin"
    exit 1
fi

echo ""
echo "==========================================="
echo "Installation de la configuration"
echo "==========================================="
./install-config.sh
if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de l'installation de la configuration"
    exit 1
fi

echo ""
echo "==========================================="
echo "Installation terminée avec succès !"
echo "==========================================="
echo ""
echo "Pour utiliser le mod:"
echo "1. Lancez KSP via Steam"
echo "2. Le plugin sera automatiquement chargé"
echo "3. Configurez vos contrôleurs via l'interface Steam"
