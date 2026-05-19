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
    //  This class is a context daemon that detects when the game is in flight mode
    //  with the map displayed
    // </summary>
    public class FlightMapCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("FlightMapCtxDaemon");
        
        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.MapFlightControls;
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

        // ============================================================

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LOGGER.LogDebug("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;
            
            this.FireContextEnterOrLeave(false);

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

            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            this.FireContextEnterOrLeave(false);
        }

        // ============================================================

        private void OnMapEntered()
        {
            LOGGER.LogTrace("=> OnMapEntered");
            
           GameEvents.OnFlightUIModeChanged.Add(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Add(OnVesselChange);

            this.FireContextEnterOrLeave(
                InFlightMode()
            );
        }

        private void OnMapExited()
        {
            LOGGER.LogTrace("=> OnMapExited");
            
            GameEvents.OnFlightUIModeChanged.Remove(OnFlightUIModeChanged);
            GameEvents.onVesselChange.Remove(OnVesselChange);

            this.FireContextEnterOrLeave(false);
        }
        
        private void OnFlightUIModeChanged(FlightUIMode mode)
        {
            LOGGER.LogTrace("=> OnFlightUIModeChanged : " + mode.ToString());
            this.FireContextEnterOrLeave(
                InFlightMode(mode)
            );
        }

        private void OnVesselChange(Vessel vessel)
        {
            LOGGER.LogTrace("=> OnVesselChange : " + vessel.name);
            this.FireContextEnterOrLeave(
                InFlightMode(FlightUIModeController.Instance.Mode)
            );
        }
    }
}
