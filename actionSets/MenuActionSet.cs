using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class MenuActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "MenuControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public bool Active() {
            // Menu mode is the default mode when no other mods can be detected
            if( HighLogic.LoadedSceneIsFlight ) return false;
            if( HighLogic.LoadedScene == GameScenes.TRACKSTATION ) return false;
            if( HighLogic.LoadedSceneIsEditor) return false;
            if( HighLogic.LoadedScene == GameScenes.MISSIONBUILDER ) return false;
            return true;
        }

        public bool Default() {
            return true;
        }
    }
}