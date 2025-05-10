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
    // <summary>
    //  This class is a context daemon that detects when the game is in EVA mode
    // </summary>
    public class EVACtxDaemon : BaseContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("EVACtxDaemon");
        
        private bool evaBeforePause = false;

        public override ActionGroup CorrespondingActionGroup()
        {
            return ActionGroup.EvaControls;
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

        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // LOGGER.Log("OnSceneLoaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4") return;
            
            GameEvents.OnMapEntered.Add(OnMapEntered);
            GameEvents.OnMapExited.Add(OnMapExited);
            
            this.OnMapExited();
        }

        protected void OnSceneUnloaded(Scene scene)
        {
            // LOGGER.Log("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) {
                return;
            }
            this.FireContextEnterOrLeave(false);

            GameEvents.OnMapEntered.Remove(OnMapEntered);
            GameEvents.OnMapExited.Remove(OnMapExited);
            
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.onVesselChange.Remove(OnVesselChange);
        }
        
        // ============================================================

        private void OnGamePause()
        {
            // LOGGER.Log("=> OnGamePause");
            this.evaBeforePause = this.InContext();
            this.FireContextEnterOrLeave(false);
        }

        private void OnGameUnpause()
        {
            // LOGGER.Log("=> OnGameUnpause");
            this.FireContextEnterOrLeave(this.evaBeforePause);
        }

        private void OnMapEntered()
        {
            // LOGGER.Log("=> OnMapEntered");
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.onVesselChange.Remove(OnVesselChange);
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionMode);
            this.FireContextEnterOrLeave(false);
        }

        private void OnMapExited()
        {
            // LOGGER.Log("=> OnMapExited");
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
            GameEvents.onVesselChange.Add(OnVesselChange);
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionMode);
            this.FireContextEnterOrLeave(
                InEVA()
            );
        }

        private void OnVesselChange(Vessel vessel)
        {
            // LOGGER.Log("=> OnVesselChange : " + vessel.name);
            this.FireContextEnterOrLeave(
                InEVA(vessel)
            );
        }

        private void OnEVAConstructionMode(bool mode)
        {
            // LOGGER.Log("=> OnEVAConstructionMode : " + mode);
            if( mode ) {
                this.FireContextEnterOrLeave(false);
            } else {
                this.FireContextEnterOrLeave(
                    InEVA()
                );
            }
        }
    }
}
