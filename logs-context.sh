#!/bin/bash

CTX=$1

if [[ -z "$CTX" ]]; then
    CTX="Contexts"
fi

# Affiche d'abord le contenu existant du fichier, puis continue avec les nouvelles lignes
cat "${KSPDIR}/KSP.log" | grep "\[$CTX\]"
tail -f "${KSPDIR}/KSP.log" | grep "\[$CTX\]"
