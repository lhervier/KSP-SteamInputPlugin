using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class TrackingStationActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "TrackingStationControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public bool Active() {
            return HighLogic.LoadedScene == GameScenes.TRACKSTATION;
        }

        public bool Default() {
            return false;
        }
    }
}