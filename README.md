At the time of writing, the introduction of Maneuvre Mode in KSP 1.6 broke the detection of the action set to activate in game. See bug [https://bugs.kerbalspaceprogram.com/issues/22165](https://bugs.kerbalspaceprogram.com/issues/22165) for more details.

Thanks to the well documented public class "SteamController", writing a plugin that will activate Action Sets is not difficult. So, I reimplemented mine...

This plugin also add numerous other action groups :

- For the Tracking Station
- For the Mission Builder editor
- A specific action group when in Construction Mode, while in EVA with an engineer.
- When in Map mode, you will have :
    - An action group when in flight mode
    - Another action group when in docking mode
    - And another when in EVA mode
- And an action group when using the FreeIVA mod.

But using this plugin is not straightforward, becasue KSP come with its own action groups. And we need to change this :

- Create a folder named "controller_config" in your Steam directory (for example "C:\Program Files (x86)\Steam\controller_config"). Note that a folder named "controller_base" already exist. 
- Copy the file 'game_actions_220200.vdf" in this folder

Then, deploy the plugin into you GameData folder as usual.
