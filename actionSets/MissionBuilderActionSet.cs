using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class ModeMissionBuilderActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "MissionBuilderControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public RefreshType Active() {
            if( HighLogic.LoadedScene != GameScenes.MISSIONBUILDER ) return RefreshType.Nope;
            return RefreshType.Delayed;
        }

        public bool Default() {
            return false;
        }
    }
}