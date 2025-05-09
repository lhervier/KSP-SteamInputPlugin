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
    public class EVADaemon : ControllerContextDaemon
    {
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("EVADaemon");
        
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
            this.SendEvent(false);

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
            this.SendEvent(false);
        }

        private void OnGameUnpause()
        {
            // LOGGER.Log("=> OnGameUnpause");
            this.SendEvent(this.evaBeforePause);
        }

        private void OnMapEntered()
        {
            // LOGGER.Log("=> OnMapEntered");
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
            GameEvents.onVesselChange.Remove(OnVesselChange);
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionMode);
            this.SendEvent(false);
        }

        private void OnMapExited()
        {
            // LOGGER.Log("=> OnMapExited");
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);
            GameEvents.onVesselChange.Add(OnVesselChange);
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionMode);
            this.SendEvent(
                InEVA()
            );
        }

        private void OnVesselChange(Vessel vessel)
        {
            // LOGGER.Log("=> OnVesselChange : " + vessel.name);
            this.SendEvent(
                InEVA(vessel)
            );
        }

        private void OnEVAConstructionMode(bool mode)
        {
            // LOGGER.Log("=> OnEVAConstructionMode : " + mode);
            if( mode ) {
                this.SendEvent(false);
            } else {
                this.SendEvent(
                    InEVA()
                );
            }
        }
    }
}
