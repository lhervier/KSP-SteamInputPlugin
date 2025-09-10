#!/bin/bash

echo "Configuration de l'environnement pour la compilation du mod KSP SteamInput"
echo ""

# Chercher l'installation KSP dans les emplacements courants
KSP_PATHS=(
    "$HOME/.steam/steam/steamapps/common/Kerbal Space Program"
    "$HOME/.local/share/Steam/steamapps/common/Kerbal Space Program"
    "/home/$USER/.steam/steam/steamapps/common/Kerbal Space Program"
    "/home/$USER/.local/share/Steam/steamapps/common/Kerbal Space Program"
)

echo "Recherche de l'installation KSP..."
for path in "${KSP_PATHS[@]}"; do
    echo "Vérification de: '$path'"
    if [ -d "$path" ]; then
        echo "  - Dossier trouvé"
        # Vérifier les deux structures possibles (Windows et Linux)
        if [ -f "$path/KSP_x64_Data/Managed/Assembly-CSharp.dll" ]; then
            echo "  - Assembly-CSharp.dll trouvé (Windows: KSP_x64_Data)"
            echo "KSP trouvé dans: $path"
            export KSPDIR="$path"
            echo "export KSPDIR=\"$path\"" >> ~/.bashrc
            echo ""
            echo "KSPDIR a été défini et ajouté à votre ~/.bashrc"
            echo "Vous pouvez maintenant exécuter: source ~/.bashrc"
            echo "Puis lancer: ./build.sh"
            exit 0
        elif [ -f "$path/KSP_Data/Managed/Assembly-CSharp.dll" ]; then
            echo "  - Assembly-CSharp.dll trouvé (Linux: KSP_Data)"
            echo "KSP trouvé dans: $path"
            export KSPDIR="$path"
            echo "export KSPDIR=\"$path\"" >> ~/.bashrc
            echo ""
            echo "KSPDIR a été défini et ajouté à votre ~/.bashrc"
            echo "Vous pouvez maintenant exécuter: source ~/.bashrc"
            echo "Puis lancer: ./build.sh"
            exit 0
        else
            echo "  - Assembly-CSharp.dll non trouvé (ni dans KSP_x64_Data ni dans KSP_Data)"
        fi
    else
        echo "  - Dossier non trouvé"
    fi
done

echo "KSP non trouvé dans les emplacements courants."
echo ""
echo "Veuillez définir manuellement KSPDIR avec le chemin vers votre installation KSP:"
echo "export KSPDIR=\"/chemin/vers/votre/KSP\""
echo ""
echo "Puis exécutez: ./build.sh"
