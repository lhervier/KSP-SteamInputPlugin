#!/bin/bash
set -e

echo ""
echo "-------------------------------------------"
echo "Mise à jour du sous-module KSP-Shared"
echo "-------------------------------------------"

# Récupère les derniers commits de la branche suivie (main) du sous-module,
# au lieu de rester sur le SHA figé par le dépôt parent.
git submodule update --remote --merge SteamInputPlugin/KSP-Shared

echo ""
echo "Sous-module KSP-Shared mis à jour avec succès"
echo ""
echo "Si la librairie a changé, pensez à committer le nouveau pointeur :"
echo "  git add SteamInputPlugin/KSP-Shared && git commit -m \"Bump KSP-Shared\""
