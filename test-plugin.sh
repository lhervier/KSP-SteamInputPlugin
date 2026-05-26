#!/bin/bash
set -e

echo ""
echo "-------------------------------------------"
echo "Détection de la structure KSP"
echo "-------------------------------------------"

# Vérifier si KSPDIR est défini
if [ -z "$KSPDIR" ]; then
    echo "ERREUR: La variable d'environnement KSPDIR n'est pas définie"
    echo "Veuillez définir KSPDIR avec le chemin vers votre installation KSP"
    exit 1
fi

# Vérifier que les DLLs KSP existent (Windows ou Linux)
if [ -f "$KSPDIR/KSP_x64_Data/Managed/Assembly-CSharp.dll" ]; then
    echo "Structure Windows détectée (KSP_x64_Data)"
    KSP_DATA_DIR="$KSPDIR/KSP_x64_Data"
elif [ -f "$KSPDIR/KSP_Data/Managed/Assembly-CSharp.dll" ]; then
    echo "Structure Linux détectée (KSP_Data)"
    KSP_DATA_DIR="$KSPDIR/KSP_Data"
else
    echo "ERREUR: Assembly-CSharp.dll non trouvé sous $KSPDIR"
    exit 1
fi

echo "Utilisation de KSP_DATA_DIR: $KSP_DATA_DIR"

echo ""
echo "-------------------------------------------"
echo "Exécution des tests"
echo "-------------------------------------------"
dotnet test SteamInputPlugin.Tests/SteamInputPlugin.Tests.csproj -p:KSP_DATA_DIR="$KSP_DATA_DIR" --logger "console;verbosity=detailed"

if [ $? -ne 0 ]; then
    echo "ERREUR: Échec des tests"
    exit 1
fi

echo ""
echo "Tests terminés avec succès"
