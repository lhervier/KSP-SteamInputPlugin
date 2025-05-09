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
    public class DockingMapDaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("DockingMapDaemon");
        protected override string ActionGroupName()
        {
            return "DockingMapControls";
        }

        private bool dockingBeforePause = false;

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

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.Log("OnSceneLoaded");
            if( scene.name.ToUpper() != "PFLIGHT4") return;
            
            GameEvents.OnMapEntered.Add(OnMapEntered);
            GameEvents.OnMapExited.Add(OnMapExited);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.Log("OnSceneUnloaded");
            if( scene.name.ToUpper() != "PFLIGHT4" ) {
                return;
            }

            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            
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
                InDockingMode()
            );
        }

        private void OnMapExited()
        {
            LOGGER.Log("=> OnMapExited");
            
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            SendEvent(false);
        }

        private void OnGamePause()
        {
            LOGGER.Log("=> OnGamePause");
            this.dockingBeforePause = this.InContext();
            SendEvent(false);
        }

        private void OnGameUnpause()
        {
            LOGGER.Log("=> OnGameUnpause");
            SendEvent(this.dockingBeforePause);
        }

        private void OnFlightUIModeChanged(FlightUIMode mode)
        {
            LOGGER.Log("=> OnFlightUIModeChanged : " + mode.ToString());
            this.SendEvent(
                InDockingMode(mode)
            );
        }

        private void OnVesselChange(Vessel vessel)
        {
            LOGGER.Log("=> OnVesselChange : " + vessel.name);
            this.SendEvent(
                InDockingMode()
            );
        }
    }
}
