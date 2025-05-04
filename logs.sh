#!/bin/bash

# Affiche les logs du plugin. Lit le contenu du fichier de log depuis le début.
tail -n +1 -f "${KSPDIR}/KSP.log" | grep SteamControllerPlugin
