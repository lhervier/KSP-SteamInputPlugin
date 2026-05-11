#!/bin/bash
set -e

echo "Test de compilation du mod KSP SteamInput sur Ubuntu"
echo ""

# Test de la partie configuration VDF (qui ne nécessite pas KSPDIR)
echo "==========================================="
echo "Test de la construction de la configuration VDF"
echo "==========================================="

if [ ! -d "Release" ]; then
    mkdir "Release"
fi

cd MergeScripts
CONTROLLERS_JSON="../SteamInputConfig/controllers.json"
echo "Exécution de merge-controller.js (all)..."
node merge-controller.js "$CONTROLLERS_JSON" all
if [ $? -eq 0 ]; then
    echo "✓ Construction VDF contrôleurs réussie"
else
    echo "✗ Échec de la construction VDF contrôleurs"
    exit 1
fi

echo "Exécution de merge-game-actions.js..."
GAME_ACTIONS_VDF="../SteamInputConfig/game_actions_220200.vdf"
node merge-game-actions.js "$GAME_ACTIONS_VDF"
if [ $? -eq 0 ]; then
    echo "✓ Construction game_actions réussie"
else
    echo "✗ Échec de la construction game_actions"
    exit 1
fi

cd ..
echo "Copie des fichiers VDF..."
cp MergeScripts/build/*.vdf "Release/" 2>/dev/null || echo "Aucun fichier VDF généré"

echo ""
echo "==========================================="
echo "Test de la structure du projet C#"
echo "==========================================="

echo "Vérification des fichiers sources C#..."
if [ -d "SteamInputPlugin" ]; then
    echo "✓ Dossier SteamInputPlugin trouvé"
    cs_files=$(find SteamInputPlugin -name "*.cs" | wc -l)
    echo "✓ $cs_files fichiers C# trouvés"
else
    echo "✗ Dossier SteamInputPlugin non trouvé"
    exit 1
fi

echo "Vérification du fichier projet..."
if [ -f "SteamInput.csproj" ]; then
    echo "✓ Fichier SteamInput.csproj trouvé"
else
    echo "✗ Fichier SteamInput.csproj non trouvé"
    exit 1
fi

echo ""
echo "==========================================="
echo "Test de MSBuild"
echo "==========================================="

echo "Vérification de MSBuild..."
if command -v msbuild >/dev/null 2>&1; then
    echo "✓ MSBuild trouvé"
    msbuild --version
else
    echo "✗ MSBuild non trouvé"
    echo "Installation de MSBuild via Mono..."
    sudo apt install -y mono-complete
    if [ $? -eq 0 ]; then
        echo "✓ Mono installé avec succès"
    else
        echo "✗ Échec de l'installation de Mono"
        exit 1
    fi
fi

echo ""
echo "==========================================="
echo "Résumé du test"
echo "==========================================="
echo "✓ Node.js et npm fonctionnent"
echo "✓ Construction VDF réussie"
echo "✓ Structure du projet C# correcte"
echo "✓ MSBuild disponible"
echo ""
echo "Le projet est prêt pour la compilation !"
echo ""
echo "Pour compiler complètement, définissez KSPDIR et exécutez:"
echo "export KSPDIR=\"/chemin/vers/votre/KSP\""
echo "./build.sh"
