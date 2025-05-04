using UnityEngine;
using System;
using System.Collections;

namespace com.github.lhervier.ksp 
{
    public class DockingActionSet : MonoBehaviour, IKspActionSet {
        private static readonly string controlName = "DockingControls";
        private static SteamControllerLogger LOGGER = new SteamControllerLogger(controlName);

        public string ControlName() {
            return controlName;
        }

        public RefreshType Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return RefreshType.Nope;
            if( SteamControllerPlugin.GamePaused ) return RefreshType.Nope;
            if( FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA ) return RefreshType.Nope;
            
            if( MapView.MapIsEnabled ) return RefreshType.Nope;
            
            if( FlightUIModeController.Instance == null ) return RefreshType.Nope;

            if( FlightUIModeController.Instance.Mode == FlightUIMode.DOCKING ) {
                return RefreshType.Delayed;
            }
            
            return RefreshType.Nope;
        }

        public bool Default() {
            return false;
        }
    }
}