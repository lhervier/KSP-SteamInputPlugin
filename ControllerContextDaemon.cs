using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;
using SteamController;

namespace com.github.lhervier.ksp 
{
    public abstract class ControllerContextDaemon : MonoBehaviour {

        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("ControllerContextDaemon");

        private readonly EventData<ControllerContextDaemon> _onEnterContext = new EventData<ControllerContextDaemon>("OnEnterAdministrationBuilding");
        public EventData<ControllerContextDaemon> OnEnterContext() {
            return _onEnterContext;
        }

        private readonly EventData<ControllerContextDaemon> _onExitContext = new EventData<ControllerContextDaemon>("OnExitAdministrationBuilding");
        public EventData<ControllerContextDaemon> OnExitContext() {
            return _onExitContext;
        }

        private bool _inContext = false;
        public bool InContext() {
            return _inContext;
        }

        public abstract ActionGroup CorrespondingActionGroup();

        protected void SendEvent(bool inContext) {
            if( inContext == this._inContext ) return;
            this._inContext = inContext;
            if( inContext ) {
                this._onEnterContext.Fire(this);
            } else {
                this._onExitContext.Fire(this);
            }
        }

        protected bool InFlightMode(FlightUIMode flightUiMode) {
            if( this.InEVA() || this.InIVA() ) {
                return false;
            }
            return flightUiMode == FlightUIMode.STAGING || flightUiMode == FlightUIMode.MANEUVER_INFO || flightUiMode == FlightUIMode.MANEUVER_EDIT;
        }

        protected bool InFlightMode() {
            if( FlightUIModeController.Instance == null ) {
                return false;
            }
            return InFlightMode(FlightUIModeController.Instance.Mode);
        }

        protected bool InDockingMode(FlightUIMode flightUiMode) {
            if( this.InEVA() || this.InIVA() ) {
                return false;
            }
            return flightUiMode == FlightUIMode.DOCKING;
        }

        protected bool InDockingMode() {
            if( FlightUIModeController.Instance == null ) {
                return false;
            }
            return InDockingMode(FlightUIModeController.Instance.Mode);
        }
        protected bool InEVA(Vessel vessel) {
            return vessel.isEVA;
        }

        protected bool InEVA() {
            if( FlightGlobals.ActiveVessel == null ) {
                return false;
            }
            return InEVA(FlightGlobals.ActiveVessel);
        }

        protected bool InIVA() {
            if( CameraManager.Instance == null ) {
                return false;
            }
            return CameraManager.Instance.currentCameraMode == CameraManager.CameraMode.IVA;
        }
    }
}