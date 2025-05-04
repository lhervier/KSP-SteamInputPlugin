using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class FlightActionSet : MonoBehaviour, IKSPMode {
        private static readonly string modeName = "FlightControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(modeName);

        public string Name() {
            return modeName;
        }

        public bool Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return false;
            if( MapView.MapIsEnabled ) return false;
            if( FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA ) return false;
            
            FlightUIMode mode = FlightUIModeController.Instance.Mode;
            return mode == FlightUIMode.STAGING || mode == FlightUIMode.MANEUVER_INFO;
        }
    }
}