#!/bin/bash

# Affiche les logs du plugin. Lit le contenu du fichier de log depuis le début.
tail -f "${KSPDIR}/KSP.log" | grep -E "\[SteamCtrlr\]|\[KSPSteamController\]"
