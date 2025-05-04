using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class ModeMissionBuilderActionSet : MonoBehaviour, IKSPMode {
        private static readonly string modeName = "MissionBuilderControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(modeName);

        public string Name() {
            return modeName;
        }

        public bool Active() {
            return HighLogic.LoadedScene == GameScenes.MISSIONBUILDER;
        }
    }
}