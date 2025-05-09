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
    public class FlightMapDaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("FlightMapDaemon");
        private bool flightBeforePause = false;

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.MapFlightControls;
        }

        public void Start()
        {
            LOGGER.Log("Start");

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void OnDestroy()
        {
            LOGGER.Log("OnDestroy");

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        // ============================================================

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.Log("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;
            
            this.SendEvent(false);

            GameEvents.OnMapEntered.Add(OnMapEntered);
            GameEvents.OnMapExited.Add(OnMapExited);
            
            this.OnMapExited();
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) {
                return;
            }

            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            this.SendEvent(false);
        }

        // ============================================================

        private void OnMapEntered()
        {
            LOGGER.Log("=> OnMapEntered");
            
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Add(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Add(OnVesselChange);

            this.SendEvent(
                InFlightMode()
            );
        }

        private void OnMapExited()
        {
            LOGGER.Log("=> OnMapExited");
            
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            this.SendEvent(false);
        }
        
        private void OnGamePause()
        {
            LOGGER.Log("=> OnGamePause");
            this.flightBeforePause = this.InContext();
            this.SendEvent(false);
        }

        private void OnGameUnpause()
        {
            LOGGER.Log("=> OnGameUnpause");
            this.SendEvent(this.flightBeforePause);
        }

        private void OnFlightUIModeChanged(FlightUIMode mode)
        {
            LOGGER.Log("=> OnFlightUIModeChanged : " + mode.ToString());
            this.SendEvent(
                InFlightMode(mode)
            );
        }

        private void OnVesselChange(Vessel vessel)
        {
            LOGGER.Log("=> OnVesselChange : " + vessel.name);
            this.SendEvent(
                InFlightMode(FlightUIModeController.Instance.Mode)
            );
        }
    }
}
