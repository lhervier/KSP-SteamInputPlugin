using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class EvaActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string modeName = "EvaControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(modeName);

        public string ControlName() {
            return modeName;
        }

        public bool Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return false;
            if( MapView.MapIsEnabled ) return false;
            if( FlightGlobals.ActiveVessel == null ) return false;
            return FlightGlobals.ActiveVessel.isEVA;                
        }
    }
}