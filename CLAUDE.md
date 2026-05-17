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

Ainsi, dans l'écran de configuration du jeu, dans la partie "commande", il existe un onglet "Flight", dans lequel on retrouve une section "Other", où on peut définir la touche sur laquelle appuyer pour réaliser l'action "staging" (qui permet de se séparer d'un étage d'une fusée). On va donc créer un binding "Flight/Other/staging.vdf", dans lequel on définira la valeur par défaut proposée par le jeu (touche ESPACE). 

Mais en fonction du contexte dans le jeu, la même touche du clavier (ESPACE de nouveau) peut avoir des comportements différents. Elle servira, toujours par exemple, a faire sauter un Kerbal quand il se promène à la surface d'un corps céleste. On va donc aussi le déclarer, ce qui signifie que deux bindings différents peuvent activer la même touche (ou click souris, ou click sur un bouton de manette, etc...). Dans ce cas, ils auront probablement deux libellés différents.

Les libellés correspondent à ce que l'utilisateur vera dans la configuration SteamInput, quand il configure ou visualise la configuration de sa manette au travers de l'interface Steam. On reprend pour ces libellés, les libellés que l'on a dans l'écran de configuration du jeu.

Il existe aussi des bindings qui ne sont pas configurables dans le jeu, comme par exemple le clique souris, ou les flèches de direction pour faire bouger la caméra. On va aussi définir ces bindings.

Et enfin, il existe des bindings associés à des mods supportés par ce mod, comme FreeIva. Dans ce cas, les bindings sont définis dans un dossier qui porte le nom du mod.

## Les "inputs"

Ils correspondent à une action sur le controlleur lui même, et invoquent habituellement un "binding". Ils définissent en plus le type d'activation de ce binding, comme par exemple lors d'un appui normal, ou lors d'un appui long.

SteamInput défini des "modes", qui correspondent à des zones que l'on retrouve habituellement sur une manette (mais pas seulement). Ils ne sont pas en liens avec ce qui est présent sur la manette physique en elle même, mais correspondent plutôt à des modes de réaction que l'on va associer plus tard (grâce aux groupes et aux presets) à des actions sur la manette. On retrouve par exemple des modes correspondant à un pad directionnel (dpad), à une souris (absolute_mouse ou joystick_mouse), à un groupe de bouttons (four_buttons), ou à une gâchette (trigger).

Chaque mode possède ses propres "activateurs". Par exemple, en mode "four_buttons" (le groupe de boutons de la manette), on aura les activators "button_a", "button_b", "button_x" et "button_y". En mode "dpad" (Pas directionnel), on aura les activators "dpad_north", "dpad_south", "dpad_east", "dpad_west", ainsi que "click". Ici, "click" est un bon exemple car ce n'est pas un activateur disponible sur un dpad habituel. Mais si on lie cet input à un joystick physique de la manette, d'un coup, ça a du sens.

Dans ce dépot, les inputs sont organisés par "mode". Ainsi, on va définir que l'activateur "button_menu", que l'on trouve dans le mode "switches" (qui reprend tous les boutons annexes de la manette), pourra déclencher le binding qui permet d'afficher le clavier virtuel à l'écran quand on appuie longuement dessus. On pourra en plus donner des paramètres (settings) spécifiques à cet activateur. Cela donne le fichier inputs/switches/button_menu/keyboard.vdf.

## Les "groups"

Ils "construisent" un mode en assemblant des inputs entre eux, et en définissant des paramètres (settings) spécifiques au mode lui même. Ils representent la manière dont va se comporter un dpad, un joystick gauche, des triggers, etc... 

SteamInput défini ensuite des "presets", que l'on traite juste après. Ils vont faire le mapping entre des zones physiques du controlleur et des groupes. Ces zones physiques portent des noms qui ressemblent aux modes, mais doivent bien être compris comme étant des objets différents.

Dans ce dépôt, les groupes sont donc organisés selon ces modes physiques. Ainsi, on va pouvoir dire via les presets que la zone avec les 4 boutons physiques (nommée button_diamond) va être associée à un groupe dont le mode est "four_buttons". Le fichier décrivant le groupe sera alors groups/button_diamond/mon-groupe.vdf

## Les "presets"

Les presets sont un ensemble cohérent de groupes mappés à des zones physiques du controlleur. Ils correspondent à un contexte dans le jeu, et c'est le mod (le code donc !) qui décide quel preset activer à quel moment. L'utilisateur ne peut pas choisir lui même de passer d'un preset à un autre.

Les noms de ces zones physiques ressemblent beaucoup aux noms des modes de groupes, mais ce sont pourtant des notions différentes:

- "switch" correspond par exemple aux vrais boutons annexes de la manette. Cette zone physique est souvent mappée sur un groupe dont le mode est "switches", au pluriel. 
- "button_diamond" correspond à la zone physique de la manette avec les 4 boutons. Cette zone physique est souvent mappée vers un groupe dont le mode est "four_buttons", mais ce n'est pas obligatoire. Elle peut aussi être mappée vers un groupe dont le mode est "dpad", auquel cas, le bouton 'y' est vu comme le nord, 'a' comme le sud, etc... 
- C'est aussi dans les presets qu'on va indiquer que le joystick droit de la manette (nommé "joystick") correspond à un groupe dont le mode est "joystick_mouse"
- On va aussi pouvoir indiquer que la zone physique "right_trackpad" (si vous avez un steam controller v1 ou v2, ou une manette Playstation) correspond à un groupe dont le mode est "absolute_mouse".
- Ou bien que la zone "left_trackpad" est mappée vers un groupe dont le mode est "dpad". Il suffira alors de toucher le trackpad en haut, en bas, à gauche ou à droite pour déclencher les activateurs "dpad_north", "dpad_west", etc...
- Et bien sûr, certains mapping n'ont pas de sens, comme mapper un groupe de mode "absolute_mouse" sur la zone "left_trigger"...

Ainsi, lorsqu'on est en mode pilotage de fusée, on va assembler des groupes pour avoir une configuration pratique pour le pilotage. Mais quand on construit une fusée, on va assembler d'autres groupes.

Attention : Si les modes de groupes sont universels, tous les controlleurs n'ont pas les mêmes zones physiques : 

- Un SteamController (v1 ou v2) possède des trackpads, là où un manette XBox n'en a pas. 
- Une manette Playstation en possède aussi (la zone en haut au milieu de la manette est tactile).
- Un Horipad, ou une manette XBox Elite possèdent des back buttons indépendants (ils font parti de la zone physique "switch", et peuvent être mappés sur les activateurs "button_back_right" et "button_back_left" des groupes dont le mode est "switches").
- Le Horipad, ou le Steam Controller v2 possèdent même 4 back buttons !
- Une manette Playstation n'a pas de back buttons. Cependant, certains controlleurs compatibles PS4 (comme la Raiju Tournament Edition) permettent de mapper des boutons existants à des palettes à l'arrière de la manette. Dans ce cas, appuyer sur ces back buttons pourra correspondre à un click sur le joystick droit ou gauche.
- etc...

Les presets doivent donc composer avec les zones physiques présentent sur la manette, et les mapper vers les bons groupes.

## Le "mode shift"

L'idée est de permettre de dire : Quand l'utilisateur appuie sur ce déclencheur (un bouton par exemple), alors le comportement de telle partie du contolleur change.

Dans ce projet, on utilise beaucoup les boutons qui se trouvent à l'arrière de la manette. Ils ne sont malheureusement pas présents sur toutes les manette, mais KSP est tellement complexe, avec tellement d'actions possibles, que sans eux, c'est difficile de créer une configuration cohérente. Ainsi, seules les manettes avec des back buttons sont supportées...

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

Pour modulariser tout cela, ce dépot contient un script node qui assemble des configurations de controlleurs à partir d'un ensemble de fichiers.

### L'implémentation des refs VDF

Le script repose sur une implémentation qui ressemble à ce que l'on fait avec des JSONRef, mais sur des fichiers VDF.

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

### Le passage de paramètres lors de l'inclusion

Un #ref peut passer des paramètres au fichier inclu via une syntaxe de type url. Par exemple :

    "#ref"      "mon-fichier.vdf?param1=valeur&param2=valeur

Dans le fichier inclu, il est ensuite possible de faire référence à ces paramètres grâce à la syntaxe Handlebars :

    "propriete"     "valeur {{param1}} {{param2}} suite..."
    "#ref"          "fichier.vdf?param={{param2}}"

### L'accès aux variables spécifiques au controlleur

Chaque controlleur à construire est déclaré dans le fichier controllers.json. On y retrouve des variables à placer dans le contexte global Handlebars. Et vous pouvez y faire référence avec la même syntaxe Handlebar :

    {{#if (equals dpadZone "left_trackpad)}}
        "propriete"     "valeur"
    {{/if}}

### Les helpers handlebar

Plusieurs helpers Handlebar ont été ajoutés 

- "defined" pour savoir si une valeur est définie dans le context : {{#if (defined variable)}}...
- "equals" pour savoir si une valeur est égale à une autre : {{#if (equals "1" "2")}}... Ici, "1" et "2" sont des valeurs statiques, mais vous pouvez faire référence à des variables du contexte aussi
- "true" pour savoir si une variable booléenne est définie ET égale à true. {{#if (true backButtons)}}

### La génération de plusieurs controlleurs

Un fichier controllers.json permet de définir l'ensemble des controlleurs à générer, en donnant son nom, et un chemin vers un fichier VDF racine.

Mais à chaque controlleur, il est en plus possible d'ajouter un "contexte" = un ensemble de clés valeurs qui pourront être utilisées à l'intérieur des fichiers VDF. Pour cela, on utilise la bibliothèque externe "handlebars". Et ces couples de clés/valeurs ne sont rien d'autre que le contexte handlebar.
