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

        public RefreshType Active() {
            if( !HighLogic.LoadedSceneIsFlight ) return RefreshType.Nope;
            if( SteamControllerPlugin.GamePaused ) return RefreshType.Nope;
            if( FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA ) return RefreshType.Nope;
            if( CameraManager.Instance != null && CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA ) return RefreshType.Nope;

            if( MapView.MapIsEnabled ) return RefreshType.Nope;
            
            if( FlightUIModeController.Instance == null ) return RefreshType.Nope;

            FlightUIMode mode = FlightUIModeController.Instance.Mode;
            if( mode == FlightUIMode.STAGING || mode == FlightUIMode.MANEUVER_INFO ) {
                return RefreshType.Delayed;
            }
            
            return RefreshType.Nope;
        }

        public bool Default() {
            return false;
        }
    }
}