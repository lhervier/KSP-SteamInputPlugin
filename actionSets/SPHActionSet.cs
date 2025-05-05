using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class SPHActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "EditorControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public RefreshType Active() {
            if( !HighLogic.LoadedSceneIsEditor ) return RefreshType.Nope;
            if( SteamControllerPlugin.sceneName != "SPHmodern" ) return RefreshType.Nope;

            return RefreshType.Delayed;
        }

        public bool Default() {
            return false;
        }
    }
}