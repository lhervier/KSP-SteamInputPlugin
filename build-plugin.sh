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
echo "Construction du projet C# avec dotnet build"
echo "-------------------------------------------"

# Vérifier si KSPDIR est défini
if [ -z "$KSPDIR" ]; then
    echo "ERREUR: La variable d'environnement KSPDIR n'est pas définie"
    echo "Veuillez définir KSPDIR avec le chemin vers votre installation KSP"
    echo "Exemple: export KSPDIR=/home/lionel/.steam/steam/steamapps/common/Kerbal\ Space\ Program"
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
    echo "ERREUR: Assembly-CSharp.dll non trouvé dans $KSPDIR/KSP_x64_Data/Managed/ ou $KSPDIR/KSP_Data/Managed/"
    echo "Vérifiez que KSPDIR pointe vers le bon répertoire KSP"
    exit 1
fi

echo "Utilisation de KSPDIR: $KSPDIR"

# Créer le dossier de sortie
mkdir -p "Output/obj"

# Compiler avec dotnet build (.NET Framework 4.7.2 via ReferenceAssemblies)
echo "Compilation avec dotnet build..."
dotnet build -p:KSP_DATA_DIR="$KSP_DATA_DIR"

if [ $? -ne 0 ]; then
    echo "ERREUR: Échec de la compilation du projet C#"
    exit 1
fi

echo ""
echo "-------------------------------------------"
echo "Construction du fichier Zip du Plugin"
echo "-------------------------------------------"

echo "Suppression du dossier SteamInputMod s'il existe déjà"
if [ -d "Release/SteamInputMod" ]; then
    rm -rf "Release/SteamInputMod"
fi

echo "Création de la structure zip"
mkdir "Release/SteamInputMod"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de créer le dossier SteamInputMod"
    exit 1
fi

mkdir "Release/SteamInputMod/Textures"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de créer le dossier Textures"
    exit 1
fi

mkdir "Release/SteamInputMod/Localization"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de créer le dossier Localization"
    exit 1
fi

echo "Copie des fichiers du Plugin..."
echo "- Copie de SteamInputMod.dll"
cp "Output/bin/SteamInputMod.dll" "Release/SteamInputMod/"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de copier SteamInputMod.dll"
    exit 1
fi

echo "- Copie des Textures"
if [ ! -d "SteamInputPlugin/GameData/SteamInputMod/Textures" ]; then
    echo "ERREUR: SteamInputPlugin/GameData/SteamInputMod/Textures introuvable"
    exit 1
fi
cp -r "SteamInputPlugin/GameData/SteamInputMod/Textures/"* "Release/SteamInputMod/Textures/"
cp -r "SteamInputPlugin/KSP-Shared/GameData/Textures/"* "Release/SteamInputMod/Textures/"

echo "- Copie des fichiers de localisation"
if [ ! -d "SteamInputPlugin/GameData/SteamInputMod/Localization" ]; then
    echo "ERREUR: SteamInputPlugin/GameData/SteamInputMod/Localization introuvable"
    exit 1
fi
cp "SteamInputPlugin/GameData/SteamInputMod/Localization/"* "Release/SteamInputMod/Localization/"

echo "Création de l'archive zip"
cd "Release"
zip -r "SteamInputMod.zip" "SteamInputMod/"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de créer l'archive zip"
    exit 1
fi

echo "Suppression du dossier zip temporaire"
rm -rf "SteamInputMod"
if [ $? -ne 0 ]; then
    echo "ERREUR: Impossible de supprimer le dossier temporaire SteamInputMod"
    exit 1
fi

cd ..

echo ""
echo "Construction du Plugin terminée avec succès"
