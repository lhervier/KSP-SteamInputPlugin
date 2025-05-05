At the time of writing, the introduction of Maneuvre Mode in KSP 1.6 broke the detection of the action set to activate in game. See bug [https://bugs.kerbalspaceprogram.com/issues/22165](https://bugs.kerbalspaceprogram.com/issues/22165) for more details.

Thanks to the well documented public class "SteamController", writing a plugin that will activate Action Sets is not difficult. So, I reimplemented mine... even with some "help". 

This plugin also add numerous other action groups :

- For the Tracking Station
- For the Mission Builder editor
- A specifi action group when in Construction Mode, while in EVA with an engineer.
- When in Map mode, you will have :
    - An action group when in flight mode
    - And another action group when in docking mode
- And an action group when using the FreeIVA mod.

To use this plugin :

- Add the following plugin in /GameData/SteamController/SteamControllerPlugin.dll
- Create a folder controller_config in your Steam directory (c:\Program Files (x86)\Steam for example). Attention : A folder named "controller_base" already exist !
- Copy the file 'game_actions_220200.vdf" in this folder

Et voilà !