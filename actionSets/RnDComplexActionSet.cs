using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class RnDComplexActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "MenuControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public RefreshType Active() {
            if( HighLogic.LoadedScene != GameScenes.SPACECENTER) return RefreshType.Nope;
            if( SteamControllerPlugin.SpaceCenterBuilding != SpacePortFacility.RnDComplex ) return RefreshType.Nope;
            return RefreshType.Delayed;
        }

        public bool Default() {
            return false;
        }
    }
}