using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class MapActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "MapControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public bool Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return false;
            if( MapView.MapIsEnabled ) return true;
            if( FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA ) return false;

            FlightUIMode mode = FlightUIModeController.Instance.Mode;
            return mode == FlightUIMode.MAPMODE;
        }

        public bool Default() {
            return false;
        }
    }
}