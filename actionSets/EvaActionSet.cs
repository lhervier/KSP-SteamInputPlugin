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

        public bool Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return false;
            if( MapView.MapIsEnabled ) return false;
            if( FlightGlobals.ActiveVessel == null ) return false;
            return FlightGlobals.ActiveVessel.isEVA;                
        }
    }
}