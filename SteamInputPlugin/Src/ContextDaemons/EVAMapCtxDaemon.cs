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
    //  This class is a context daemon that detects when the game is in EVA mode
    //  with the map displayed
    // </summary>
    public class EVAMapCtxDaemon : BaseContextDaemon
    {
        private static readonly SteamInputLogger LOGGER = new SteamInputLogger("EVAMapCtxDaemon");
        
        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.MapEvaControls;
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
            
            this.FireContextEnterOrLeave(false);
            
            GameEvents.OnMapEntered.Add(OnMapEntered);
            GameEvents.OnMapExited.Add(OnMapExited);
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) {
                return;
            }
            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            
            GameEvents.onVesselChange.Remove(OnVesselChange);
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionMode);

            this.FireContextEnterOrLeave(false);
        }

        // ============================================================

        private void OnMapEntered()
        {
            LOGGER.LogTrace("=> OnMapEntered");
            
            GameEvents.onVesselChange.Add(OnVesselChange);
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionMode);

            this.FireContextEnterOrLeave(
                InEVA()
            );
        }

        private void OnMapExited()
        {
            LOGGER.LogTrace("=> OnMapExited");
            GameEvents.onVesselChange.Remove(OnVesselChange);
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionMode);
            this.FireContextEnterOrLeave(false);
        }

        private void OnVesselChange(Vessel vessel)
        {
            LOGGER.LogTrace("=> OnVesselChange : " + vessel.name);
            this.FireContextEnterOrLeave(
                InEVA(vessel)
            );
        }

        private void OnEVAConstructionMode(bool isInConstructionMode)
        {
            LOGGER.LogTrace("=> OnEVAConstructionModeChanged : " + isInConstructionMode);
            if( isInConstructionMode ) {
                this.FireContextEnterOrLeave(false);
            } else {
                this.FireContextEnterOrLeave(
                    InEVA()
                );
            }
        }
    }
}
