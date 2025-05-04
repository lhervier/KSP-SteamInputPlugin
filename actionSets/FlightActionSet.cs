using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class FlightActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "FlightControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public bool Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return false;
            if( MapView.MapIsEnabled ) return false;
            if( FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA ) return false;
            
            FlightUIMode mode = FlightUIModeController.Instance.Mode;
            return mode == FlightUIMode.STAGING || mode == FlightUIMode.MANEUVER_INFO;
        }

        public bool Default() {
            return false;
        }
    }
}