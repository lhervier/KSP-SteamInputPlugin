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

        public RefreshType Active() {
            if( HighLogic.LoadedScene != GameScenes.TRACKSTATION ) return RefreshType.Nope;
            return RefreshType.Delayed;
        }

        public bool Default() {
            return false;
        }
    }
}