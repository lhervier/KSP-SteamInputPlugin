# Structure du projet

Il s'agit d'un mod pour Kerbal Space Program. LE dossier "SteamInputPlugin" contient un mod qui ajoute le support SteamInput. Ne le regarde pas, ce n'est pas le but de ma question.

Le dossier SteamInputConfig2 est important pour ma question, lit le bien. Il contient des fichiers et un script qui permet de générer une configuration SteamInput pour plusieurs controlleurs (steam controller, hori steam pad et XBox Elite). Le dossier SteamInputConfig contient une vieille version que je supprimerai à terme. Ne le regarde pas.

# Objectif de cette conversation

Mon objectif est de permettre de créer une configuration correcte pour le controlleur PS4. 

La différence entre ce controlleur et les 3 autres déjà supportés, c'est qu'il n'a pas de boutons à l'arrière. Or, ces boutons sont critiques : 

- Le bouton arrière droit correspond à la touche "Mod", beaucoup utilisée dans KSP
- Mais surtout le bouton arrière gauche fait des "modeshift", en changeant la configuration de chacun des groupes, en fonction du mode dans lequel on est.

Tu peux regarder les inputs de type "switch", où tu trouveras les définitions des "button_back_right" et "button_back_left".

Mais le controlleur PS4 ne possède pas ses boutons. Je ne vais donc pas supporter les controlleurs standards, mais seulement ceux qui ajoutent ces boutons. Or, ce n'est jamais des véritables boutons supplémentaires. J4ai par exemple un controlleur Raiju Tournament Edition, qui me permet de mapper ces boutons sur d'autres boutons. Et je veux utiliser le "click" sur chacun des deux joysticks à la place.

Dans ma conf, je dois donc changer pour le controlleur PS4, et inverser les clicks sur les boutons arrières par des clicks sur les joysticks, mais SANS DUPLIQUER DES TAS DE CONF. Je veux pouvoir changer le comportement du click sur un de ces boutons sans avoir à retoucher mon copde à des milliards d'endroits !

# Rappel du fonctionnement du script

Sur le principe, le script implémente des jsonref. On a un fichier racine qui contient des #ref vers d'autres fichiers, qui peuvent à leur tour en contenir. Le script fusionne tout ça en un seul fichier vdf. Je te laisse regarder la lib vdf-utils pour ça car ça ne fonctionne pas tout à faire comme des jsonref.

Il y a un mécanisme supplémentaire qui permet de spécialiser les fichiers en fonction du controlleur: Quand on fusionne, on passe en plus un nom de controlleur, et quand le script trouve un fichier nommé "XXX.<nom controller>.vdf", il le fusionne automatiquement en plus du #ref qu'il aura trouvé. Là encore, je te laisse regarder vdf-utils pour comprendre comment ça fonctionne.

Jusque là, ça a bien fonctionné, mais ce n'est plus suffisant pour faire ce que je veux.

# Proposition d'évolution

Je suis un vieux développeur Java backend, et j'ai cette intuiition qui me vient d'un mécanisme de JSF : Les facettes

On gère des propriétés #ref supplémentaires, dont la valeur n'est plus une chaîne, mais un objet. Il sera donc facile de les distinguer des existantes. Et on leur passe en plus des "facets".  Par exemple, dans le fichier parent :
  "#ref"
  {
    "path"    "<chemin vers un vdf à inclure>"
    "facet1"
    {
        contenu vdf, ou #ref
    }
    "facet2"
    {
        ....
    }
}

Puis, dans le fichier vdf inclu, on ajoute des propriétés au milieu du contenu
   "#facet"    "facette1"

Et on met le contenu des facettes à cet emplacement.

# Ce que tu dois faire

Avant de perdre du temps et de l'énergie à implémenter, je veux que tu analyses simplement si c'est faisable, et à quel cout. Mais cette analyse doit être rapide.

Ensuite, je veux que tu regardes les modeshifts qui sont dans inputs/switches/backright/. Prend le temps de regarder comment ils sont utilisés, et comment ça fonctionne.

Mon intuition est que 