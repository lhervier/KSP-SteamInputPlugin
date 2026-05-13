Ce dossier contient un mod pour Kerbal Space Program (KSP) qui ajoute un support correct de l'API SteamInput. Le jeu la prend déjà en charge, mais les dernières évolutions l'ont cassée. Ce mode désactive donc le plugin officiel, et en démarre un nouveau qui tient compte de ces dernières évolutions.

Il est divisé en deux :

- Le mod en lui même, écrit en C#, pour Unity.
- Et un script qui génère des configurations pour un ensemble de controleurs.

# Le mod

Il est dans le dossier SteamInputPlugin.

TBC.

# Les configurations de controlleurs

Elles sont dans le dossier "SteamInputConfig".

Une configuration est un fichier .vdf, très long. A l'intérieur, on retrouve :

- des "bindings"
- des "inputs"
- des "groups"
- et des "presets"

## Les "bindings"

Ils correspondent à une action clavier/souris/manette/autre avec un libellé associé.

Dans ce dépôt, il sont calqués directement sur la configuration proposée par le jeu. 

Ainsi, le jeu dit qu'il existe une action "staging" (qui permet de se séparer d'un étage d'une fusée) activable par la touche ESPACE. On va donc créer un binding associé.

Mais en fonction du contexte dans le jeu, la même touche du clavier (ESPACE de nouveau) peut avoir des comportements différents. Elle servira, toujours par exemple, a faire sauter un Kerbal quand il se promène à la surface d'un corps céleste. On va donc aussi le déclarer, ce qui signifie que deux bindings différents peuvent activer la même touche (ou click souris, ou click sur un bouton de manette, etc...). Dans ce cas, ils auront probablement deux libellés différents.

Les libellés correspondent à ce que l'utilisateur vera dans la configuration SteamInput, quand il configure ou visualise la configuration de sa manette au travers de l'interface Steam.

## Les "inputs"

Ils correspondent à une action sur le controlleur lui même, et invoquent habituellement un "binding".

Dans ce dépot, ils sont donc organisés par "mode", c'est à dire par type de regroupement d'actions sur une manette : Le pad directionnel, un joystick, le groupe de boutons, etc... Puis, pour chaque "mode", on a l'inpu correspondant : Si on est sur un groupe de boutons, on aura les boutons "a", "b", "x" et "y". Si on est sur un dpad, on aura les directions "north", "south", "east" et "west", etc...

Ainsi, on va définir que le bouton "A", que l'on trouve dans le mode "four_buttons" (les 4 boutons a, b, x et y), pourra déclencher le binding qui permet de faire un "staging". 

Mais on définira aussi que dans d'autres circonstances (quand on fait se déplacer un Kerbal à la surface d'un corps céleste), il pourra déclencher le binding qui correspond à un saut. Ou changer le mode de déplacement du vaisseau quand on le contrôle par des RCS (rotation versus translation), etc...

On retrouve donc dans les "input" l'ensemble des bindings possibles pour un même action sur la manette.

## Les "groups"

Ils correspondent au fonctionnement d'un ensemble cohérent sur un controlleur habituel. Ils representent la manière dont va se comporte le dpad, le joystick gauche, les triggers, etc...

Ainsi, on va pouvoir dire que les 4 boutons de droite vont pouvoir déclencher un ensemble d'inputs (un input pour le bouton A, un autre pour B, etc...).

Mais de la même manière que pour les inputs et les bindings, un même groupe pourra déclencher des inputs différents en fonction du contexte. On aura donc autant de groups que de combinaisons possibles dans le jeu.

Les 4 boutons ne feront pas la même chose si on pilote ou si on construit une fusée. Ils auront chacun un "group".

## Les "presets"

Les presets sont un ensemble cohérent de groupes. Ils correspondent à un contexte dans le jeu.

Quand on est en mode pilotage de fusée, on va assembler des groupes pour avoir une configuration pratique pour le pilotage. Mais quand on construit une fusée, on va assembler d'autres groupes.

Le passage d'un preset à l'autre est piloté par le jeu lui même. C'est là que le mod est nécessaire : Il détecte les situations (construction d'une fusée, pilotage d'une fusée, etc...) et active tel ou tel preset.

Les presets permettent de créer un mapping entre une zone réelle du contolleur et un groupe correspondant à un mode donné. Par exemple, les presets nomment la zone aves les 4 boutons, le "button_diamond". Et il est possible de la mapper ver un "group" dont le mode est "four_buttons" (Boutons a, b, x et y). Mais il est aussi possible de la mapper vers un "group" dont le mode est "dpad" (Avec les directions north, south, eat et west).

Les presets correspondent donc à une zone physique sur un controlleur (et tous n'ont pas les mêmes ! Un SteamController possède des trackpads, là où un manette XBox n'en a pas !). Là où les groupes correspondent plus à une zone virtuelle, telle qu'on l'a tous dans notre représentation d'un controlleur (un croix directionnelle, des joystick, des trackpads, etc...)

## Le "mode shift"

L'idée est de permettre de dire : Quand l'utilisateur appuie sur ce déclencheur (un bouton par exemple), alors le comportement de telle partie du contolleur change.

Dans ce projet, on utilise beaucoup les boutons qui se trouvent à l'arrière de la manette. Ils ne sont malheureusement pas présents sur toutes les manette, mais KSP est tellement complexe, avec tellement d'actions possibles, que sans eux, c'est difficile de créer une configuration cohérente.

Pour déclarer qu'un modeshift est possible, il y a deux conditions :

- Dans le preset, la zone qui va changer (les 4 boutons par exemple = le "button_diamond") doit être déclaré une fois pour chaque possibilité:
    - On va lui associer un "group" pour son fonctionnement normal
    - Et on va lui en associer un autre, pour son fonctionnement alternatif, en le déclarant grâce au mot clé "modeshift".
- Il faut ensuite un binding sur l'input qui déclenche le changement de mode. Ce binding va utiliser le mot clé "mode_shift" (avec un "_" cette fois) et va cibler la zone (diamond_button) et le groupe déclaré comme alternatif dans le preset.

## Les layers

TBC

## La description de la configuration des controlleurs

### Le problème

Un fichier correspondant à un controlleur est potentiellement très gros, surtout quand on doit gérer de nombreux presets, avec des nombreux bindings.

De plus, il y aura beaucoup de répétitions à l'intérieur car un même groupe pourra être utilisé à plusieurs endroits, et c'est la même chose pour les inputs et les bindings.

Et l'objectif étant de proposer des configurations pour plusieurs manettes, il y aura beaucoup de répétitions entre ces configurations.

Pour modulariser tout cas, ce dépot contient un script node qui assemble des configurations de controlleurs à partir d'un ensemble de fichiers.

### L'implémentation des refs VDF

Le script repose repose sur une implémentation qui ressemble à ce que l'on fait avec des JSONRef, mais sur des fichiers VDF.

Voici un exemple de fichier racine :

    "MonObjet"
    {
        "propriete"     "valeur"
        "sousObjet"
        {
            "propriete"     "valeur"
        }
        "#ref"      "sousfichier1.vdf"
        "#ref"      "sousfichier2.vdf"
    }

Et voici le code du 1er sous fichier nommé "sousfichier1.vdf" : La propriété racine "ref" est obligatoire

    "ref"
    {
        "sousObjet1"
        {
            "propriete1"    "valeur1"
        }
    }

Et le code du 2eme sous fichier nommé "sousfichier2.vdf" : La propriété racine "ref" est obligatoire

    "ref"
    {
        "sousObjet2"
        {
            "propriete2"    "valeur2"
        }
    }

Le résultat de la fusion sera :

    "MonObjet"
    {
        "propriete"     "valeur"
        "sousObjet"
        {
            "propriete"     "valeur"
        }
        "sousObjet1"
        {
            "propriete1"    "valeur1"
        }
        "sousObjet2"
        {
            "propriete2"    "valeur2"
        }
    }

Le fonctionnement n'est donc pas exactement le même que JSON ref. 

### La génération de plusieurs controlleurs

Un fichier controllers.json permet de définir l'ensemble des controlleurs à générer, en donnant son nom, et un chemin vers un fichier VDF racine.

Mais à chaque controlleur, il est en plus possible d'ajouter un "contexte" = un ensemble de clés valeurs qui pourront être utilisées à l'intérieur des ichiers VDF. Pour cela, on utilise la bibliothèque externe "handlebars". Et ces couples de clés/valeurs ne sont rien d'autre que le contexte handlebar.

