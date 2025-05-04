using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class DockingActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string modeName = "DockingControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(modeName);

        public string ControlName() {
            return modeName;
        }

        public bool Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return false;
            if( MapView.MapIsEnabled ) return false;
            if( FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA ) return false;
            
            FlightUIMode mode = FlightUIModeController.Instance.Mode;
            return mode == FlightUIMode.DOCKING;
        }
    }
}