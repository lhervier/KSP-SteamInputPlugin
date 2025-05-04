using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class EvaActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "EvaControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public RefreshType Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return RefreshType.Nope;
            if( MapView.MapIsEnabled ) return RefreshType.Nope;
            if( SteamControllerPlugin.GamePaused ) return RefreshType.Nope;
            if( FlightGlobals.ActiveVessel == null ) return RefreshType.Nope;
            if( !FlightGlobals.ActiveVessel.isEVA ) return RefreshType.Nope;
            return RefreshType.Delayed;
        }

        public bool Default() {
            return false;
        }
    }
}