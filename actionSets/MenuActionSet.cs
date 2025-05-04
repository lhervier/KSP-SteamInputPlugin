using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class MenuActionSet : MonoBehaviour, IKSPMode {
        private static readonly string modeName = "MenuControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(modeName);

        public string Name() {
            return modeName;
        }

        public bool Active() {
            // Menu mode is the default mode when no other mods can be detected
            if( HighLogic.LoadedSceneIsFlight ) return false;
            if( HighLogic.LoadedScene == GameScenes.TRACKSTATION ) return false;
            if( HighLogic.LoadedSceneIsEditor) return false;
            if( HighLogic.LoadedScene == GameScenes.MISSIONBUILDER ) return false;
            return true;
        }
    }
}