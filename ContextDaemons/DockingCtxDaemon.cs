using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine.SceneManagement;

namespace com.github.lhervier.ksp 
{
    // <summary>
    //  This class is a context daemon that detects when the game is in docking mode
    // </summary>
    public class DockingCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("DockingCtxDaemon");
        private bool dockingBeforePause = false;

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.DockingControls;
        }

        public void Start()
        {
            LOGGER.LogInfo("Start");

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.LogDebug("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;
            
            GameEvents.OnMapEntered.Add(OnMapEntered);
            GameEvents.OnMapExited.Add(OnMapExited);

            this.OnMapExited();
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) {
                return;
            }
            
            this.FireContextEnterOrLeave(false);

            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);
        }

        // ============================================================

        private void OnMapEntered()
        {
            LOGGER.LogDebug("=> OnMapEntered");
            
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);
            
            this.FireContextEnterOrLeave(false);
        }

        private void OnMapExited()
        {
            LOGGER.LogDebug("=> OnMapExited");
            
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
            GameEvents.OnFlightUIModeChanged.Add(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Add(OnVesselChange);

            this.FireContextEnterOrLeave(
                InDockingMode()
            );
        }

        private void OnGamePause()
        {
            LOGGER.LogDebug("=> OnGamePause");

            this.dockingBeforePause = InContext();
            FireContextEnterOrLeave(false);
        }

        private void OnGameUnpause()
        {
            LOGGER.LogDebug("=> OnGameUnpause");

            FireContextEnterOrLeave(this.dockingBeforePause);
        }

        private void OnFlightUIModeChanged(FlightUIMode mode)
        {
            LOGGER.LogDebug("=> OnFlightUIModeChanged : " + mode.ToString());
            this.FireContextEnterOrLeave(
                InDockingMode(mode)
            );
        }

        private void OnVesselChange(Vessel vessel)
        {
            LOGGER.LogDebug("=> OnVesselChange : " + vessel.name + " isEVA : " + vessel.isEVA);
            this.FireContextEnterOrLeave(
                InDockingMode(FlightUIModeController.Instance.Mode)
            );
        }
    }
}
